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

        public static LoginViewModel LoginViewModel => _loginViewModel ?? (_loginViewModel = App.Services.GetService<LoginViewModel>());
        public static LogoffViewModel LogoffViewModel => _logoffViewModel ?? (_logoffViewModel = App.Services.GetService<LogoffViewModel>());
        public static SettingsViewModel SettingsViewModel => _settingsViewModel ?? (_settingsViewModel = App.Services.GetService<SettingsViewModel>());
        public static MainViewModel MainViewModel => _mainViewModel ?? (_mainViewModel = App.Services.GetService<MainViewModel>());
        public static MovieViewModel MovieViewModel => _movieViewModel ?? (_movieViewModel = App.Services.GetService<MovieViewModel>());

        public static void Init()
        {
            if (_settingsViewModel == null)
            {
                _settingsViewModel = App.Services.GetService<SettingsViewModel>();
            }
        }
    }
}
