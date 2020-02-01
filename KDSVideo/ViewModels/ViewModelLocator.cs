using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using KDSVideo.Views;

namespace KDSVideo.ViewModels
{
    public class ViewModelLocator
    {
        public const string LoginPageKey = nameof(LoginViewModel);
        public const string MainPageKey = nameof(MainViewModel);

        private static LoginViewModel _loginViewModel;
        private static MainViewModel _mainViewModel;

        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            var navigationService = new NavigationService();
            navigationService.Configure(LoginPageKey, typeof(LoginPage));
            navigationService.Configure(MainPageKey, typeof(Views.MainPage));

            //Register your services used here
            SimpleIoc.Default.Register<INavigationService>(() => navigationService);
            SimpleIoc.Default.Register<LoginViewModel>();
            SimpleIoc.Default.Register<MainViewModel>();
        }

        public static LoginViewModel LoginViewModel => _loginViewModel ?? (_loginViewModel = ServiceLocator.Current.GetInstance<LoginViewModel>());

        public static MainViewModel MainViewModel => _mainViewModel ?? (_mainViewModel = ServiceLocator.Current.GetInstance<MainViewModel>());
    }
}