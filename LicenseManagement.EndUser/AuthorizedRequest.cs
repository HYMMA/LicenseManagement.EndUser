using System.Net;
using System.Net.Http;
using System.Threading;

namespace LicenseManagement.EndUser
{
    internal class AuthorizedRequest : HttpRequestMessage
    {
        private static int _tlsBootstrapped;

        public AuthorizedRequest(HttpMethod method, string uri, string apiKey) : base(method, uri)
        {
            Headers.Add(Constants.ApiKeyHeader, apiKey);
            EnsureTls12Available();
        }

        // Some hosts (e.g. SolidWorks running on .NET Framework with default settings) ship with
        // TLS 1.2 disabled. Server requires TLS 1.2 or higher. We OR Tls12 onto the existing
        // SecurityProtocol value rather than replacing it, so a host that has explicitly enabled
        // TLS 1.3 keeps it. Done once per process.
        private static void EnsureTls12Available()
        {
            if (Interlocked.Exchange(ref _tlsBootstrapped, 1) != 0) return;
            if (ServicePointManager.SecurityProtocol == SecurityProtocolType.SystemDefault) return;
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
        }
    }
}
