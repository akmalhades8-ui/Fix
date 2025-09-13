using System;
using System.Net;
using System.Net.Http;

namespace RYLAdminApp
{
    public static class ApiClient
    {
        private static readonly HttpClientHandler handler;
        public static readonly HttpClient Client;

        static ApiClient()
        {
            handler = new HttpClientHandler
            {
                CookieContainer = new CookieContainer(),
                UseCookies = true,
                AllowAutoRedirect = true
            };
            Client = new HttpClient(handler)
            {
                BaseAddress = new Uri("http://31.58.143.7/APImailadmin/"),
                Timeout = TimeSpan.FromSeconds(30)
            };
        }
    }
}
