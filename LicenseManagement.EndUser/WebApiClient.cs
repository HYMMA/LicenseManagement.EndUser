using HttpClientFactory.Impl;
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
            var client = new PerHostHttpClientFactory().GetHttpClient(Constants.BaseAddress);
            client.BaseAddress = new Uri(Constants.BaseAddress);
            return client;
        }
    }
}
