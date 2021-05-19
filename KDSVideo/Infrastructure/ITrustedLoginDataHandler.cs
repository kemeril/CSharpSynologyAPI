namespace KDSVideo.Infrastructure
{
    public interface ITrustedLoginDataHandler
    {
        /// <summary>
        /// Get server side generated trusted device id for the <paramref name="host"/>, <paramref name="account"/> and <paramref name="password"/>.
        /// </summary>
        /// <param name="host">Host.</param>
        /// <param name="account">Account.</param>
        /// <param name="password">Password.</param>
        /// <returns></returns>
        string GetDeviceId(string host, string account, string password);
        void AddOrUpdate(string host, string account, string password, string deviceId);
        void RemoveIfExist(string host, string account);
    }
}
