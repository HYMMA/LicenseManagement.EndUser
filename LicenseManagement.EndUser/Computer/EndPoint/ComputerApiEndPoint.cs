using LicenseManagement.EndUser.Exceptions;
using LicenseManagement.EndUser.Extensions;
using LicenseManagement.EndUser.Models;
using LicenseManagement.EndUser.Utilities;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LicenseManagement.EndUser
{
    internal class ComputerApiEndPoint
    {
        private const string Path = "computer";
        private readonly string _apiKey;

        internal ComputerApiEndPoint(string apiKey)
        {
            _apiKey = apiKey;
        }

        private static void Validate(string computerInfo, string paramName)
        {
            if (string.IsNullOrEmpty(computerInfo))
                throw new ArgumentNullException(paramName, "computer info is empty");

            if (computerInfo.Length > 128)
                throw new ArgumentOutOfRangeException(paramName, "computerInfo cannot be longer than 128 chars");
        }

        /// <summary>
        /// Posts a new computer to the license server.
        /// </summary>
        /// <returns><see cref="HttpStatusCode.Created"/> on success or <see cref="HttpStatusCode.Conflict"/> if the computer already exists.</returns>
        internal HttpStatusCode PostComputer(PostComputerModel model)
        {
            Validate(model.MacAddress, nameof(model.MacAddress));
            Validate(model.Name, nameof(model.Name));
            return ApiHttp.SendForStatus(HttpMethod.Post, Path, _apiKey, model, IdempotencyKey(model));
        }

        internal Task<HttpStatusCode> PostComputerAsync(PostComputerModel model, CancellationToken cancellationToken = default(CancellationToken))
        {
            Validate(model.MacAddress, nameof(model.MacAddress));
            Validate(model.Name, nameof(model.Name));
            return ApiHttp.SendForStatusAsync(HttpMethod.Post, Path, _apiKey, model, IdempotencyKey(model), cancellationToken);
        }

        internal ComputerModel GetComputer()
        {
            return ApiHttp.SendJson<ComputerModel>(HttpMethod.Get, BuildGetPath(), _apiKey);
        }

        internal Task<ComputerModel> GetComputerAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return ApiHttp.SendJsonAsync<ComputerModel>(HttpMethod.Get, BuildGetPath(), _apiKey, cancellationToken: cancellationToken);
        }

        private static string BuildGetPath()
        {
            var endPoint = new Uri(WebApiClient.HttpClient.BaseAddress, Path);
            return endPoint.WithQueryString(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("macAddress", ComputerId.Instance.MachineId),
            });
        }

        // Stable per (mac, name) pair so retries after a network blip don't create duplicate computer rows.
        // The webapp caches the response for 24h keyed off this header.
        private static string IdempotencyKey(PostComputerModel model)
            => $"computer:{model.MacAddress}:{model.Name}";
    }
}
