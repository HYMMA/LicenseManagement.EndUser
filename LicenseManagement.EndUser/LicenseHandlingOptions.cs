using System;
using LicenseManagement.EndUser.Diagnostics;

namespace LicenseManagement.EndUser
{
    /// <summary>
    /// Process-wide HTTP and diagnostics options for the license handling pipeline.
    /// Read by <c>ApiHttp</c> and the endpoint helpers. Set values once at app start
    /// (or before the first <c>HandleLicense</c> / <c>HandleLicenseAsync</c> call).
    /// </summary>
    /// <remarks>
    /// In a WiX deferred custom action context, <see cref="MaxRetryDuration"/> must stay small
    /// enough that the action does not blow past the implicit MSI timeout. The default 10s
    /// is a deliberate ceiling for that reason.
    /// </remarks>
    public static class LicenseHandlingOptions
    {
        /// <summary>
        /// Per-request HTTP timeout. Default 15s. Avoid relying on <see cref="System.Net.Http.HttpClient"/>'s
        /// 100s default — a hung TLS handshake on a flaky network would otherwise stall application launch.
        /// </summary>
        public static TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(15);

        /// <summary>
        /// Hard ceiling on total wall-clock time spent retrying transient failures (429, 503, network errors).
        /// Default 10s — keeps WiX deferred custom actions inside their implicit MSI timeout.
        /// </summary>
        public static TimeSpan MaxRetryDuration { get; set; } = TimeSpan.FromSeconds(10);

        /// <summary>
        /// Maximum number of attempts (including the initial one) per call. Default 3.
        /// </summary>
        public static int MaxAttempts { get; set; } = 3;

        /// <summary>
        /// Optional logger for HTTP and chain diagnostics. Default is no-op.
        /// </summary>
        public static ILicenseLogger Logger { get; set; } = NullLicenseLogger.Instance;

        /// <summary>
        /// Base URL of the license server API, including trailing slash.
        /// Defaults to <see cref="Constants.BaseAddress"/>. Override in tests or for self-hosted servers.
        /// Must be set before the first API call — the HTTP client is initialized lazily and caches the value.
        /// </summary>
        public static string ServerBaseAddress { get; set; } = Constants.BaseAddress;
    }
}
