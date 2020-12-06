using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;
using StdUtils;

namespace KDSVideo.Infrastructure
{
    public class HistoricalLoginDataHandler : IHistoricalLoginDataHandler
    {
        private const string HistoricalLoginDataKey = nameof(HistoricalLoginDataKey);
        private const int MaxItemStorage = 20;

        public IList<HistoricalLoginData> GetAll()
        {
            var settingValues = ApplicationData.Current.LocalSettings.Values;

            if (!settingValues.TryGetValue(HistoricalLoginDataKey, out var listObject))
            {
                return new List<HistoricalLoginData>();
            }

            return listObject != null && !string.IsNullOrWhiteSpace((string)listObject)
                ? JsonHelper.FromJson<List<HistoricalLoginData>>((string)listObject) ?? new List<HistoricalLoginData>()
                : new List<HistoricalLoginData>();
        }

        public void AddOrUpdate(string host, string account, string password)
        {
            // Check for mandatory parameters
            if (string.IsNullOrWhiteSpace(host))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(host));
            }
            if (string.IsNullOrWhiteSpace(account))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(account));
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(password));
            }

            // Save data
            var all = GetAll();
            for (var i = 0; i < all.Count; i++)
            {
                var historicalLoginData = all[i];
                if (host.Equals(historicalLoginData.Host, StringComparison.InvariantCultureIgnoreCase) && account.Equals(historicalLoginData.Account, StringComparison.InvariantCultureIgnoreCase))
                {
                    all.RemoveAt(i);
                    break;
                }
            }
            all.Insert(0, new HistoricalLoginData
            {
                Host = host,
                Account = account,
                Password = password
            });
            all = all.Take(MaxItemStorage).ToList();

            var json = JsonHelper.ToJson(all);
            var settingValues = ApplicationData.Current.LocalSettings.Values;
            settingValues[HistoricalLoginDataKey] = json;
        }
    }
}