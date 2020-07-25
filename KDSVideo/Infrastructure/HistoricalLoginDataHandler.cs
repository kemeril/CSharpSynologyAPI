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

        public HistoricalLoginData Get(string host, string account, string password)
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

            // Get data
            var all = GetAll();
            return all.FirstOrDefault(it => host.Equals(it.Host, StringComparison.InvariantCultureIgnoreCase)
                                            && account.Equals(it.Account)
                                            && password.Equals(it.Password));
        }

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
                if (historicalLoginData.Host.Equals(all[i].Host, StringComparison.InvariantCultureIgnoreCase) && historicalLoginData.Account.Equals(all[i].Account))
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