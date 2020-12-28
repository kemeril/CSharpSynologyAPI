using System;
using System.Net;

namespace KDSVideo.Infrastructure
{
    public interface INetworkService
    {
        string GetComputerName();
        Uri GetHostUri(string host);
        IWebProxy GetProxy();
        IWebProxy CreateUserProxy(string proxyUrl, string userName, string password);
        IWebProxy GetSystemWebProxy();
    }
}