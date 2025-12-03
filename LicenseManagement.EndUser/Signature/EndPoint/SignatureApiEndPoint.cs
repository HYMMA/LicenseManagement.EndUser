using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Hymma.Lm.EndUser.Signature.EndPoint
{
    /// <summary>
    /// api endpoints helper class to deal with public key and signatures
    /// </summary>
    internal class SignatureApiEndPoint
    {
        private string apiKey;

        internal SignatureApiEndPoint(string apiKey)
        {
            this.apiKey = apiKey;
        }

        /// <summary>
        /// get the public key of the RSA key used to sign a license file
        /// </summary>
        /// <returns></returns>
        internal string GetPublicKey()
        {
            string result = string.Empty;

            //ms.Seek(0, SeekOrigin.Begin);
            using (var request = new AuthorizedRequest(HttpMethod.Get, "signingKeys/.xml", apiKey))
            {
                using (var response = WebApiClient.HttpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead).GetAwaiter().GetResult())
                {
                    response.EnsureSuccessStatusCode();
                    using (var stream = response.Content.ReadAsStreamAsync().GetAwaiter().GetResult())
                    {
                        TextReader reader = new StreamReader(stream);
                        result = reader.ReadToEnd();
                    }
                }
            }
            return result;
        }

        internal async Task<string> GetPublicKeyAsync()
        {
            string result = string.Empty;

            //ms.Seek(0, SeekOrigin.Begin);
            using (var request = new AuthorizedRequest(HttpMethod.Get, "signingKeys/.xml", apiKey))
            {
                using (var response = await WebApiClient.HttpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead))
                {
                    response.EnsureSuccessStatusCode();
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        TextReader reader = new StreamReader(stream);
                        result = reader.ReadToEnd();
                    }
                }
            }
            return result;
        }
    }
}
