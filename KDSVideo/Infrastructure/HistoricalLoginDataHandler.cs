using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;
using StdUtils;

namespace KDSVideo.Infrastructure
{
    public class HistoricalLoginDataHandler : IHistoricalLoginDataHandler
    {
        private const string LoginDataKey = nameof(LoginDataKey);

        public IList<HistoricalLoginData> GetAll()
        {
            var settingValues = ApplicationData.Current.LocalSettings.Values;
            return settingValues.TryGetValue(LoginDataKey, out var listObject)
                ? JsonHelper.FromJson<List<HistoricalLoginData>>((string) listObject)
                : new List<HistoricalLoginData>();
        }

        public HistoricalLoginData GetLatest() => GetAll().FirstOrDefault();

        public void AddOrUpdate(HistoricalLoginData historicalLoginData)
        {
            // Check for mandatory parameters
            if (historicalLoginData == null)
            {
                throw new ArgumentNullException(nameof(historicalLoginData));
            }
            if (string.IsNullOrWhiteSpace(historicalLoginData.Host))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(historicalLoginData.Host));
            }
            if (string.IsNullOrWhiteSpace(historicalLoginData.Account))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(historicalLoginData.Account));
            }

            // Fix optional parameters
            historicalLoginData = historicalLoginData.Clone();
            if (historicalLoginData.Password == null)
            {
                historicalLoginData.Password = string.Empty;
            }
            if (historicalLoginData.DeviceId == null)
            {
                historicalLoginData.DeviceId = string.Empty;
            }

            // Save data
            var all = GetAll();
            for (var i = 0; i < all.Count; i++)
            {
                if (all[i].Host == historicalLoginData.Host && all[i].Account == historicalLoginData.Account)
                {
                    all.RemoveAt(i);
                    break;
                }
            }
            all.Insert(0, historicalLoginData);
            all = all.Take(20).ToList();

            var json = JsonHelper.ToJson(all);
            var settingValues = ApplicationData.Current.LocalSettings.Values;
            settingValues[LoginDataKey] = json;
        }
    }
}