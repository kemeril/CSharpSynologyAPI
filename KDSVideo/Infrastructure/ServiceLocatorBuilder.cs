using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using KDSVideo.ViewModels;
using KDSVideo.Views;
using SynologyAPI;

namespace KDSVideo.Infrastructure
{
    public static class ServiceLocatorBuilder
    {
        private static volatile IServiceLocator _serviceLocator;

        public static IServiceLocator Build()
        {
            if (_serviceLocator != null)
            {
                return _serviceLocator;
            }

            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            var navigationService = new NavigationServiceEx();
            navigationService.Configure(PageNavigationKey.LoginPage, typeof(LoginPage));

            //Register your services used here
            SimpleIoc.Default.Register<IMessenger>(() => Messenger.Default);
            SimpleIoc.Default.Register<IDeviceIdProvider>(() => new DeviceIdProvider());
            SimpleIoc.Default.Register<IAutoLoginDataHandler>(() => new AutoLoginDataHandler());
            SimpleIoc.Default.Register<IHistoricalLoginDataHandler>(() => new HistoricalLoginDataHandler());
            SimpleIoc.Default.Register<ITrustedLoginDataHandler>(() => new TrustedLoginDataHandler());
            SimpleIoc.Default.Register<INetworkService>(() => new NetworkService());
            SimpleIoc.Default.Register<IVideoStation>(() => new VideoStation());
            SimpleIoc.Default.Register<INavigationServiceEx>(() => navigationService);
            SimpleIoc.Default.Register<LoginViewModel>();
            SimpleIoc.Default.Register<MainViewModel>();

            _serviceLocator = ServiceLocator.Current;

            return _serviceLocator;
        }
    }
}
