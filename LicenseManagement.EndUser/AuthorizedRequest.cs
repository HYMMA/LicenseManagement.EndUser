using System.Net;
using System.Net.Http;

namespace LicenseManagement.EndUser
{
    internal class AuthorizedRequest : HttpRequestMessage
    {
        public AuthorizedRequest(HttpMethod method, string uri, string apiKey) : base(method, uri)
        {
            Headers.Add(Constants.ApiKeyHeader, apiKey);

            //this is important when calling the server from very old applications such as SOLIDWORKS
            //Note: our server blocks TLS1
            //https://www.smarterasp.net/support/kb/a1968/how-to-fix-error-underlying-connection-was-closed-an-unexpected-error-occurred-on.aspx?KBSearchID=1566361
            //https://blogs.perficient.com/2016/04/28/tls-1-2-and-net-support/
            if (ServicePointManager.SecurityProtocol != SecurityProtocolType.SystemDefault)
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }
    }
}
