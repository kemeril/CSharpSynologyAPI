using KDSVideo.Infrastructure;
using KDSVideo.Messages;
using System;
using Windows.UI.Xaml.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using SynologyAPI.SynologyRestDAL.Vs;
using NavigationEventArgs = KDSVideo.Infrastructure.NavigationEventArgs;

namespace KDSVideo.ViewModels.NavigationViewModels
{
    public class MovieViewModel : ObservableRecipient, IDisposable, INavigable
    {
        private readonly IMessenger _messenger;
        private bool _disposedValue;

        public MovieViewModel(IMessenger messenger)
            : base(messenger)
        {
            _messenger = messenger;

            IsActive = true;
        }

        ~MovieViewModel()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public void Navigated(in object sender, in NavigationEventArgs args)
        {
            if (args.NavigationMode == NavigationMode.Back)
            {
                return;
            }

            Library = args.Parameter as Library;
        }

        public Library? Library { get; private set; }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    IsActive = false;
                }

                _disposedValue = true;
            }
        }

        protected override void OnActivated()
        {
            _messenger.Register<LogoutMessage>(this, (_, _) => LogoutMessageReceived());
        }

        protected override void OnDeactivated()
        {
            _messenger.UnregisterAll(this);
        }

        private void LogoutMessageReceived()
        {
            CleanUp();
        }

        private void CleanUp()
        {
            Library = null;
        }
    }
}
