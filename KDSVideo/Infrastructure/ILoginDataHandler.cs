using System.Collections.Generic;

namespace KDSVideo.Infrastructure
{
    public interface ILoginDataHandler
    {
        IList<LoginData> GetAll();
        LoginData GetLast();
        void AddOrUpdate(LoginData loginData);
    }
}