using System;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using KDSVideo.Infrastructure;
using KDSVideo.Messages;

namespace KDSVideo.ViewModels
{
    public partial class LogoffViewModel
    {
        private readonly INavigationService _navigationService;

        private readonly IMessenger _messenger;

        [RelayCommand]
        private void LogOff()
        {
            _messenger.Send(new LogoutMessage());
            _navigationService.NavigateTo(PageNavigationKey.LOGIN_PAGE);
        }

        public LogoffViewModel(INavigationService navigationService, IMessenger messenger)
        {
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
        }
    }
}
