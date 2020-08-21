using System.Collections.Generic;

namespace KDSVideo.Infrastructure
{
    public interface IHistoricalLoginDataHandler
    {
        IList<HistoricalLoginData> GetAll();
        void AddOrUpdate(string host, string account, string password);
    }
}