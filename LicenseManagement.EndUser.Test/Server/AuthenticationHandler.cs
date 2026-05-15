using System.Net.Http;

namespace LicenseManagement.EndUser.Test.Server
{
    class AuthenticationHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add("X-API-KEY", ContextManager.ApiKey);
            return base.SendAsync(request, cancellationToken);
        }
    }
}
