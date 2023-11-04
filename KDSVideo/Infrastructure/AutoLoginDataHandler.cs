using System;
using Windows.Storage;
using StdUtils;

namespace KDSVideo.Infrastructure
{
    public class AutoLoginDataHandler : IAutoLoginDataHandler
    {
        private const string AUTO_LOGIN_DATA_HANDLER_KEY = nameof(AUTO_LOGIN_DATA_HANDLER_KEY);

        public AutoLoginData? Get()
        {
            var settingValues = ApplicationData.Current.LocalSettings.Values;

            if (!settingValues.TryGetValue(AUTO_LOGIN_DATA_HANDLER_KEY, out var valueObject))
            {
                return null;
            }

            return valueObject != null && !string.IsNullOrWhiteSpace((string)valueObject)
                ? JsonHelper.FromJson<AutoLoginData>((string)valueObject)
                : null;
        }

        public void SetOrUpdate(string host, string account, string password)
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
            var json = JsonHelper.ToJson(new AutoLoginData
            {
                Host = host,
                Account = account,
                Password = password
            });
            var settingValues = ApplicationData.Current.LocalSettings.Values;
            settingValues[AUTO_LOGIN_DATA_HANDLER_KEY] = json;
        }

        public void Clear()
        {
            var settingValues = ApplicationData.Current.LocalSettings.Values;
            settingValues[AUTO_LOGIN_DATA_HANDLER_KEY] = string.Empty;
        }
    }
}
