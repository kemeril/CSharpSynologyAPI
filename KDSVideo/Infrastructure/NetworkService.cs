using System;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;

namespace KDSVideo.Infrastructure
{
    public class NetworkService : INetworkService
    {
        private const string QUICK_CONNECT_TO = "QuickConnect.to";

        public string? GetComputerName()
        {
            try
            {
                return Environment.MachineName;
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public Uri? GetHostUri(string host)
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                return null;
            }

            try
            {
                var isQuickConnectId = IsQuickConnectId(host);
                if (isQuickConnectId)
                {
                    _ = ConvertQuickConnectIdToHost(host);
                    throw new QuickConnectLoginNotSupportedException();
                }

                host = CheckHostScheme(host);

                var uriBuilder = new UriBuilder(host);

                //TODO: Implement QuickConnectId based connection! Remark: the Synology relay server may redirect the request.
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (!isQuickConnectId && !DoesHostContainsPortNumber(host, uriBuilder.Uri.HostNameType))
                {
                    uriBuilder.Port = 5000; //Synology default port number
                }

                var uri = uriBuilder.Uri;

                return uri;
            }
            catch (Exception e)
            {
                Trace.TraceInformation(e.ToString());
                throw;
            }
        }

        /// <summary>
        /// A custom QuickConnect ID can include letters, numbers, and hyphens ("-"), and must start with a letter.
        /// </summary>
        /// <param name="host"></param>
        /// <returns><c>true</c> if the host is a QuickConnect Id; otherwise <c>false</c>.</returns>
        private static bool IsQuickConnectId(string host)
        {
            const string PATTERN = @"^[a-zA-Z][a-zA-Z0-9\-]*$";
            var regex = new Regex(PATTERN);
            return regex.IsMatch(host);
        }

        private static string ConvertQuickConnectIdToHost(string quickConnectId) => $"http://{QUICK_CONNECT_TO}/{quickConnectId}/";

        private static string CheckHostScheme(string host)
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
        /// <returns><c>true</c> if the host already contains port number; otherwise <c>false</c>.</returns>
        private static bool DoesHostContainsPortNumber(string host, UriHostNameType uriHostNameType)
        {
            switch (uriHostNameType)
            {
                case UriHostNameType.Unknown:
                case UriHostNameType.Basic:
                    throw new NotSupportedException("Unknown host.");
                case UriHostNameType.Dns:
                case UriHostNameType.IPv4:
                    {
                        const string PATTERN = @":\d";
                        var regex = new Regex(PATTERN);
                        return regex.IsMatch(host);
                    }
                case UriHostNameType.IPv6:
                    {
                        const string PATTERN = @"]:\d";
                        var regex = new Regex(PATTERN);
                        return regex.IsMatch(host);
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public IWebProxy GetProxy() => GetSystemWebProxy();

        public IWebProxy? CreateUserProxy(string proxyUrl, string userName, string password)
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
