using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;
using StdUtils;

namespace KDSVideo.Infrastructure
{
    public class TrustedLoginDataHandler : ITrustedLoginDataHandler
    {
        private const string TrustedLoginDataKey = nameof(TrustedLoginDataKey);
        private const int MaxItemStorage = 1000;

        private IList<TrustedLoginData> GetAll()
        {
            var settingValues = ApplicationData.Current.LocalSettings.Values;

            if (!settingValues.TryGetValue(TrustedLoginDataKey, out var listObject)) return null;

            return listObject != null && !string.IsNullOrWhiteSpace((string)listObject)
                ? JsonHelper.FromJson<List<TrustedLoginData>>((string) listObject)
                : new List<TrustedLoginData>();
        }

        public string GetDeviceId(string host, string account, string password)
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

            // Get data
            var all = GetAll();

            return all.FirstOrDefault(it => host.Equals(it.Host, StringComparison.InvariantCultureIgnoreCase)
                                            && account.Equals(it.Account)
                                            && (password ?? string.Empty).Equals(it.Password))?.DeviceId;
        }

        public void AddOrUpdate(string host, string account, string password, string deviceId)
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
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(deviceId));
            }

            // Save data
            var all = GetAll();
            for (var i = 0; i < all.Count; i++)
            {
                if (host.Equals(all[i].Host, StringComparison.InvariantCultureIgnoreCase) && account.Equals(all[i].Account))
                {
                    all.RemoveAt(i);
                    break;
                }
            }
            all.Insert(0, new TrustedLoginData
            {
                Host = host,
                Account = account,
                Password = password ?? string.Empty,
                DeviceId = deviceId
            });
            all = all.Take(MaxItemStorage).ToList();

            var json = JsonHelper.ToJson(all);
            var settingValues = ApplicationData.Current.LocalSettings.Values;
            settingValues[TrustedLoginDataKey] = json;
        }
    }
}