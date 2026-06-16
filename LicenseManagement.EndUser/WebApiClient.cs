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
            // On .NET 8 the built-in SocketsHttpHandler pools connections, and this static
            // Lazy already yields a single shared HttpClient — the recommended pattern — so
            // the net45-only PerHostHttpClientFactory (which can't load on .NET 8) is not
            // needed. Behaviour is identical: one pooled client with the base address set.
            var client = new HttpClient { BaseAddress = new Uri(baseAddress) };
#else
            var client = new PerHostHttpClientFactory().GetHttpClient(baseAddress);
            client.BaseAddress = new Uri(baseAddress);
#endif
            return client;
        }
    }
}
