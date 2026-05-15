using LicenseManagement.EndUser.Diagnostics;
using LicenseManagement.EndUser.Exceptions;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LicenseManagement.EndUser.Utilities
{
    /// <summary>
    /// Centralized HTTP helper. Owns timeout, retry-with-backoff for 429/503, Retry-After honoring,
    /// X-Correlation-Id propagation, optional Idempotency-Key for POSTs, RFC 7807 problem+json
    /// error parsing, and translation of common status codes into typed exceptions.
    /// </summary>
    /// <remarks>
    /// All endpoint helpers in the library funnel through this. Sync overloads exist for WiX
    /// custom-action consumers (where awaiting is impossible) and wrap the async path in a
    /// <see cref="Task.Run(Func{Task})"/> to escape any caller-supplied <see cref="SynchronizationContext"/>.
    /// </remarks>
    internal static class ApiHttp
    {
        // ---------- async, JSON body in / JSON response out ----------

        internal static async Task<TResponse> SendJsonAsync<TResponse>(
            HttpMethod method,
            string relativePath,
            string apiKey,
            object body = null,
            string idempotencyKey = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var raw = await SendRawAsync(method, relativePath, apiKey, body, idempotencyKey, cancellationToken)
                .ConfigureAwait(false);
            return raw.Length == 0 ? default(TResponse) : JsonConvert.DeserializeObject<TResponse>(raw);
        }

        // ---------- async, JSON body in / status code only out (POST/PATCH semantics) ----------

        internal static async Task<HttpStatusCode> SendForStatusAsync(
            HttpMethod method,
            string relativePath,
            string apiKey,
            object body = null,
            string idempotencyKey = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var response = await SendCoreAsync(method, relativePath, apiKey, body, idempotencyKey, cancellationToken)
                .ConfigureAwait(false))
            {
                // For POST/PATCH endpoints the caller branches on status code (Created/Conflict/NoContent).
                // Don't translate 4xx/5xx into exceptions here — the calling handler decides what to do.
                return response.StatusCode;
            }
        }

        // ---------- async, raw string response (for license XML) ----------

        internal static async Task<string> GetStringAsync(
            string relativePath,
            string apiKey,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await SendRawAsync(HttpMethod.Get, relativePath, apiKey, body: null, idempotencyKey: null, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        // ---------- sync wrappers (for WiX custom actions) ----------

        internal static TResponse SendJson<TResponse>(
            HttpMethod method,
            string relativePath,
            string apiKey,
            object body = null,
            string idempotencyKey = null)
            => RunSync(() => SendJsonAsync<TResponse>(method, relativePath, apiKey, body, idempotencyKey));

        internal static HttpStatusCode SendForStatus(
            HttpMethod method,
            string relativePath,
            string apiKey,
            object body = null,
            string idempotencyKey = null)
            => RunSync(() => SendForStatusAsync(method, relativePath, apiKey, body, idempotencyKey));

        internal static string GetString(string relativePath, string apiKey)
            => RunSync(() => GetStringAsync(relativePath, apiKey));

        // Defensively run async work on the thread pool so any caller-supplied SynchronizationContext
        // (e.g. SolidWorks STA) cannot deadlock on `.GetResult()`. WiX CAs have no sync context so this
        // is harmless there.
        private static T RunSync<T>(Func<Task<T>> work)
            => Task.Run(work).GetAwaiter().GetResult();

        // ---------- core ----------

        private static async Task<string> SendRawAsync(
            HttpMethod method,
            string relativePath,
            string apiKey,
            object body,
            string idempotencyKey,
            CancellationToken cancellationToken)
        {
            using (var response = await SendCoreAsync(method, relativePath, apiKey, body, idempotencyKey, cancellationToken)
                .ConfigureAwait(false))
            {
                if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                {
                    if (response.Content == null) return string.Empty;
                    return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }

                // Non-2xx on a "give me data" call is always an error. Translate to typed exception.
                var (problem, raw) = await TryReadProblemAsync(response).ConfigureAwait(false);
                throw BuildException(response, problem, raw, ExtractCorrelationId(response));
            }
        }

        private static async Task<HttpResponseMessage> SendCoreAsync(
            HttpMethod method,
            string relativePath,
            string apiKey,
            object body,
            string idempotencyKey,
            CancellationToken cancellationToken)
        {
            var attempts = Math.Max(1, LicenseHandlingOptions.MaxAttempts);
            var deadline = DateTime.UtcNow + LicenseHandlingOptions.MaxRetryDuration;
            var logger = LicenseHandlingOptions.Logger;

            Exception lastTransient = null;

            for (int attempt = 1; attempt <= attempts; attempt++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var request = BuildRequest(method, relativePath, apiKey, body, idempotencyKey))
                using (var perRequestCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
                {
                    perRequestCts.CancelAfter(LicenseHandlingOptions.RequestTimeout);
                    var sw = Stopwatch.StartNew();
                    HttpResponseMessage response = null;
                    try
                    {
                        response = await WebApiClient.HttpClient
                            .SendAsync(request, HttpCompletionOption.ResponseHeadersRead, perRequestCts.Token)
                            .ConfigureAwait(false);
                        sw.Stop();
                        LogResponse(logger, method, relativePath, apiKey, response, sw.ElapsedMilliseconds, attempt);

                        // 401/403 = bad API key. Don't retry.
                        if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                        {
                            var (problem, raw) = await TryReadProblemAsync(response).ConfigureAwait(false);
                            response.Dispose();
                            throw new InvalidApiKeyException(problem?.Detail ?? problem?.Title ?? raw ?? "License server rejected the API key.");
                        }

                        // Retry on 429 and 5xx (except 501 which is permanent).
                        if (IsTransient(response.StatusCode))
                        {
                            if (attempt >= attempts || DateTime.UtcNow >= deadline)
                            {
                                if (response.StatusCode == (HttpStatusCode)429)
                                {
                                    var retryAfter = response.Headers.RetryAfter?.Delta;
                                    response.Dispose();
                                    throw new RateLimitException("License server rate-limited the request.", retryAfter);
                                }
                                // Final attempt failed — surface the body.
                                return response;
                            }

                            var delay = ComputeBackoff(response, attempt, deadline);
                            response.Dispose();
                            response = null;
                            await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
                            continue;
                        }

                        // 2xx, 3xx, or non-transient 4xx — return as-is, caller decides.
                        return response;
                    }
                    catch (HttpRequestException ex) when (attempt < attempts && DateTime.UtcNow < deadline)
                    {
                        sw.Stop();
                        logger.Log(LicenseLogLevel.Warning,
                            $"HTTP {method.Method} /{relativePath} attempt {attempt}/{attempts} failed: {ex.Message}", ex);
                        response?.Dispose();
                        lastTransient = ex;
                        await Task.Delay(ComputeBackoff(null, attempt, deadline), cancellationToken).ConfigureAwait(false);
                    }
                    catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested && attempt < attempts && DateTime.UtcNow < deadline)
                    {
                        // Timeout (not user cancellation). Treat as transient.
                        sw.Stop();
                        logger.Log(LicenseLogLevel.Warning,
                            $"HTTP {method.Method} /{relativePath} attempt {attempt}/{attempts} timed out after {LicenseHandlingOptions.RequestTimeout.TotalSeconds:F1}s");
                        response?.Dispose();
                        lastTransient = new NetworkUnavailableException(
                            $"Request to /{relativePath} timed out after {LicenseHandlingOptions.RequestTimeout.TotalSeconds:F1}s.",
                            null);
                        await Task.Delay(ComputeBackoff(null, attempt, deadline), cancellationToken).ConfigureAwait(false);
                    }
                }
            }

            // All retries exhausted on a network error.
            throw lastTransient ?? new NetworkUnavailableException();
        }

        private static HttpRequestMessage BuildRequest(HttpMethod method, string relativePath, string apiKey, object body, string idempotencyKey)
        {
            var request = new AuthorizedRequest(method, relativePath, apiKey);

            // Send a correlation id so the server logs and our logs can be cross-referenced.
            request.Headers.TryAddWithoutValidation("X-Correlation-Id", Guid.NewGuid().ToString("N"));

            if (!string.IsNullOrEmpty(idempotencyKey))
                request.Headers.TryAddWithoutValidation("Idempotency-Key", idempotencyKey);

            if (body != null)
            {
                var json = JsonConvert.SerializeObject(body);
                var content = new StringContent(json, Encoding.UTF8);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                request.Content = content;
            }

            return request;
        }

        private static bool IsTransient(HttpStatusCode status)
        {
            int code = (int)status;
            if (code == 429) return true;                            // rate limit
            if (code == 408) return true;                            // request timeout
            if (code >= 500 && code != 501 && code != 505) return true; // server error (skip permanent ones)
            return false;
        }

        private static TimeSpan ComputeBackoff(HttpResponseMessage response, int attempt, DateTime deadline)
        {
            // Honor server-supplied Retry-After when present (seconds OR HTTP-date).
            TimeSpan? serverHint = response?.Headers.RetryAfter?.Delta;
            if (response?.Headers.RetryAfter?.Date is DateTimeOffset retryAt && retryAt > DateTimeOffset.UtcNow)
                serverHint = retryAt - DateTimeOffset.UtcNow;

            // Exponential backoff with jitter: 200ms, 400ms, 800ms…
            var exponential = TimeSpan.FromMilliseconds(200 * Math.Pow(2, attempt - 1));
            var chosen = serverHint ?? exponential;

            // Don't sleep past the overall deadline.
            var remaining = deadline - DateTime.UtcNow;
            if (chosen > remaining) chosen = remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;

            // And cap any single backoff at 5s so a server sending a huge Retry-After can't trap a CA.
            if (chosen > TimeSpan.FromSeconds(5)) chosen = TimeSpan.FromSeconds(5);

            return chosen;
        }

        private static async Task<(ProblemDetails problem, string raw)> TryReadProblemAsync(HttpResponseMessage response)
        {
            if (response.Content == null) return (null, null);
            string raw;
            try
            {
                raw = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
            catch
            {
                return (null, null);
            }

            if (string.IsNullOrWhiteSpace(raw)) return (null, null);

            var contentType = response.Content.Headers.ContentType?.MediaType;
            if (contentType == "application/problem+json" || contentType == "application/json")
            {
                try
                {
                    return (JsonConvert.DeserializeObject<ProblemDetails>(raw), raw);
                }
                catch
                {
                    // fall through — not parseable, return raw
                }
            }
            return (null, raw);
        }

        private static string ExtractCorrelationId(HttpResponseMessage response)
        {
            if (response.Headers.TryGetValues("X-Correlation-Id", out var values))
                foreach (var v in values) return v;
            return null;
        }

        private static Exception BuildException(HttpResponseMessage response, ProblemDetails problem, string raw, string correlationId)
        {
            if (problem != null)
            {
                return new ProblemDetailsException(
                    response.StatusCode,
                    problem.Type,
                    problem.Title,
                    problem.Detail,
                    problem.Instance,
                    correlationId);
            }

            // No problem+json — fall back to a generic ApiException with the body in the message.
            var msg = !string.IsNullOrEmpty(raw)
                ? $"License server returned {(int)response.StatusCode} {response.ReasonPhrase}: {Truncate(raw, 500)}"
                : $"License server returned {(int)response.StatusCode} {response.ReasonPhrase}";
            if (correlationId != null) msg += $" (correlationId={correlationId})";
            return new ApiException(msg, response.StatusCode);
        }

        private static string Truncate(string s, int max) => s.Length <= max ? s : s.Substring(0, max) + "…";

        private static void LogResponse(ILicenseLogger logger, HttpMethod method, string path, string apiKey, HttpResponseMessage response, long elapsedMs, int attempt)
        {
            var level = (int)response.StatusCode >= 500 ? LicenseLogLevel.Warning
                      : (int)response.StatusCode >= 400 ? LicenseLogLevel.Information
                      : LicenseLogLevel.Debug;
            logger.Log(level,
                $"HTTP {method.Method} /{path} attempt={attempt} status={(int)response.StatusCode} elapsed={elapsedMs}ms key=…{LastFour(apiKey)} cid={ExtractCorrelationId(response) ?? "-"}");
        }

        private static string LastFour(string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey)) return "(none)";
            return apiKey.Length <= 4 ? apiKey : apiKey.Substring(apiKey.Length - 4);
        }

        // RFC 7807 problem details payload shape.
        private sealed class ProblemDetails
        {
            public string Type { get; set; }
            public string Title { get; set; }
            public string Detail { get; set; }
            public string Instance { get; set; }
            public int Status { get; set; }
        }
    }
}
