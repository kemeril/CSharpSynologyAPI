using KDSVideo.Infrastructure;
using KDSVideo.Views;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using KDSVideo.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace KDSVideo
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            ServiceLocator.Services = ServiceProviderBuilder.ConfigureServices();
            Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested;
            var navigationService = ServiceLocator.Services.GetService<INavigationService>();

            var mainPage = Window.Current.Content as MainPage;
            if (navigationService != null && mainPage == null)
            {
                mainPage = new MainPage();
                var rootFrame = mainPage.NavigationFrame;
                rootFrame.NavigationFailed += (_, args) => throw new Exception("Failed to load Page " + args.SourcePageType.FullName);
                navigationService.CurrentFrame = rootFrame;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                Window.Current.Content = mainPage;
            }

            if (navigationService != null && e.PrelaunchActivated == false)
            {
                if (navigationService.CurrentFrame?.Content == null)
                {
                    ViewModelLocator.Init();

                    // Set initial navigation page
                    navigationService.NavigateTo(PageNavigationKey.LOGIN_PAGE);
                }

                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        private void App_BackRequested(object sender, Windows.UI.Core.BackRequestedEventArgs e)
        {
            e.Handled = On_BackRequested();
        }

        private bool On_BackRequested()
        {
            try
            {
                var navigationService = ServiceLocator.Services.GetService<INavigationService>();
                if (navigationService != null && navigationService.CanGoBack)
                {
                    navigationService.GoBack();
                    return true;
                }
            }
            catch
            {
                // Intentional left blank
            }

            return false;
        }
    }
}
