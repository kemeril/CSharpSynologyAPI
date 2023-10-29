using KDSVideo.Infrastructure;
using KDSVideo.ViewModels.NavigationViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace KDSVideo.ViewModels
{
    public class ViewModelLocator
    {
        private static LoginViewModel _loginViewModel;
        private static LogoffViewModel _logoffViewModel;
        private static MainViewModel _mainViewModel;
        private static SettingsViewModel _settingsViewModel;
        private static MovieViewModel _movieViewModel;

        public static LoginViewModel LoginViewModel => _loginViewModel ??= ServiceLocator.Services.GetService<LoginViewModel>();
        public static LogoffViewModel LogoffViewModel => _logoffViewModel ??= ServiceLocator.Services.GetService<LogoffViewModel>();
        public static SettingsViewModel SettingsViewModel => _settingsViewModel ??= ServiceLocator.Services.GetService<SettingsViewModel>();
        public static MainViewModel MainViewModel => _mainViewModel ??= ServiceLocator.Services.GetService<MainViewModel>();
        public static MovieViewModel MovieViewModel => _movieViewModel ??= ServiceLocator.Services.GetService<MovieViewModel>();

        public static void Init()
        {
            _settingsViewModel ??= ServiceLocator.Services.GetService<SettingsViewModel>();
        }
    }
}
