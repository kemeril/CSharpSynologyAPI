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

        public IList<TrustedLoginData> GetAll()
        {
            var settingValues = ApplicationData.Current.LocalSettings.Values;
            return settingValues.TryGetValue(TrustedLoginDataKey, out var listObject)
                ? JsonHelper.FromJson<List<TrustedLoginData>>((string) listObject)
                : new List<TrustedLoginData>();
        }

        public TrustedLoginData GetLatest() => GetAll().FirstOrDefault();

        public TrustedLoginData Get(string host, string account, string password)
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

        public void AddOrUpdate(TrustedLoginData trustedLoginData)
        {
            // Check for mandatory parameters
            if (trustedLoginData == null)
            {
                throw new ArgumentNullException(nameof(trustedLoginData));
            }
            if (string.IsNullOrWhiteSpace(trustedLoginData.Host))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(trustedLoginData.Host));
            }
            if (string.IsNullOrWhiteSpace(trustedLoginData.Account))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(trustedLoginData.Account));
            }

            // Fix optional parameters
            trustedLoginData = trustedLoginData.Clone();
            if (trustedLoginData.Password == null)
            {
                trustedLoginData.Password = string.Empty;
            }
            if (trustedLoginData.DeviceId == null)
            {
                trustedLoginData.DeviceId = string.Empty;
            }

            // Save data
            var all = GetAll();
            for (var i = 0; i < all.Count; i++)
            {
                if (trustedLoginData.Host.Equals(all[i].Host, StringComparison.InvariantCultureIgnoreCase) && trustedLoginData.Account.Equals(all[i].Account))
                {
                    all.RemoveAt(i);
                    break;
                }
            }
            all.Insert(0, trustedLoginData);
            all = all.Take(20).ToList();

            var json = JsonHelper.ToJson(all);
            var settingValues = ApplicationData.Current.LocalSettings.Values;
            settingValues[TrustedLoginDataKey] = json;
        }
    }
}