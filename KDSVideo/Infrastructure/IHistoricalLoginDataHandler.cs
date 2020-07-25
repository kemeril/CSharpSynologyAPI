using System.Collections.Generic;

namespace KDSVideo.Infrastructure
{
    public interface IHistoricalLoginDataHandler
    {
        IList<HistoricalLoginData> GetAll();
        HistoricalLoginData GetLatest();
        void AddOrUpdate(HistoricalLoginData historicalLoginData);
    }
}