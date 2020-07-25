using System.Collections.Generic;

namespace KDSVideo.Infrastructure
{
    public interface IHistoricalLoginDataHandler
    {
        IList<HistoricalLoginData> GetAll();
        HistoricalLoginData GetLatest();
        HistoricalLoginData Get(string host, string account, string password);
        void AddOrUpdate(HistoricalLoginData historicalLoginData);
    }
}