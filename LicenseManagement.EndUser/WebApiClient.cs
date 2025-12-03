using HttpClientFactory.Impl;
using System;
using System.Net.Http;

namespace Hymma.Lm.EndUser
{
    internal static class WebApiClient
    {
        internal static HttpClient HttpClient
        {
            get
            {

                var client = new PerHostHttpClientFactory().GetHttpClient(Constants.BaseAddress);
                client.BaseAddress = new Uri(Constants.BaseAddress);
                return client;
//#if DEBUG
//                var client = new PerHostHttpClientFactory().GetHttpClient("http://localhost:7298/api/");
//                client.BaseAddress = new Uri("http://localhost:7298/api/");
//                return client;
//#else
//                var client = new PerHostHttpClientFactory().GetHttpClient(Constants.BaseAddress);
//                client.BaseAddress = new Uri(Constants.BaseAddress);
//                return client;
//#endif
            }
        }
    }

}
