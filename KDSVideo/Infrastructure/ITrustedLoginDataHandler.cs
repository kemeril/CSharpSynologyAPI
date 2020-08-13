namespace KDSVideo.Infrastructure
{
    public interface ITrustedLoginDataHandler
    {
        string GetDeviceId(string host, string account, string password);
        void AddOrUpdate(string host, string account, string password, string deviceId);
    }
}