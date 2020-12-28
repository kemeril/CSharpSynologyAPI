using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using KDSVideo.Messages;
using SynologyAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using KDSVideo.Views;
using SynologyRestDAL.Vs;

namespace KDSVideo.ViewModels.NavigationViewModels.TabViewModels
{
    public class MetaDataItemsAllTabViewModel : ViewModelBase, IDisposable
    {
        private readonly IMessenger _messenger;
        private readonly IVideoStation _videoStation;
        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(30);
        
        private bool _disposedValue;
        
        private bool _showProgressIndicator;
        private Library _library;
        private IList<MediaMetaDataItem> _mediaMetaDataItems;

        public MetaDataItemsAllTabViewModel()
        {
            if (!IsInDesignModeStatic)
            {
                _messenger = ServiceLocator.Current.GetInstance<IMessenger>();
                _videoStation = ServiceLocator.Current.GetInstance<IVideoStation>();

                RegisterMessages();
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
            set => Set(nameof(ShowProgressIndicator), ref _showProgressIndicator, value);
        }

        public IList<MediaMetaDataItem> MediaMetaDataItems
        {
            get => _mediaMetaDataItems;
            private set => Set(nameof(MediaMetaDataItems), ref _mediaMetaDataItems, value);
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
            _messenger.Register<LogoutMessage>(this, LogoutMessageReceived);
        }

        private void UnregisterMessages()
        {
            _messenger?.Unregister<LogoutMessage>(this, LogoutMessageReceived);
        }

        private void LogoutMessageReceived(LogoutMessage logoutMessage)
        {
            CleanUp();
        }
        
        private void CleanUp()
        {
            _library = null;
            MediaMetaDataItems = new List<MediaMetaDataItem>();
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
                        MediaMetaDataItems = moviesInfo.Movies
                            .Select(item => new MovieMetaDataItem(item))
                            .Cast<MediaMetaDataItem>()
                            .ToList();
                        break;
                    }
                    case LibraryType.TvShow:
                        var tvShowsInfo = await _videoStation.TvShowListAsync(library.Id, cancellationToken: cts.Token);
                        MediaMetaDataItems = tvShowsInfo.TvShows
                            .Select(item => new TvShowMetaDataItem(item))
                            .Cast<MediaMetaDataItem>()
                            .ToList();
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
                    case WebException webException when webException.Response == null:
                        errorCode = ApplicationLevelErrorCodes.ConnectionWithTheServerCouldNotBeEstablished;
                        break;
                    default:
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

            //await RefreshPosters();
        }

        private async Task RefreshPosters()
        {
            var metaDataItem = MediaMetaDataItems.FirstOrDefault();
            if (metaDataItem == null)
            {
                return;
            }
            
            var poster = await metaDataItem.GetPoster(_videoStation, 154, 234);
            metaDataItem.Poster = poster;
        }
    }
}