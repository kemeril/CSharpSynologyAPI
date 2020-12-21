namespace KDSVideo.Infrastructure.Mock
{
    public class ApplicationInfoService : IApplicationInfoService
    {
        public string GetAppVersion() => "1.0.0.0";
    }
}
