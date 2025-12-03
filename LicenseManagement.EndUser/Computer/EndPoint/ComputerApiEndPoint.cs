using Hymma.Lm.EndUser.Exceptions;
using Hymma.Lm.EndUser.Extensions;
using Hymma.Lm.EndUser.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Hymma.Lm.EndUser
{
    public class ComputerApiEndPoint
    {
        private Uri _endPoint;
        private string _apiKey;

        public ComputerApiEndPoint(string apiKey)
        {
            _endPoint = new Uri(WebApiClient.HttpClient.BaseAddress, "computer");
            _apiKey = apiKey;
        }
        void Validate(string computerInfo)
        {
            if (string.IsNullOrEmpty(computerInfo))
                throw new System.ArgumentNullException("computer info is empty");

            if (computerInfo.Length > 128)
                throw new ArgumentOutOfRangeException("computerInfo cannot be longer than 128 chars");
        }

        /// <summary>
        /// posts a new computer Id to hymma license management online api 
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns><see cref="HttpStatusCode.Created"/> on success or <see cref="HttpStatusCode.Conflict"/> in case computer exists</returns>
        ///<exception cref="ArgumentOutOfRangeException"
        ///<exception cref="ArgumentNullException"
        public HttpStatusCode PostComputer(PostComputerModel model)
        {
            Validate(model.MacAddress);
            Validate(model.Name);
            var json = JsonConvert.SerializeObject(model);
            HttpStatusCode status;

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                //ms.Seek(0, SeekOrigin.Begin);
                using (var request = new AuthorizedRequest(HttpMethod.Post, "computer", _apiKey))
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

        /// <summary>
        /// posts a new computer Id to hymma license management online api 
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns><see cref="HttpStatusCode.Created"/> on success or <see cref="HttpStatusCode.Conflict"/> in case computer exists</returns>
        ///<exception cref="ArgumentOutOfRangeException"
        ///<exception cref="ArgumentNullException"
        public async Task<HttpStatusCode> PostComputerAsync(PostComputerModel model)
        {
            Validate(model.MacAddress);
            Validate(model.Name);
            var json = JsonConvert.SerializeObject(model);
            HttpStatusCode status;
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                //ms.Seek(0, SeekOrigin.Begin);
                using (var request = new AuthorizedRequest(HttpMethod.Post, "computer", _apiKey))
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

        /// <summary>
        /// gets a computer object via it's mac-address from online api
        /// </summary>
        /// <param name="deviceId">unique identifier for a computer </param>
        /// <returns></returns>
        ///<exception cref="ApiException"
        ///<exception cref="ArgumentOutOfRangeException"
        ///<exception cref="ArgumentNullException"
        public async Task<Models.ComputerModel> GetComputerAsync()
        {

            //create uri query
            var endPointWithQuery = _endPoint.WithQueryString(new List<KeyValuePair<string, string>>
            {
             new KeyValuePair<string,string>( "macAddress", ComputerId.Instance.MachineId),
            });
            var computer = new ComputerModel();
            using (var request = new AuthorizedRequest(HttpMethod.Get, endPointWithQuery, _apiKey))
            {
                using (var response = await WebApiClient.HttpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead))
                {
                    response.EnsureSuccessStatusCode();
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        TextReader reader = new StreamReader(stream);
                        computer = JsonConvert.DeserializeObject<ComputerModel>(reader.ReadToEnd());
                    }
                }
            }
            return computer;
        }

        /// <summary>
        /// gets a computer object via it's mac-address from online api
        /// </summary>
        /// <param name="deviceId">unique identifier for a computer </param>
        /// <returns></returns>
        ///<exception cref="ApiException"
        ///<exception cref="ArgumentOutOfRangeException"
        ///<exception cref="ArgumentNullException"
        public ComputerModel GetComputer()
        {
            //make sure not null and not over 128 chars
            //create uri query
            var endPointWithQuery = _endPoint.WithQueryString(new List<KeyValuePair<string, string>>
            {
             new KeyValuePair<string,string>( "macAddress", ComputerId.Instance.MachineId)
            });
            var computer = new ComputerModel();
            using (var request = new AuthorizedRequest(HttpMethod.Get, endPointWithQuery, _apiKey))
            {
                using (var response = WebApiClient.HttpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead).GetAwaiter().GetResult())
                {
                    response.EnsureSuccessStatusCode();
                    using (var stream = response.Content.ReadAsStreamAsync().GetAwaiter().GetResult())
                    {
                        TextReader reader = new StreamReader(stream);
                        computer = JsonConvert.DeserializeObject<ComputerModel>(reader.ReadToEnd());
                    }
                }
            }
            return computer;
        }
    }
}
