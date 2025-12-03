using Hymma.Lm.EndUser.Exceptions;
using Hymma.Lm.EndUser.Extensions;
using Hymma.Lm.EndUser.Models;
using Hymma.Lm.EndUser.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Hymma.Lm.EndUser.License.EndPoint
{
    public class LicenseApiEndPoint
    {
        private readonly Uri _endPoint;
        private string _apiKey;

        public LicenseApiEndPoint(string apiKey)
        {
            _endPoint = new Uri(WebApiClient.HttpClient.BaseAddress, "license");
            _apiKey = apiKey;
        }
        public HttpStatusCode PostLicense(PostLicenseModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            HttpStatusCode status;
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                using (var request = new AuthorizedRequest(HttpMethod.Post, "license", _apiKey))
                {
                    using (var requestContent = new StreamContent(ms))
                    {
                        requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        request.Content = requestContent;

                        using (var response = WebApiClient.HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).GetAwaiter().GetResult())
                        {
                            status = response.StatusCode;
                        }
                    }
                }
            }
            return status;
        }

        public async Task<HttpStatusCode> PostLicenseAsync(PostLicenseModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            HttpStatusCode status;
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                using (var request = new AuthorizedRequest(HttpMethod.Post, "license", _apiKey))
                {
                    using (var requestContent = new StreamContent(ms))
                    {
                        requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        request.Content = requestContent;

                        using (var response = await WebApiClient.HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
                        {
                            status = response.StatusCode;
                        }
                    }
                }
            }
            return status;
        }

        public async Task<HttpStatusCode> PatchLicenseAsync(PatchLicenseModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            HttpStatusCode status;
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                using (var request = new AuthorizedRequest(new HttpMethod("patch"), "license", _apiKey))
                {
                    //request.Headers.Accept.Add(new MediaTypeHeaderValue("application/json"));
                    using (var requestContent = new StreamContent(ms))
                    {
                        requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        request.Content = requestContent;
                        //request.Headers.Add("X-API-KEY", LicHandlingContext.Instance.PublisherPreferences.ApiKey);

                        using (var response = await WebApiClient.HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
                        {
                            status = response.StatusCode;
                        }
                    }
                }
            }
            return status;
        }

        public HttpStatusCode PatchLicense(PatchLicenseModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            HttpStatusCode status;
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                using (var request = new AuthorizedRequest(new HttpMethod("patch"), "license", _apiKey))
                {
                    //request.Headers.Accept.Add(new MediaTypeHeaderValue("application/json"));
                    using (var requestContent = new StreamContent(ms))
                    {
                        requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        request.Content = requestContent;
                        //request.Headers.Add("X-API-KEY", LicHandlingContext.Instance.PublisherPreferences.ApiKey);

                        using (var response = WebApiClient.HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).GetAwaiter().GetResult())
                        {
                            status = response.StatusCode;
                        }
                    }
                }
            }
            return status;
        }

        public async Task<string> GetLicenseAsync(string computer, string product, List<string> features, uint validDays)
        {

            string result = string.Empty;

            //ms.Seek(0, SeekOrigin.Begin);
            using (var request = new AuthorizedRequest(HttpMethod.Get, GetQueryForGetReq(computer, product, features, validDays), _apiKey))
            {
                using (var response = await WebApiClient.HttpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead))
                {
                    response.EnsureSuccessStatusCode();
                    //since not content is a success status code
                    response.EnsureContentIsNotEmpty($"No license found for computer {computer} and product {product}");
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        TextReader reader = new StreamReader(stream);
                        result = reader.ReadToEnd();
                    }
                }
            }
            return result;
        }
     
        private string GetQueryForGetReq(string computer, string product, List<string> features, uint validDays)
        {
            var collection = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string,string>("computer",computer),
                new KeyValuePair<string,string>("product",product),
                new KeyValuePair<string,string>("validDays",validDays.ToString()),
            };
            if (features != null && features.Count != 0)
            {
                foreach (var feature in features)
                {
                    collection.Add(new KeyValuePair<string, string>("features", feature));
                }
            }

            return _endPoint.WithQueryString(collection);
        }

        public string GetLicense(string computer, string product, List<string> features, uint validDays)
        {
            string result = string.Empty;

            //ms.Seek(0, SeekOrigin.Begin);
            using (var request = new AuthorizedRequest(HttpMethod.Get, GetQueryForGetReq(computer, product, features, validDays), _apiKey))
            {
                using (var response = WebApiClient.HttpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead).GetAwaiter().GetResult())
                {
                    response.EnsureSuccessStatusCode();

                    //since not content is a success status code
                    response.EnsureContentIsNotEmpty($"No license found for computer {computer} and product {product}");

                    using (var stream = response.Content.ReadAsStreamAsync().GetAwaiter().GetResult())
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
