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
            // swap in their own key and get the SDK to accept arbitrary licenses. Guard the
            // address actually used (LicenseHandlingOptions.ServerBaseAddress, which the HTTP
            // layer reads) — not just the compile-time constant. http is allowed ONLY for a
            // loopback host, so the local test/dev server (http://localhost:7298) still works
            // while a misconfigured non-local http endpoint is rejected.
            var effective = LicenseHandlingOptions.ServerBaseAddress ?? Constants.BaseAddress;
            if (!IsSecureOrLoopback(effective))
            {
                throw new CryptographicException(
                    "Refusing to fetch the license-signing public key over a non-https, non-loopback endpoint. " +
                    "Use an https URL (http is permitted only for localhost during testing).");
            }

            var client = new SignatureApiEndPoint(context.PublisherPreferences.ApiKey);
            context.PublisherPreferences.PublicKey = client.GetPublicKey();
        }

        // https is always accepted; http only when the host is loopback (localhost / 127.0.0.1 /
        // ::1), the test/dev case. Anything else (non-https, non-loopback) is refused.
        private static bool IsSecureOrLoopback(string baseAddress)
        {
            if (!Uri.TryCreate(baseAddress, UriKind.Absolute, out var uri))
                return false;
            if (uri.Scheme == Uri.UriSchemeHttps)
                return true;
            return uri.Scheme == Uri.UriSchemeHttp && uri.IsLoopback;
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
                // Signature invalid and license not fresh from server. Distinguish two cases:
                //   Key rotation: server has a new key that validates the existing file → proceed
                //   Tampering:    even the fresh server key fails → content was modified → throw
                // If the server is unreachable, fall back to re-fetching the full license.
                string previousKey = context.PublisherPreferences.PublicKey;
                try
                {
                    GetPublicKeyFromServer(context);
                    if (IsSigValid(context))
                    {
                        SetNext(new LicenseParsingHandler());
                        return;
                    }
                    // Fresh key still doesn't validate — file was tampered.
                    SetNextError(context, new CryptographicException("Verification failed: Signature was not valid"));
                }
                catch
                {
                    // Can't reach server — fall back to re-fetching the full license.
                    context.PublisherPreferences.PublicKey = previousKey;
                    try { context.LicenseModel = Models.LicenseModel.FromXml(context.SignedLicense); }
                    catch { }
                    SetNext(new ApiGetLicenseHandler());
                }
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