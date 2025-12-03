using Hymma.Lm.EndUser.License.Handlers;
using Hymma.Lm.EndUser.Signature.EndPoint;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using System.Xml;

namespace Hymma.Lm.EndUser.Signature.Handlers
{
    internal class LicenseSignatureValidationHandler : LicenseValidationHandler
    {
        //private string publicKey;

        /// <summary>
        /// validates signature of the xml documents and sets the next handler to <see cref="LicenseParsingHandler"/>
        /// </summary>


        public LicenseSignatureValidationHandler()
        {
        }


        bool IsSigValid(LicHandlingContext context)
        {
            if (string.IsNullOrEmpty(context.PublisherPreferences.PublicKey))
                return false;   
            
            XmlDocument xmlDocument = new XmlDocument() { PreserveWhitespace = true };
            xmlDocument.LoadXml(context.SignedLicense);

            // Create a new SignedXml object and pass it
            // the XML document class.
            SignedXml signedXml = new SignedXml(xmlDocument);

            // Find the "Signature" node and create a new
            // XmlNodeList object.
            XmlNodeList signatures = xmlDocument
                .GetElementsByTagName("Signature");

            // Throw an exception if no signature was found.
            // Throw an exception
            // if more than one signature was found.
            if (signatures.Count != 1)
                return false;

            // Load the first <signature> node.
            signedXml.LoadXml((XmlElement)signatures[0]);

            // Check the signature and return the result.
            //return signedXml.CheckSignature(GetKey());
            bool sigIsValid = false;
            using (var rsa = RSA.Create())
            {
                rsa.FromXmlString(context.PublisherPreferences.PublicKey);
                sigIsValid = signedXml.CheckSignature(rsa);
            }
            return sigIsValid;
        }

        void GetPublicKeyFromServer(LicHandlingContext context)
        {
            var client = new SignatureApiEndPoint(context.PublisherPreferences.ApiKey);
            context.PublisherPreferences.PublicKey = client.GetPublicKey();
        }
        void SetNextHandler(LicHandlingContext context)
        {
            //getting pubkey from local takes precedence over server one
            //becaues it is much faster, as this line of code gets called when an app starts,
            //Problem: if developer reset their keys on website their apps will fail but it is their responsiblity to 
            //upgrade their apps with new public keys
            if (IsSigValid(context))
            {
                SetNext(new LicenseParsingHandler());
            }
            else
            {
                if (context.IsLicenseFreshOutOfServer)
                {
                    try
                    {
                        GetPublicKeyFromServer(context);
                    }
                    catch (System.Exception)
                    {
                        SetNextError(context, new CryptographicException("Could not fetch public key."));
                    }

                    //double check with freh public key from server
                    if (IsSigValid(context))
                    {
                        SetNext(new LicenseParsingHandler());
                    }
                    else
                    {
                        SetNextError(context, new CryptographicException("Verification failed: Signature was not valid"));
                    }
                }
                else
                {
                    //when failed maybe the public key had been renewed on website so get a new license from server
                    SetNext(new ApiGetLicenseHandler());
                }
            }
        }

        public override async Task HandleContextAsync(LicHandlingContext context)
        {
            SetNextHandler(context);
            await nextHandler
                .HandleContextAsync(context)
                .ConfigureAwait(false);
        }

        public override void HandleContext(LicHandlingContext context)
        {
            SetNextHandler(context);
            nextHandler.HandleContext(context);
        }
    }
}