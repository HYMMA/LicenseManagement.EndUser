using System.Net.Http;

namespace Hymma.Lm.EndUser.Test.Server
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
