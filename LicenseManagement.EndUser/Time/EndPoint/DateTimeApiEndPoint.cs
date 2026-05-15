using LicenseManagement.EndUser.Utilities;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LicenseManagement.EndUser.Time.EndPoint
{
    internal class DateTimeApiEndPoint
    {
        private const string Path = "DateTime";
        private readonly string _apiKey;

        internal DateTimeApiEndPoint(string apiKey)
        {
            _apiKey = apiKey;
        }

        /// <summary>Gets current UTC time from server.</summary>
        internal DateTime GetCurrentUtcTime()
            => ApiHttp.SendJson<DateTime>(HttpMethod.Get, Path, _apiKey);

        internal Task<DateTime> GetCurrentUtcTimeAsync(CancellationToken cancellationToken = default(CancellationToken))
            => ApiHttp.SendJsonAsync<DateTime>(HttpMethod.Get, Path, _apiKey, cancellationToken: cancellationToken);
    }
}
