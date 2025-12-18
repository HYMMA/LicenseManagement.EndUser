using LicenseManagement.EndUser.Exceptions;
using LicenseManagement.EndUser.Extensions;
using LicenseManagement.EndUser.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace LicenseManagement.EndUser.Product.EndPoint
{
    /// <summary>
    /// manages api calls related to <see cref="Models.ProductModel"/>
    /// </summary>
    public class ProductApiEndPoint
    {
        private string apiKey;

        public ProductApiEndPoint(string apiKey)
        {
            this.apiKey = apiKey;
        }
        private string GetQueryString(string id)
        {
            ValidateProductName(id);
            var endPoint = new Uri(WebApiClient.HttpClient.BaseAddress, "product");

            var endPointWithQuery = endPoint.WithQueryString(new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("id",id)
            });
            return endPointWithQuery;
        }

        /// <summary>
        /// gets a product form db online
        /// </summary>
        /// <param name="id">product name which cannot be more than 250 chars</param>
        /// <returns><see cref="Models.ProductModel"/> or <see cref="ApiException"/> if any error calling the api</returns>
        ///<exception cref="ArgumentOutOfRangeException"></exception>
        ///<exception cref="ArgumentNullException"></exception>
        ///<exception cref="ApiException"></exception>
        public ProductModel GetProduct(string id)
        {
            ProductModel result = null;
            using (var request = new AuthorizedRequest(HttpMethod.Get, GetQueryString(id), apiKey))
            {
                using (var response = WebApiClient.HttpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead).GetAwaiter().GetResult())
                {
                    response.EnsureSuccessStatusCode();
                    using (var stream = response.Content.ReadAsStreamAsync().GetAwaiter().GetResult())
                    {
                        TextReader reader = new StreamReader(stream);
                        result = JsonConvert.DeserializeObject<ProductModel>(reader.ReadToEnd());
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// gets a product form db online
        /// </summary>
        /// <param name="id">product name which cannot be more than 250 chars</param>
        /// <returns><see cref="Models.ProductModel"/> or <see cref="ApiException"/> if any error calling the api</returns>
        ///<exception cref="ArgumentOutOfRangeException"></exception>
        ///<exception cref="ArgumentNullException"></exception>
        ///<exception cref="ApiException"></exception>
        public async Task<ProductModel> GetProductAsync(string id)
        {
            ProductModel result = null;
            using (var request = new AuthorizedRequest(HttpMethod.Get, GetQueryString(id), apiKey))
            {
                using (var response =await WebApiClient.HttpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead))
                {
                    response.EnsureSuccessStatusCode();
                    using (var stream =await response.Content.ReadAsStreamAsync())
                    {
                        TextReader reader = new StreamReader(stream);
                        result = JsonConvert.DeserializeObject<ProductModel>(reader.ReadToEnd());
                    }
                }
            }
            return result;
        }
        private void ValidateProductName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name of product cannot be empty");
            }
            if (name.Length > 250)
            {
                throw new ArgumentOutOfRangeException("name",
                                                      name,
                                                      "product name cannot be larger than 250 characters");
            }
        }
    }
}
