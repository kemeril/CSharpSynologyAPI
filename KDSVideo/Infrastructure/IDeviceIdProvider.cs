namespace KDSVideo.Infrastructure
{
    public interface IDeviceIdProvider
    {
        /// <summary>
        /// Create or get existing device id.
        /// This device id describes the device where the application is running.
        /// </summary>
        /// <returns>Returns the device id.</returns>
        string GetDeviceId();
    }
}
