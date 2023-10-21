using System;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using KDSVideo.Infrastructure;
using KDSVideo.Messages;

namespace KDSVideo.ViewModels
{
    public class LogoffViewModel
    {
        public RelayCommand LogoffCommand { get; }

        public LogoffViewModel(INavigationService navigationService, IMessenger messenger)
        {
            if (navigationService == null)
            {
                throw new ArgumentNullException(nameof(navigationService));
            }
            if (messenger == null)
            {
                throw new ArgumentNullException(nameof(messenger));
            }

            LogoffCommand = new RelayCommand(() =>
            {
                messenger.Send(new LogoutMessage());
                navigationService.NavigateTo(PageNavigationKey.LoginPage);
            });
        }
    }
}
