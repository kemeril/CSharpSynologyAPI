using System;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;

namespace KDSVideo.Infrastructure
{
    public class NetworkService : INetworkService
    {
        public Uri GetHostUri(string host)
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                return null;
            }
          
            try
            {
                if (IsQuickConnectId(host))
                {
                    host = ConvertQuickConnectIdToHost(host);
                }

                host = CheckHostScheme(host);

                var uriBuilder = new UriBuilder(host);
                if (!DoesHostContainsPortNumber(host, uriBuilder.Uri.HostNameType))
                {
                    uriBuilder.Port = 5000; //Synology default port number
                }
                
                var uri = uriBuilder.Uri;
                
                return uri;
            }
            catch (Exception e)
            {
                Trace.TraceInformation(e.ToString());
                return null;
            }
        }

        /// <summary>
        /// A custom QuickConnect ID can include letters, numbers, and hyphens ("-"), and must start with a letter.
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        private bool IsQuickConnectId(string host)
        {
            const string pattern = @"^[a-zA-Z][a-zA-Z0-9\-]*$";
            var regex = new Regex(pattern);
            return regex.IsMatch(host ?? string.Empty);
        }

        private string ConvertQuickConnectIdToHost(string quickConnectId)
        {
            return "http://QuickConnect.to/" + quickConnectId;
        }

        private string CheckHostScheme(string host)
        {
            if (!host.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase)
                && !host.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase))
            {
                host = "http://" + host;
            }

            return host;
        }

        /// <summary>
        /// Checks if does the IPV4 or IPV6 based url (host) contain port number.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="uriHostNameType"></param>
        /// <returns></returns>
        private bool DoesHostContainsPortNumber(string host, UriHostNameType uriHostNameType)
        {
            switch (uriHostNameType)
            {
                case UriHostNameType.Unknown:
                case UriHostNameType.Basic:
                case UriHostNameType.Dns:
                    throw new NotSupportedException();
                case UriHostNameType.IPv4:
                {
                    const string pattern = @":\d";
                    var regex = new Regex(pattern);
                    return regex.IsMatch(host ?? string.Empty);
                }
                case UriHostNameType.IPv6:
                {
                    const string pattern = @"]:\d";
                    var regex = new Regex(pattern);
                    return regex.IsMatch(host ?? string.Empty);
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

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