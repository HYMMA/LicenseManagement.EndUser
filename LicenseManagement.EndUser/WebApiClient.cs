#if !NET8_0_OR_GREATER
using HttpClientFactory.Impl;
#endif
using System;
using System.Net.Http;

namespace LicenseManagement.EndUser
{
    /// <summary>
    /// Provides a shared <see cref="HttpClient"/>. The factory caches per host so we don't
    /// blow through sockets — every endpoint helper goes through this single property.
    /// </summary>
    /// <remarks>
    /// Per-request timeout is enforced by <see cref="Utilities.ApiHttp"/> using a linked
    /// <see cref="System.Threading.CancellationTokenSource"/>, NOT by <see cref="HttpClient.Timeout"/>.
    /// We don't set HttpClient.Timeout because the factory may share the instance across consumers,
    /// and mutating the Timeout property is documented as not thread-safe.
    /// </remarks>
    internal static class WebApiClient
    {
        private static readonly Lazy<HttpClient> _client = new Lazy<HttpClient>(BuildClient);

        internal static HttpClient HttpClient => _client.Value;

        private static HttpClient BuildClient()
        {
            var baseAddress = LicenseHandlingOptions.ServerBaseAddress ?? Constants.BaseAddress;
#if NET8_0_OR_GREATER
            // On .NET 8 the net45-only PerHostHttpClientFactory can't load, so we build the
            // client directly. PooledConnectionLifetime recycles pooled connections (and so
            // re-resolves DNS) periodically — the modern, built-in equivalent of the net481
            // factory's connection-lease recycle. Without it this long-lived, process-wide
            // Lazy<HttpClient> would pin connections to the first resolved IP forever, so a
            // server IP/failover change would break license checks until the host restarts.
            var handler = new SocketsHttpHandler { PooledConnectionLifetime = TimeSpan.FromMinutes(2) };
            var client = new HttpClient(handler) { BaseAddress = new Uri(baseAddress) };
#else
            var client = new PerHostHttpClientFactory().GetHttpClient(baseAddress);
            client.BaseAddress = new Uri(baseAddress);
#endif
            return client;
        }
    }
}
