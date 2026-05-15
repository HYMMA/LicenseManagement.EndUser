using LicenseManagement.EndUser.Utilities;
using System.Threading;
using System.Threading.Tasks;

namespace LicenseManagement.EndUser.Signature.EndPoint
{
    /// <summary>
    /// Wraps the /signingKeys endpoint that serves the publisher's RSA public key (XML format).
    /// </summary>
    internal class SignatureApiEndPoint
    {
        private const string Path = "signingKeys/.xml";
        private readonly string _apiKey;

        internal SignatureApiEndPoint(string apiKey)
        {
            _apiKey = apiKey;
        }

        internal string GetPublicKey()
            => ApiHttp.GetString(Path, _apiKey);

        internal Task<string> GetPublicKeyAsync(CancellationToken cancellationToken = default(CancellationToken))
            => ApiHttp.GetStringAsync(Path, _apiKey, cancellationToken);
    }
}
