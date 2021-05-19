using System;
using Windows.Storage;

namespace KDSVideo.Infrastructure
{
    public class DeviceIdProvider : IDeviceIdProvider
    {
        private const string DEVICE_ID_KEY = nameof(DEVICE_ID_KEY);

        public string GetDeviceId()
        {
            var settingValues = ApplicationData.Current.LocalSettings.Values;
            if (settingValues.TryGetValue(DEVICE_ID_KEY, out var deviceIdObject))
            {
                return (string)deviceIdObject;
            }

            var deviceId = CreateDeviceId();
            settingValues[DEVICE_ID_KEY] = deviceId;
            return deviceId;
        }

        private static string CreateDeviceId() =>
            Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace("=", "")
                .Replace("+", "");
    }
}
