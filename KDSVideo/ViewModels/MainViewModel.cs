using System;
using System.Collections.Generic;
using System.Linq;
using KDSVideo.Infrastructure;
using KDSVideo.Messages;
using KDSVideo.UIHelpers;
using Windows.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SynologyAPI.SynologyRestDAL.Vs;

namespace KDSVideo.ViewModels
{
    public partial class MainViewModel : ObservableRecipient
    {
        private readonly INavigationService _navigationService;
        private readonly IMessenger _messenger;

        public MainViewModel(INavigationService navigationService, IMessenger messenger)
            : base(messenger)
        {
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _messenger = messenger;
            IsActive = true;
        }

        public NavigationViewPaneDisplayMode PaneDisplayMode => IsNavigationVisible
            ? NavigationViewPaneDisplayMode.Left
            : NavigationViewPaneDisplayMode.LeftMinimal;

        public NavigationViewBackButtonVisible IsBackButtonVisible => IsNavigationVisible
            ? NavigationViewBackButtonVisible.Auto
            : NavigationViewBackButtonVisible.Collapsed;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PaneDisplayMode))]
        [NotifyPropertyChangedFor(nameof(IsBackButtonVisible))]
        [NotifyCanExecuteChangedFor(nameof(NavigateBackCommand))]
        private bool _isNavigationVisible;

        [ObservableProperty]
        private List<NavigationItemBase> _libraries = new();

        [RelayCommand(CanExecute = nameof(IsNavigationVisible))]
        private void NavigateBack() => _navigationService.GoBack();

        [RelayCommand]
        private void NavigateTo(NavigationViewItemInvokedEventArgs? args)
        {
            // could also use a converter on the command parameter if you don't like
            // the idea of passing in a NavigationViewItemInvokedEventArgs

            if (args == null)
            {
                return;
            }

            if (args.IsSettingsInvoked)
            {
                _navigationService.NavigateTo(PageNavigationKey.SETTINGS_PAGE);
                return;
            }

            if (args.InvokedItemContainer?.Tag is not Library library)
            {
                return;
            }

            NavigateToLibrary(library);
        }

        protected override void OnActivated()
        {
            _messenger.Register<LoginMessage>(this, (_, loginMessage) => LoginMessageReceived(loginMessage));
            _messenger.Register<LogoutMessage>(this, (_, _) => LogoutMessageReceived());
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
            Libraries = new List<NavigationItemBase>();
        }

        private static List<NavigationItemBase> ConvertLibraries(IEnumerable<Library> libraries)
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

        private void NavigateToLibrary(Library library)
        {
            switch (library.LibraryType)
            {
                case LibraryType.Movie:
                    SelectLibraryOnUI(library);
                    _navigationService.NavigateTo(PageNavigationKey.MOVIE_PAGE, library);
                    break;
                case LibraryType.TvShow:
                    SelectLibraryOnUI(library);
                    _navigationService.NavigateTo(PageNavigationKey.TV_SHOW_PAGE, library);
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
            foreach (var navigationItemBase in Libraries)
            {
                if (navigationItemBase is NavigationCategory navigationCategory && navigationCategory.Library == library)
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
