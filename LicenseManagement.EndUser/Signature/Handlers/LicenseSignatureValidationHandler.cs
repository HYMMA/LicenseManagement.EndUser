using LicenseManagement.EndUser.License.Handlers;
using LicenseManagement.EndUser.Signature.EndPoint;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using System.Xml;

namespace LicenseManagement.EndUser.Signature.Handlers
{
    internal class LicenseSignatureValidationHandler : LicenseValidationHandler
    {
        public LicenseSignatureValidationHandler() { }

        bool IsSigValid(LicHandlingContext context)
        {
            if (string.IsNullOrEmpty(context.PublisherPreferences.PublicKey))
                return false;

            if (!IsPublicOnlyKey(context.PublisherPreferences.PublicKey))
                return false;

            // Setting XmlResolver=null is the standard mitigation against XML External Entity (XXE)
            // attacks. Even though the document is signed, the resolver runs *before* signature
            // verification and would happily fetch external DTDs / entities. Belt and braces.
            var xmlDocument = new XmlDocument { PreserveWhitespace = true, XmlResolver = null };
            xmlDocument.LoadXml(context.SignedLicense);

            var signatures = xmlDocument.GetElementsByTagName("Signature");
            if (signatures.Count != 1)
                return false;

            var signedXml = new SignedXml(xmlDocument);
            signedXml.LoadXml((XmlElement)signatures[0]);

            using (var rsa = RSA.Create())
            {
                rsa.FromXmlString(context.PublisherPreferences.PublicKey);
                return signedXml.CheckSignature(rsa);
            }
        }

        // Reject XML keys that contain private-key elements. If a publisher accidentally
        // hands a private key (which contains the public key as a subset), the signature
        // would still verify but the publisher's private key would be embedded in every
        // shipped binary. Refuse to load it.
        private static bool IsPublicOnlyKey(string xmlKey)
        {
            return xmlKey.IndexOf("<P>", StringComparison.Ordinal) < 0
                && xmlKey.IndexOf("<Q>", StringComparison.Ordinal) < 0
                && xmlKey.IndexOf("<D>", StringComparison.Ordinal) < 0
                && xmlKey.IndexOf("<DP>", StringComparison.Ordinal) < 0
                && xmlKey.IndexOf("<DQ>", StringComparison.Ordinal) < 0
                && xmlKey.IndexOf("<InverseQ>", StringComparison.Ordinal) < 0;
        }

        void GetPublicKeyFromServer(LicHandlingContext context)
        {
            // Refuse plain HTTP for the public-key fetch. This is the trust root for license
            // signature validation — if it travels over plain HTTP, a network attacker can
            // swap in their own key and get the SDK to accept arbitrary licenses.
            if (!Constants.BaseAddress.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                throw new CryptographicException(
                    "Refusing to fetch the license-signing public key over a non-https endpoint. " +
                    "Set Constants.BaseAddress to an https URL.");
            }

            var client = new SignatureApiEndPoint(context.PublisherPreferences.ApiKey);
            context.PublisherPreferences.PublicKey = client.GetPublicKey();
        }

        void SetNextHandler(LicHandlingContext context)
        {
            // Local public-key validation takes precedence over the server-fetched one because
            // it's much faster — this runs at every app start. Trade-off: if the publisher rotates
            // keys server-side, deployed apps with the old key will fail until they upgrade.
            if (IsSigValid(context))
            {
                SetNext(new LicenseParsingHandler());
                return;
            }

            if (context.IsLicenseFreshOutOfServer)
            {
                try
                {
                    GetPublicKeyFromServer(context);
                }
                catch (Exception ex)
                {
                    SetNextError(context, new CryptographicException("Could not fetch public key.", ex));
                    return;
                }

                if (IsSigValid(context))
                    SetNext(new LicenseParsingHandler());
                else
                    SetNextError(context, new CryptographicException("Verification failed: Signature was not valid"));
            }
            else
            {
                // Public key may have been rotated on the server — refresh by re-fetching the license.
                SetNext(new ApiGetLicenseHandler());
            }
        }

        public override async Task HandleContextAsync(LicHandlingContext context)
        {
            SetNextHandler(context);
            await nextHandler.HandleContextAsync(context).ConfigureAwait(false);
        }

        public override void HandleContext(LicHandlingContext context)
        {
            SetNextHandler(context);
            nextHandler.HandleContext(context);
        }
    }
}