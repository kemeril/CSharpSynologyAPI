﻿using System.Net;

namespace KDSVideo.Infrastructure
{
    public interface INetworkService
    {
        IWebProxy GetProxy();
        IWebProxy CreateUserProxy(string proxyUrl, string userName, string password);
        IWebProxy GetSystemWebProxy();
    }
}