using System;
using KDSVideo.ViewModels;
using KDSVideo.ViewModels.NavigationViewModels;
using KDSVideo.Views;
using KDSVideo.Views.NavigationViews;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Messaging;
using SynologyAPI;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace KDSVideo.Infrastructure
{
    public static class ServiceProviderBuilder
    {
        private static volatile IServiceProvider Services;

        public static IServiceProvider ConfigureServices()
        {
            if (Services != null)
            {
                return Services;
            }

            //ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            var services = new ServiceCollection();

            var navigationService = new NavigationService()
                .Configure(PageNavigationKey.LoginPage, typeof(LoginPage))
                .Configure(PageNavigationKey.MoviePage, typeof(MoviePage))
                .Configure(PageNavigationKey.TvShowPage, typeof(TVShowPage))
                .Configure(PageNavigationKey.SettingsPage, typeof(SettingsPage))
                .ConfigureBackNavigationTransition(NavigationService.UnknownPageKey, PageNavigationKey.LoginPage)
                .ConfigureBackNavigationTransition(PageNavigationKey.MoviePage, PageNavigationKey.LoginPage)
                .ConfigureBackNavigationTransition(PageNavigationKey.CurrentMoviePage, PageNavigationKey.MoviePage)
                .ConfigureBackNavigationTransition(PageNavigationKey.TvShowPage, PageNavigationKey.LoginPage)
                .ConfigureBackNavigationTransition(PageNavigationKey.CurrentTvShowPage, PageNavigationKey.TvShowPage)
                .ConfigureBackNavigationTransition(PageNavigationKey.SettingsPage, PageNavigationKey.LoginPage);

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
            services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
            services.AddSingleton<IAuthenticationIdProvider, AuthenticationIdProvider>();
            services.AddSingleton<IDeviceIdProvider, DeviceIdProvider>();
            services.AddSingleton<IAutoLoginDataHandler, AutoLoginDataHandler>();
            services.AddSingleton<IHistoricalLoginDataHandler, HistoricalLoginDataHandler>();
            services.AddSingleton<ITrustedLoginDataHandler, TrustedLoginDataHandler>();
            services.AddSingleton<INetworkService, NetworkService>();
            services.AddSingleton<IVideoStation, VideoStation>();
            services.AddSingleton<IVideoSettingsDataHandler, VideoSettingsDataHandler>();
            services.AddSingleton<INavigationService>(navigationService);

            if (DesignMode.DesignModeEnabled)
            {
                services.AddSingleton<IApplicationInfoService, Mock.ApplicationInfoService>();
            }
            else
            {
                services.AddSingleton<IApplicationInfoService, ApplicationInfoService>();
            }

            services.AddSingleton<LoginViewModel>();
            services.AddSingleton<LogoffViewModel>();
            services.AddSingleton<SettingsViewModel>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<MovieViewModel>();

            Services = services.BuildServiceProvider();

            return Services;
        }
    }
}
