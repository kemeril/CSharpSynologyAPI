using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;
using StdUtils;

namespace KDSVideo.Infrastructure
{
    public class TrustedLoginDataHandler : ITrustedLoginDataHandler
    {
        private const string TRUSTED_LOGIN_DATA_KEY = nameof(TRUSTED_LOGIN_DATA_KEY);
        private const int MAX_ITEM_STORAGE = 1000;

        private IList<TrustedLoginData> GetAll()
        {
            var settingValues = ApplicationData.Current.LocalSettings.Values;

            if (!settingValues.TryGetValue(TRUSTED_LOGIN_DATA_KEY, out var listObject))
            {
                return new List<TrustedLoginData>();
            }

            return listObject != null && !string.IsNullOrWhiteSpace((string)listObject)
                ? JsonHelper.FromJson<List<TrustedLoginData>>((string)listObject) ?? new List<TrustedLoginData>()
                : new List<TrustedLoginData>();
        }

        public string? GetDeviceId(string host, string account, string password)
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

            return all.FirstOrDefault(trustedLoginData =>
                host.Equals(trustedLoginData.Host, StringComparison.CurrentCultureIgnoreCase)
                && account.Equals(trustedLoginData.Account, StringComparison.CurrentCultureIgnoreCase)
                && password.Equals(trustedLoginData.Password, StringComparison.Ordinal))
                ?.DeviceId;
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
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(password));
            }
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(deviceId));
            }

            // Save data
            var all = GetAll();
            for (var i = 0; i < all.Count; i++)
            {
                var trustedLoginData = all[i];
                if (host.Equals(trustedLoginData.Host, StringComparison.CurrentCultureIgnoreCase) && account.Equals(trustedLoginData.Account, StringComparison.CurrentCultureIgnoreCase))
                {
                    all.RemoveAt(i);
                    break;
                }
            }
            all.Insert(0, new TrustedLoginData
            {
                Host = host,
                Account = account,
                Password = password,
                DeviceId = deviceId
            });
            all = all.Take(MAX_ITEM_STORAGE).ToList();

            var json = JsonHelper.ToJson(all);
            var settingValues = ApplicationData.Current.LocalSettings.Values;
            settingValues[TRUSTED_LOGIN_DATA_KEY] = json;
        }

        public void RemoveIfExist(string host, string account)
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

            // Update and save data
            var all = GetAll();

            for (var i = 0; i < all.Count; i++)
            {
                var trustedLoginData = all[i];
                if (host.Equals(trustedLoginData.Host, StringComparison.CurrentCultureIgnoreCase) && account.Equals(trustedLoginData.Account, StringComparison.CurrentCultureIgnoreCase))
                {
                    all.RemoveAt(i);

                    var json = JsonHelper.ToJson(all);
                    var settingValues = ApplicationData.Current.LocalSettings.Values;
                    settingValues[TRUSTED_LOGIN_DATA_KEY] = json;

                    return;
                }
            }
        }
    }
}
