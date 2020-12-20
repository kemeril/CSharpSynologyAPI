using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using KDSVideo.Infrastructure;
using KDSVideo.Messages;
using SynologyRestDAL.Vs;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls;
using KDSVideo.UIHelpers;

namespace KDSVideo.ViewModels
{
    public class MainViewModel : ViewModelBase, IDisposable
    {
        private readonly INavigationService _navigationService;
        private readonly IMessenger _messenger;
        
        private bool _disposedValue;
        private bool _isNavigationVisible;
        private IReadOnlyCollection<NavigationItemBase> _libraries = new List<NavigationItemBase>().AsReadOnly();

        public RelayCommand NavigateBackCommand { get; }

        public MainViewModel(INavigationService navigationService, IMessenger messenger)
        {
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
            NavigateBackCommand = new RelayCommand(() => _navigationService.GoBack(), () => IsNavigationVisible);
            RegisterMessages();
        }

        ~MainViewModel()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public bool IsNavigationVisible
        {
            get => _isNavigationVisible;
            private set
            {
                Set(nameof(IsNavigationVisible), ref _isNavigationVisible, value);
                RaisePropertyChanged(nameof(PaneDisplayMode));
                RaisePropertyChanged(nameof(IsBackButtonVisible));
            }
        }

        public NavigationViewPaneDisplayMode PaneDisplayMode => IsNavigationVisible
            ? NavigationViewPaneDisplayMode.Left
            : NavigationViewPaneDisplayMode.LeftMinimal;

        public NavigationViewBackButtonVisible IsBackButtonVisible => IsNavigationVisible
            ? NavigationViewBackButtonVisible.Auto
            : NavigationViewBackButtonVisible.Collapsed;

        public IReadOnlyCollection<NavigationItemBase> Libraries
        {
            get => _libraries;
            private set => Set(nameof(Libraries), ref _libraries, value);
        }

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
            _messenger.Register<LoginMessage>(this, LoginMessageReceived);
            _messenger.Register<LogoutMessage>(this, LogoutMessageReceived);
        }

        private void UnregisterMessages()
        {
            _messenger.Unregister<LoginMessage>(this, LoginMessageReceived);
            _messenger.Unregister<LogoutMessage>(this, LogoutMessageReceived);
        }

        private void LoginMessageReceived(LoginMessage loginMessage)
        {
            Libraries = ConvertLibraries(loginMessage.Libraries);
            IsNavigationVisible = true;
            _navigationService.ClearContent();
        }

        private void LogoutMessageReceived(LogoutMessage logoffMessage)
        {
            IsNavigationVisible = false;
            Libraries = new List<NavigationItemBase>().AsReadOnly();
        }

        private IReadOnlyCollection<NavigationItemBase> ConvertLibraries(IEnumerable<Library> libraries)
        {
            var tmpLibraries = libraries as Library[] ?? libraries.ToArray();
            var builtInLibraries = tmpLibraries
                .Where(library => library.Id == 0 && library.Visible)
                .Select(library => new NavigationCategory
                {
                    Name = library.Title,
                    Glyph = Symbol.Play,
                    Tooltip = null,
                    Library = library
                })
                .ToArray();
            var customLibraries = tmpLibraries
                .Where(library => library.Id != 0 && library.Visible)
                .Select(library => new NavigationCategory
                {
                    Name = library.Title,
                    Glyph = Symbol.Play,
                    Tooltip = null,
                    Library = library
                })
                .ToArray();

            var result = new List<NavigationItemBase>(builtInLibraries);
            
            // Add separator
            if (builtInLibraries.Length > 0 && customLibraries.Length > 0)
            {
                result.Add(new NavigationMenuSeparator());
            }

            result.AddRange(customLibraries);

            return result;
        }
    }
}