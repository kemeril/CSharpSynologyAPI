using System.Collections.Generic;

namespace KDSVideo.Infrastructure
{
    public interface ITrustedLoginDataHandler
    {
        IList<TrustedLoginData> GetAll();
        TrustedLoginData GetLatest();
        TrustedLoginData Get(string host, string account, string password);
        void AddOrUpdate(TrustedLoginData trustedLoginData);
    }
}