using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KDSVideo.Infrastructure
{
    public class NetworkService : INetworkService
    {
        public IWebProxy GetProxy()
        {
            return GetSystemWebProxy() ?? CreateUserProxy(null, null, null);
        }

        public IWebProxy CreateUserProxy(string proxyUrl, string userName, string password)
        {
            var proxy = string.IsNullOrWhiteSpace(proxyUrl) ? null : new WebProxy(new Uri(proxyUrl));

            if (proxy != null && !string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(password))
            {
                proxy.Credentials = new NetworkCredential(userName, password);
            }

            return proxy;
        }

        public IWebProxy GetSystemWebProxy()
        {
            var proxy = WebRequest.GetSystemWebProxy();
            proxy.Credentials = CredentialCache.DefaultNetworkCredentials;
            return proxy;
        }
    }
}