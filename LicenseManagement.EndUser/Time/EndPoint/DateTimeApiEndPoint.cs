using LicenseManagement.EndUser.Utilities;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LicenseManagement.EndUser.Time.EndPoint
{
    public class DateTimeApiEndPoint
    {
        private const string Path = "DateTime";
        private readonly string _apiKey;

        public DateTimeApiEndPoint(string apiKey)
        {
            _apiKey = apiKey;
        }

        /// <summary>Gets current UTC time from server.</summary>
        public DateTime GetCurrentUtcTime()
            => ApiHttp.SendJson<DateTime>(HttpMethod.Get, Path, _apiKey);

        public Task<DateTime> GetCurrentUtcTimeAsync(CancellationToken cancellationToken = default(CancellationToken))
            => ApiHttp.SendJsonAsync<DateTime>(HttpMethod.Get, Path, _apiKey, cancellationToken: cancellationToken);
    }
}
