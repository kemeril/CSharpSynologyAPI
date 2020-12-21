using CommonServiceLocator;

namespace KDSVideo.ViewModels
{
    public class ViewModelLocator
    {
        private static LoginViewModel _loginViewModel;
        private static LogoffViewModel _logoffViewModel;
        private static  MainViewModel _mainViewModel;
        private static SettingsViewModel _settingsViewModel;

        public static LoginViewModel LoginViewModel => _loginViewModel ?? (_loginViewModel = ServiceLocator.Current.GetInstance<LoginViewModel>());
        public static LogoffViewModel LogoffViewModel => _logoffViewModel ?? (_logoffViewModel = ServiceLocator.Current.GetInstance<LogoffViewModel>());
        public static SettingsViewModel SettingsViewModel => _settingsViewModel ?? (_settingsViewModel = ServiceLocator.Current.GetInstance<SettingsViewModel>());
        public static  MainViewModel MainViewModel => _mainViewModel ?? (_mainViewModel = ServiceLocator.Current.GetInstance<MainViewModel>());
        
        public static void Init()
        {
            if (_settingsViewModel == null)
            {
                _settingsViewModel = ServiceLocator.Current.GetInstance<SettingsViewModel>();
            }
        }
    }
}