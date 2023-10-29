using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using KDSVideo.Infrastructure;
using KDSVideo.Messages;
using KDSVideo.Views;
using Microsoft.Extensions.DependencyInjection;
using SynologyAPI;
using SynologyAPI.SynologyRestDAL.Vs;

namespace KDSVideo.ViewModels.NavigationViewModels.TabViewModels
{
    public class MetaDataItemsAllTabViewModel : ObservableRecipient, IDisposable
    {
        private readonly IMessenger _messenger;
        private readonly IVideoStation _videoStation;
        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(30);

        private bool _disposedValue;

        private bool _showProgressIndicator;
        private Library _library;
        private ObservableCollection<MediaMetaDataItem> _mediaMetaDataItems;

        public MetaDataItemsAllTabViewModel()
        {
            if (ServiceLocator.Services != null)
            {
                _messenger = ServiceLocator.Services.GetService<IMessenger>();
                _videoStation = ServiceLocator.Services.GetService<IVideoStation>();

                IsActive = true;
            }
        }

        ~MetaDataItemsAllTabViewModel()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public bool ShowProgressIndicator
        {
            get => _showProgressIndicator;
            set => SetProperty(ref _showProgressIndicator, value);
        }

        public ObservableCollection<MediaMetaDataItem> MediaMetaDataItems
        {
            get => _mediaMetaDataItems;
            private set => SetProperty(ref _mediaMetaDataItems, value);
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
            _library = null;
            MediaMetaDataItems = new ObservableCollection<MediaMetaDataItem>();
        }

        public void RefreshData()
        {
            RefreshData(_library, true);
        }

        public async void RefreshData(Library library, bool forceUpdate)
        {
            if (_videoStation == null)
            {
                return;
            }

            if (!forceUpdate && _library == library)
            {
                return;
            }

            CleanUp();
            _library = library;
            if (_library == null)
            {
                return;
            }

            var cts = new CancellationTokenSource(_timeout);

            ShowProgressIndicator = true;
            try
            {
                switch (library.LibraryType)
                {
                    case LibraryType.Movie:
                        {
                            var moviesInfo = await _videoStation.MovieListAsync(library.Id, cancellationToken: cts.Token);
                            MediaMetaDataItems = new ObservableCollection<MediaMetaDataItem>(moviesInfo.Movies
                                .Select(item => new MovieMetaDataItem(item)));
                            break;
                        }
                    case LibraryType.TvShow:
                        var tvShowsInfo = await _videoStation.TvShowListAsync(library.Id, cancellationToken: cts.Token);
                        MediaMetaDataItems = new ObservableCollection<MediaMetaDataItem>(tvShowsInfo.TvShows
                            .Select(item => new TvShowMetaDataItem(item)));
                        break;
                    case LibraryType.HomeVideo:
                    case LibraryType.TvRecord:
                    case LibraryType.Unknown:
                        throw new NotSupportedException();
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception ex)
            {
                var errorCode = ApplicationLevelErrorCodes.UnknownError;
                switch (ex)
                {
                    case OperationCanceledException _:
                        errorCode = ApplicationLevelErrorCodes.OperationTimeOut;
                        break;
                    case WebException { Response: null }:
                        errorCode = ApplicationLevelErrorCodes.ConnectionWithTheServerCouldNotBeEstablished;
                        break;
                }

                var errorMessage = ApplicationLevelErrorMessages.GetErrorMessage(errorCode);
                Trace.TraceWarning($"{errorMessage}. Exception: {ex}");
                CleanUp();

                if (!string.IsNullOrWhiteSpace(errorMessage))
                {
                    await new ErrorDialog(errorMessage).ShowAsync();
                }
            }
            finally
            {
                ShowProgressIndicator = false;
            }
        }
    }
}
