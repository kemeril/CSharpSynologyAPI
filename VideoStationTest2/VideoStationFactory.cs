using SynologyAPI;
using System;
using System.Net;

namespace VideoStationTest2
{
    public static class VideoStationFactory
    {
        private static IWebProxy CreateProxy(string proxyUrl)
        {
            return string.IsNullOrWhiteSpace(proxyUrl) ? null : new WebProxy(new Uri(proxyUrl));
        }

        private static IWebProxy GetDefaultProxy()
        {
            var proxy = WebRequest.GetSystemWebProxy();
            proxy.Credentials = CredentialCache.DefaultNetworkCredentials;
            return proxy;
        }

        public static VideoStation CreateVideoStation()
        {
            return new VideoStation(new Uri("http://<your-subdomain-here>.duckdns.org/"), GetDefaultProxy() ?? CreateProxy(""));
        }
    }
}