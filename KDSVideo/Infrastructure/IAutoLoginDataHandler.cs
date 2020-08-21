namespace KDSVideo.Infrastructure
{
    public interface IAutoLoginDataHandler
    {
        AutoLoginData Get();
        void SetOrUpdate(string host, string account, string password);
        void Clear();
    }
}