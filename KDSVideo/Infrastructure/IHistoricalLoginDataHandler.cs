using System.Collections.Generic;

namespace KDSVideo.Infrastructure
{
    public interface IHistoricalLoginDataHandler
    {
        IList<HistoricalLoginData> GetAll();
        HistoricalLoginData GetLatest();
        void AddOrUpdate(string host, string account, string password);
        void Remove(string host, string account);
    }
}