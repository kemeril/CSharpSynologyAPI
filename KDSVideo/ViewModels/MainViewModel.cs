using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;

namespace KDSVideo.ViewModels
{
    public class MainViewModel
    {
        private readonly INavigationService _navigationService;

        public RelayCommand NavigateCommand { get; }

        public MainViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            NavigateCommand = new RelayCommand(()=> _navigationService.NavigateTo(ViewModelLocator.LoginPageKey));
        }
    }
}