using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LicenseManagement.EndUser.Utilities
{
    internal static class HttpResponseMessageExtensions
    {
        /// <summary>
        /// throws an exception if the status code is 204 (NoContent)
        /// </summary>
        /// <param name="message"></param>
        ///<param name="errorMsg">the error message to use with exception</param>
        /// <returns><see cref="HttpRequestException"/> if status code is 204. Otherwise the <see cref="HttpRequestMessage"/></returns>
        /// <exception cref="HttpRequestException"></exception>
        ///<remarks>while 204 is essentially a success status code, this method treats it as exception</remarks>
        public static HttpResponseMessage EnsureContentIsNotEmpty(this HttpResponseMessage message,string errorMsg)
        {
            if (message.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                throw new HttpRequestException("The requested data does not exist on the server.");
            }
            else
            {
                return message;
            }
        }
    }
}
