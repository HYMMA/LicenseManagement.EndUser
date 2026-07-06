using System.Threading;
using System.Threading.Tasks;
using LicenseManagement.EndUser.License.EndPoint;

namespace LicenseManagement.EndUser
{
    /// <summary>
    /// Signed-license wire format. <see cref="Xml"/> is the default enveloped XML-DSig
    /// the .NET SDK consumes; the others are compact JWS tokens for embedded / non-.NET
    /// consumers (a device, a native engine) that verify offline with the matching
    /// public key. Mirrors the server's <c>?format=</c> values.
    /// </summary>
    public enum LicenseFormat
    {
        Xml = 0,
        Jws = 1,    // JWS RS256 (RSA)
        Es256 = 2,  // JWS ES256 (ECDSA P-256)
        EdDsa = 3   // JWS EdDSA (Ed25519)
    }

    /// <summary>
    /// Fetches a compact, portably-verifiable signed license for an already-registered
    /// computer+product — the token an embedded consumer verifies offline. This is
    /// additive: it does not touch the XML activation flow. Run the normal
    /// Launch/Register first so the computer is registered and the license is valid,
    /// then fetch the compact form for the ids on the resulting license.
    /// </summary>
    public static class CompactLicense
    {
        /// <summary>The <c>?format=</c> query value for a format (null for Xml).</summary>
        public static string ToQuery(LicenseFormat format)
        {
            switch (format)
            {
                case LicenseFormat.Jws: return "jws";
                case LicenseFormat.Es256: return "es256";
                case LicenseFormat.EdDsa: return "eddsa";
                default: return null; // Xml: no format parameter
            }
        }

        /// <summary>
        /// Fetch the compact signed license (raw JWS string) for the given computer and
        /// product. The compact formats require an active PAID license in live mode
        /// (server policy); a trial can only be issued as XML.
        /// </summary>
        public static Task<string> FetchAsync(
            string apiKey, string computerId, string productId,
            LicenseFormat format, uint validDays = 90,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var endpoint = new LicenseApiEndPoint(apiKey);
            return endpoint.GetCompactLicenseAsync(computerId, productId, ToQuery(format), validDays, cancellationToken);
        }
    }
}
