using LicenseManagement.EndUser.Exceptions;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace LicenseManagement.EndUser.Time.EndPoint
{
    public class DateTimeApiEndPoint  
    {
        private const string DateTime = "DateTime";
        private string _apiKey;

        public DateTimeApiEndPoint(string apiKey)
        {
            _apiKey = apiKey;
        }

        /// <summary>
        /// get current time from server
        /// </summary>
        /// <returns><see cref="DateTime.UtcNow"/> if there was internet connection or could parse server response</returns>
        /// <exception cref="ApiException"></exception>
        public DateTime GetCurrentUtcTime()
        {

            DateTime time;
            using (var request = new AuthorizedRequest(HttpMethod.Get, DateTime,_apiKey))
            {
                using (var response = WebApiClient.HttpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead).GetAwaiter().GetResult())
                {
                    response.EnsureSuccessStatusCode();
                    using (var stream = response.Content.ReadAsStreamAsync().GetAwaiter().GetResult())
                    {
                        TextReader reader = new StreamReader(stream);
                        time = JsonConvert.DeserializeObject<DateTime>(reader.ReadToEnd());

                    }
                }
            }
            return time;
        }

        /// <summary>
        /// get current time from server
        /// </summary>
        /// <returns><see cref="DateTime.UtcNow"/> if there was internet connection or could parse server response</returns>
        /// <exception cref="ApiException"></exception>
        public async Task<DateTime> GetCurrentUtcTimeAsync()
        {

            DateTime time;
            using (var request = new AuthorizedRequest(HttpMethod.Get, DateTime,_apiKey))
            {
                using (var response = await WebApiClient.HttpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead))
                {
                    response.EnsureSuccessStatusCode();
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        TextReader reader = new StreamReader(stream);
                        time = JsonConvert.DeserializeObject<DateTime>(reader.ReadToEnd());
                    }
                }
            }
            return time;
        }
    }
}
