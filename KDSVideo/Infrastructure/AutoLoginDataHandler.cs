using System;
using Windows.Storage;
using StdUtils;

namespace KDSVideo.Infrastructure
{
    public class AutoLoginDataHandler : IAutoLoginDataHandler
    {
        private const string AutoLoginDataHandlerKey = nameof(AutoLoginDataHandlerKey);

        public AutoLoginData Get()
        {
            var settingValues = ApplicationData.Current.LocalSettings.Values;

            if (!settingValues.TryGetValue(AutoLoginDataHandlerKey, out var valueObject))
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
            settingValues[AutoLoginDataHandlerKey] = json;
        }

        public void Clear()
        {
            var settingValues = ApplicationData.Current.LocalSettings.Values;
            settingValues[AutoLoginDataHandlerKey] = string.Empty;
        }
    }
}