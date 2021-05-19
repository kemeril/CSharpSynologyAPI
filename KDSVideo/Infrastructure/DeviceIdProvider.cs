using System;
using Windows.Storage;

namespace KDSVideo.Infrastructure
{
    public class DeviceIdProvider : IDeviceIdProvider
    {
        private const string DEVICE_ID = nameof(DEVICE_ID);

        public string GetDeviceId()
        {
            var settingValues = ApplicationData.Current.LocalSettings.Values;
            if (settingValues.TryGetValue(DEVICE_ID, out var deviceIdObject))
            {
                return (string)deviceIdObject;
            }

            var deviceId = CreateDeviceId();
            settingValues[DEVICE_ID] = deviceId;
            return deviceId;
        }

        private static string CreateDeviceId() =>
            Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace("=", "")
                .Replace("+", "");
    }
}
