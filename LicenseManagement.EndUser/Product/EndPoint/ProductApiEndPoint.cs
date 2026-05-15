using LicenseManagement.EndUser.Extensions;
using LicenseManagement.EndUser.Models;
using LicenseManagement.EndUser.Utilities;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LicenseManagement.EndUser.Product.EndPoint
{
    /// <summary>
    /// Wraps the /product endpoint.
    /// </summary>
    internal class ProductApiEndPoint
    {
        private const string Path = "product";

        private readonly string _apiKey;

        internal ProductApiEndPoint(string apiKey)
        {
            _apiKey = apiKey;
        }

        internal ProductModel GetProduct(string id)
            => ApiHttp.SendJson<ProductModel>(HttpMethod.Get, BuildGetPath(id), _apiKey);

        internal Task<ProductModel> GetProductAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
            => ApiHttp.SendJsonAsync<ProductModel>(HttpMethod.Get, BuildGetPath(id), _apiKey, cancellationToken: cancellationToken);

        private static string BuildGetPath(string id)
        {
            ValidateProductName(id);
            var endPoint = new Uri(WebApiClient.HttpClient.BaseAddress, Path);
            return endPoint.WithQueryString(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("id", id)
            });
        }

        private static void ValidateProductName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name), "name of product cannot be empty");

            if (name.Length > 250)
                throw new ArgumentOutOfRangeException(nameof(name), name, "product name cannot be larger than 250 characters");
        }
    }
}
