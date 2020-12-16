using CommonServiceLocator;

namespace KDSVideo.ViewModels
{
    public class ViewModelLocator
    {
        private static LoginViewModel _loginViewModel;
        private static LogoffViewModel _logoffViewModel;
        private static  MainViewModel _mainViewModel;

        public static LoginViewModel LoginViewModel => _loginViewModel ?? (_loginViewModel = ServiceLocator.Current.GetInstance<LoginViewModel>());
        public static LogoffViewModel LogoffViewModel => _logoffViewModel ?? (_logoffViewModel = ServiceLocator.Current.GetInstance<LogoffViewModel>());
        public static  MainViewModel MainViewModel => _mainViewModel ?? (_mainViewModel = ServiceLocator.Current.GetInstance<MainViewModel>());
    }
}