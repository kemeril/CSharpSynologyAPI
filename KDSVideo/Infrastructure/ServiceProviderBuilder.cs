using System;
using KDSVideo.ViewModels;
using KDSVideo.ViewModels.NavigationViewModels;
using KDSVideo.Views;
using KDSVideo.Views.NavigationViews;
using Microsoft.Extensions.DependencyInjection;
using SynologyAPI;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using CommunityToolkit.Mvvm.Messaging;
using KDSVideo.Messages;

namespace KDSVideo.Infrastructure
{
    public static class ServiceProviderBuilder
    {
        private static volatile IServiceProvider? Services;

        public static IServiceProvider ConfigureServices()
        {
            if (Services != null)
            {
                return Services;
            }

            return Services = new ServiceCollection()
                .ConfigureNavigation()
                .RegisterAppServices()
                .RegisterViewModels()
                .BuildServiceProvider();
        }

        private static IServiceCollection ConfigureNavigation(this IServiceCollection services)
        {
            var navigationService = new NavigationService()
                .Configure(PageNavigationKey.LOGIN_PAGE, typeof(LoginPage))
                .Configure(PageNavigationKey.MOVIE_PAGE, typeof(MoviePage))
                .Configure(PageNavigationKey.TV_SHOW_PAGE, typeof(TVShowPage))
                .Configure(PageNavigationKey.SETTINGS_PAGE, typeof(SettingsPage))
                .ConfigureBackNavigationTransition(NavigationService.UNKNOWN_PAGE_KEY, PageNavigationKey.LOGIN_PAGE)
                .ConfigureBackNavigationTransition(PageNavigationKey.MOVIE_PAGE, PageNavigationKey.LOGIN_PAGE)
                .ConfigureBackNavigationTransition(PageNavigationKey.CURRENT_MOVIE_PAGE, PageNavigationKey.MOVIE_PAGE)
                .ConfigureBackNavigationTransition(PageNavigationKey.TV_SHOW_PAGE, PageNavigationKey.LOGIN_PAGE)
                .ConfigureBackNavigationTransition(PageNavigationKey.CURRENT_TV_SHOW_PAGE, PageNavigationKey.TV_SHOW_PAGE)
                .ConfigureBackNavigationTransition(PageNavigationKey.SETTINGS_PAGE, PageNavigationKey.LOGIN_PAGE);

            navigationService.Navigating += OnNavigationServiceNavigating;

            navigationService.Navigated += (_, args) =>
            {
                ((navigationService.CurrentFrame?.Content as FrameworkElement)?.DataContext as INavigable)?
                    .Navigated(navigationService, args);
            };

            services.AddSingleton<INavigationService>(navigationService);

            return services;
        }

        private static async void OnNavigationServiceNavigating(object _, NavigatingCancelEventArgs args)
        {
            if (args is { Cancel: false, NavigationMode: NavigationMode.Back } && PageNavigationKey.LOGIN_PAGE.Equals(args.SourcePageKey, StringComparison.Ordinal))
            {
                args.Cancel = true;
                var logoffDialog = new LogoffDialog();
                if (await logoffDialog.ShowAsync() == ContentDialogResult.Primary)
                {
                    WeakReferenceMessenger.Default.Send<LogoutMessage>();
                }
            }
        }

        private static IServiceCollection RegisterAppServices(this IServiceCollection services)
        {
            services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
            services.AddSingleton<IDeviceIdProvider, DeviceIdProvider>();
            services.AddSingleton<IAutoLoginDataHandler, AutoLoginDataHandler>();
            services.AddSingleton<IHistoricalLoginDataHandler, HistoricalLoginDataHandler>();
            services.AddSingleton<ITrustedLoginDataHandler, TrustedLoginDataHandler>();
            services.AddSingleton<INetworkService, NetworkService>();
            services.AddSingleton<IVideoStation, VideoStation>();
            services.AddSingleton<IVideoSettingsDataHandler, VideoSettingsDataHandler>();


            if (DesignMode.DesignModeEnabled)
            {
                services.AddSingleton<IApplicationInfoService, Mock.ApplicationInfoService>();
            }
            else
            {
                services.AddSingleton<IApplicationInfoService, ApplicationInfoService>();
            }

            return services;
        }

        private static IServiceCollection RegisterViewModels(this IServiceCollection services)
        {
            services.AddSingleton<LoginViewModel>();
            services.AddSingleton<LogoffViewModel>();
            services.AddSingleton<SettingsViewModel>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<MovieViewModel>();

            return services;
        }
    }
}
