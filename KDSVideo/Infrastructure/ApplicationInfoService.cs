using Windows.ApplicationModel;

namespace KDSVideo.Infrastructure
{
    public class ApplicationInfoService : IApplicationInfoService
    {
        public string GetAppVersion()
        {
            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;

            return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }
    }
}
