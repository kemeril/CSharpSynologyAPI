using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using KDSVideo.Infrastructure;
using KDSVideo.Messages;
using SynologyRestDAL.Vs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
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

        public ICommand NavigateBackCommand { get; }
        public ICommand NavigateToCommand { get; }

        public MainViewModel(INavigationService navigationService, IMessenger messenger)
        {
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
            NavigateBackCommand = new RelayCommand(() => _navigationService.GoBack(), () => IsNavigationVisible);
            NavigateToCommand = new RelayCommand<NavigationViewItemInvokedEventArgs>(NavigateToInvoked);
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

            var initialLibrary = Libraries
                .OfType<NavigationCategory>()
                .Cast<NavigationCategory>()
                .FirstOrDefault();

            if (initialLibrary == null)
            {
                _navigationService.ClearContent();
            }
            else
            {
                NavigateToLibrary(initialLibrary.Library);
            }
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
                .Where(library => library.Id == 0 && library.Visible && library.LibraryType == LibraryType.Movie || library.LibraryType == LibraryType.TvShow)
                .Select(library => new NavigationCategory
                {
                    Name = LibraryNameConverter.GetLibraryName(library),
                    Library = library
                })
                .ToArray();
            var customLibraries = tmpLibraries
                .Where(library => library.Id != 0 && library.Visible)
                .Select(library => new NavigationCategory
                {
                    Name = LibraryNameConverter.GetLibraryName(library),
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

        private void NavigateToInvoked(NavigationViewItemInvokedEventArgs args)
        {
            // could also use a converter on the command parameter if you don't like
            // the idea of passing in a NavigationViewItemInvokedEventArgs

            if (args == null)
            {
                return;
            }

            if (args.IsSettingsInvoked)
            {
                _navigationService.NavigateTo(PageNavigationKey.SettingsPage);
                return;
            }
            
            if (!(args.InvokedItemContainer?.Tag is Library library))
            {
                return;
            }

            NavigateToLibrary(library);
        }

        private void NavigateToLibrary(Library library)
        {
            switch (library.LibraryType)
            {
                case LibraryType.Movie:
                    _navigationService.NavigateTo(PageNavigationKey.MoviePage, library);
                    break;
                case LibraryType.TvShow:
                    _navigationService.NavigateTo(PageNavigationKey.TvShowPage, library);
                    break;
                case LibraryType.HomeVideo:
                case LibraryType.TvRecord:
                case LibraryType.Unknown:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}