using KDSVideo.Infrastructure;
using KDSVideo.ViewModels.NavigationViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace KDSVideo.ViewModels
{
    public class ViewModelLocator
    {
#pragma warning disable IDE1006 // Naming Styles
        // ReSharper disable InconsistentNaming
        private static LoginViewModel? _loginViewModel;
        private static LogoffViewModel? _logoffViewModel;
        private static MainViewModel? _mainViewModel;
        private static SettingsViewModel? _settingsViewModel;
        private static MovieViewModel? _movieViewModel;
        // ReSharper restore InconsistentNaming
#pragma warning restore IDE1006 // Naming Styles

        public static LoginViewModel LoginViewModel => _loginViewModel ??= ServiceLocator.Services.GetRequiredService<LoginViewModel>();
        public static LogoffViewModel LogoffViewModel => _logoffViewModel ??= ServiceLocator.Services.GetRequiredService<LogoffViewModel>();
        public static SettingsViewModel SettingsViewModel => _settingsViewModel ??= ServiceLocator.Services.GetRequiredService<SettingsViewModel>();
        public static MainViewModel MainViewModel => _mainViewModel ??= ServiceLocator.Services.GetRequiredService<MainViewModel>();
        public static MovieViewModel MovieViewModel => _movieViewModel ??= ServiceLocator.Services.GetRequiredService<MovieViewModel>();

        public static void Init()
        {
            _settingsViewModel ??= ServiceLocator.Services.GetRequiredService<SettingsViewModel>();
        }
    }
}
