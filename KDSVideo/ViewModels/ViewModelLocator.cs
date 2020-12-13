using CommonServiceLocator;

namespace KDSVideo.ViewModels
{
    public class ViewModelLocator
    {
        private static LoginViewModel _loginViewModel;
        private static  MainViewModel _mainViewModel;

        public static LoginViewModel LoginViewModel => _loginViewModel ?? (_loginViewModel = ServiceLocator.Current.GetInstance<LoginViewModel>());
        public static  MainViewModel MainViewModel => _mainViewModel ?? (_mainViewModel = ServiceLocator.Current.GetInstance<MainViewModel>());
    }
}