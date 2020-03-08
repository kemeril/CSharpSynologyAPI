using System;
using Windows.Storage;

namespace KDSVideo.Infrastructure
{
    public class DeviceIdProvider : IDeviceIdProvider
    {
        private const string DeviceIdKey = nameof(DeviceIdKey);

        public string GetDeviceId()
        {
            var settingValues = ApplicationData.Current.LocalSettings.Values;
            if (settingValues.TryGetValue(DeviceIdKey, out var deviceIdObject))
            {
                return (string)deviceIdObject;
            }

            var deviceId = CreateDeviceId();
            settingValues[DeviceIdKey] = deviceId;
            return deviceId;
        }

        private string CreateDeviceId() => Guid.NewGuid().ToString().Replace("-", string.Empty);
    }
}