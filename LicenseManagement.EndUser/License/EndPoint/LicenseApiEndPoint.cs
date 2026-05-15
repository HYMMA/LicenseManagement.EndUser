using LicenseManagement.EndUser.Extensions;
using LicenseManagement.EndUser.Models;
using LicenseManagement.EndUser.Utilities;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LicenseManagement.EndUser.License.EndPoint
{
    public class LicenseApiEndPoint
    {
        private const string Path = "license";
        private static readonly HttpMethod Patch = new HttpMethod("PATCH");

        private readonly string _apiKey;

        public LicenseApiEndPoint(string apiKey)
        {
            _apiKey = apiKey;
        }

        public HttpStatusCode PostLicense(PostLicenseModel model)
            => ApiHttp.SendForStatus(HttpMethod.Post, Path, _apiKey, model, IdempotencyKeyForPost(model));

        public Task<HttpStatusCode> PostLicenseAsync(PostLicenseModel model, CancellationToken cancellationToken = default(CancellationToken))
            => ApiHttp.SendForStatusAsync(HttpMethod.Post, Path, _apiKey, model, IdempotencyKeyForPost(model), cancellationToken);

        public HttpStatusCode PatchLicense(PatchLicenseModel model)
            => ApiHttp.SendForStatus(Patch, Path, _apiKey, model);

        public Task<HttpStatusCode> PatchLicenseAsync(PatchLicenseModel model, CancellationToken cancellationToken = default(CancellationToken))
            => ApiHttp.SendForStatusAsync(Patch, Path, _apiKey, model, cancellationToken: cancellationToken);

        public string GetLicense(string computer, string product, List<string> features, uint validDays)
            => ApiHttp.GetString(BuildGetPath(computer, product, features, validDays), _apiKey);

        public Task<string> GetLicenseAsync(string computer, string product, List<string> features, uint validDays, CancellationToken cancellationToken = default(CancellationToken))
            => ApiHttp.GetStringAsync(BuildGetPath(computer, product, features, validDays), _apiKey, cancellationToken);

        private static string BuildGetPath(string computer, string product, List<string> features, uint validDays)
        {
            var endPoint = new Uri(WebApiClient.HttpClient.BaseAddress, Path);
            var collection = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("computer", computer),
                new KeyValuePair<string, string>("product", product),
                new KeyValuePair<string, string>("validDays", validDays.ToString()),
            };
            if (features != null && features.Count != 0)
            {
                foreach (var feature in features)
                    collection.Add(new KeyValuePair<string, string>("features", feature));
            }
            return endPoint.WithQueryString(collection);
        }

        // Stable per (computer, product) pair so retries after a network blip don't create duplicate license rows.
        private static string IdempotencyKeyForPost(PostLicenseModel model)
            => $"license:{model.Computer}:{model.Product}";
    }
}
