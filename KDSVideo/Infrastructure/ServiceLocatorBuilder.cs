using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using KDSVideo.ViewModels;
using KDSVideo.Views;
using SynologyAPI;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

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

            var navigationService = new NavigationService()
                .Configure(PageNavigationKey.LoginPage, typeof(LoginPage))
                .ConfigureBackNavigationTransition(NavigationService.UnknownPageKey, PageNavigationKey.LoginPage)
                .ConfigureBackNavigationTransition(PageNavigationKey.MoviePage, PageNavigationKey.LoginPage)
                .ConfigureBackNavigationTransition(PageNavigationKey.CurrentMoviePage, PageNavigationKey.MoviePage)
                .ConfigureBackNavigationTransition(PageNavigationKey.TvShowPage, PageNavigationKey.LoginPage)
                .ConfigureBackNavigationTransition(PageNavigationKey.CurrentTvShowPage, PageNavigationKey.TvShowPage);

            navigationService.Navigating += async (sender, args) =>
            {
                if (!args.Cancel && args.NavigationMode == NavigationMode.Back
                    && PageNavigationKey.LoginPage.Equals(args.SourcePageKey, StringComparison.Ordinal))
                {
                    args.Cancel = true;
                    var logoffDialog = new LogoffDialog();
                    await logoffDialog.ShowAsync();
                }
            };

            navigationService.Navigated += (sender, args) =>
            {
                ((navigationService.CurrentFrame.Content as FrameworkElement)?.DataContext as INavigable)?.Navigated(navigationService, args);
            };

            //Register your services used here
            SimpleIoc.Default.Register(() => Messenger.Default);
            SimpleIoc.Default.Register<IDeviceIdProvider>(() => new DeviceIdProvider());
            SimpleIoc.Default.Register<IAutoLoginDataHandler>(() => new AutoLoginDataHandler());
            SimpleIoc.Default.Register<IHistoricalLoginDataHandler>(() => new HistoricalLoginDataHandler());
            SimpleIoc.Default.Register<ITrustedLoginDataHandler>(() => new TrustedLoginDataHandler());
            SimpleIoc.Default.Register<INetworkService>(() => new NetworkService());
            SimpleIoc.Default.Register<IVideoStation>(() => new VideoStation());
            SimpleIoc.Default.Register<INavigationService>(() => navigationService);
            SimpleIoc.Default.Register<LoginViewModel>();
            SimpleIoc.Default.Register<LogoffViewModel>();
            SimpleIoc.Default.Register<MainViewModel>();

            _serviceLocator = ServiceLocator.Current;

            return _serviceLocator;
        }
    }
}
