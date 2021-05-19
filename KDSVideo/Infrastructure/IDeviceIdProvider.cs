namespace KDSVideo.Infrastructure
{
    public interface IDeviceIdProvider
    {
        /// <summary>
        /// Create or get existing device id.
        /// </summary>
        /// <returns>Returns the device id.</returns>
        string GetDeviceId();
    }
}
