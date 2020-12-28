using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using KDSVideo.Infrastructure;
using KDSVideo.Messages;
using SynologyRestDAL.Vs;
using System;
using Windows.UI.Xaml.Navigation;
using NavigationEventArgs = KDSVideo.Infrastructure.NavigationEventArgs;

namespace KDSVideo.ViewModels.NavigationViewModels
{
    public class MovieViewModel : ViewModelBase, IDisposable, INavigable
    {
        private readonly IMessenger _messenger;
        private bool _disposedValue;

        public MovieViewModel(IMessenger messenger)
        {
            _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));

            RegisterMessages();
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

        public Library Library { get; private set; }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    UnregisterMessages();
                }

                _disposedValue = true;
            }
        }

        private void RegisterMessages()
        {
            _messenger.Register<LogoutMessage>(this, LogoutMessageReceived);
        }

        private void UnregisterMessages()
        {
            _messenger.Unregister<LogoutMessage>(this, LogoutMessageReceived);
        }

        private void LogoutMessageReceived(LogoutMessage logoutMessage)
        {
            CleanUp();
        }

        private void CleanUp()
        {
            Library = null;
        }
    }
}