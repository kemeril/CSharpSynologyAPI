using SynologyAPI;
using System;
using System.Net;

namespace VideoStationTest2
{
    public static class VideoStationFactory
    {
        private static WebProxy CreateProxy(string proxyUrl)
        {
            return String.IsNullOrWhiteSpace(proxyUrl) ? null : new WebProxy(new Uri(proxyUrl));
        }

        public static VideoStation CreateVideoStation()
        {
            return new VideoStation(new Uri("http://<your-subdomain-here>.duckdns.org/"), "video.station.dev", "C1E908Vw18u474p99tsrFNqo6kEj7c", CreateProxy(""));
        }
    }
}