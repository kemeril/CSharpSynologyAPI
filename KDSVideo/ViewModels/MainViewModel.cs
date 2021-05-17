using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
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
    public class MainViewModel : ObservableRecipient, IDisposable
    {
        private readonly INavigationService _navigationService;
        private readonly IMessenger _messenger;

        private bool _disposedValue;
        private bool _isNavigationVisible;
        private IReadOnlyCollection<NavigationItemBase> _libraries = new List<NavigationItemBase>().AsReadOnly();

        public ICommand NavigateBackCommand { get; }
        public ICommand NavigateToCommand { get; }

        public MainViewModel(INavigationService navigationService, IMessenger messenger)
            : base(messenger)
        {
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _messenger = messenger;
            NavigateBackCommand = new RelayCommand(() => _navigationService.GoBack(), () => IsNavigationVisible);
            NavigateToCommand = new RelayCommand<NavigationViewItemInvokedEventArgs>(NavigateToInvoked);
            IsActive = true;
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
                SetProperty(ref _isNavigationVisible, value);
                OnPropertyChanged(nameof(PaneDisplayMode));
                OnPropertyChanged(nameof(IsBackButtonVisible));
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
            private set => SetProperty(ref _libraries, value);
        }

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
            _messenger.Register<LoginMessage>(this, (recipient, loginMessage) => LoginMessageReceived(loginMessage));
            _messenger.Register<LogoutMessage>(this, (recipient, logoutMessage) => LogoutMessageReceived());
        }

        protected override void OnDeactivated()
        {
            _messenger.UnregisterAll(this);
        }

        private void LoginMessageReceived(LoginMessage loginMessage)
        {
            Libraries = ConvertLibraries(loginMessage.Libraries);
            IsNavigationVisible = true;

            var initialLibrary = Libraries
                .OfType<NavigationCategory>()
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

        private void LogoutMessageReceived()
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
                    SelectLibraryOnUI(library);
                    _navigationService.NavigateTo(PageNavigationKey.MoviePage, library);
                    break;
                case LibraryType.TvShow:
                    SelectLibraryOnUI(library);
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

        private void SelectLibraryOnUI(Library library)
        {
            foreach (var navigationItemBase in _libraries)
            {
                if (navigationItemBase is NavigationCategory navigationCategory &&
                    navigationCategory.Library == library)
                {
                    if (!navigationCategory.IsSelected)
                    {
                        navigationCategory.IsSelected = true;
                    }
                    break;
                }
            }
        }
    }
}
