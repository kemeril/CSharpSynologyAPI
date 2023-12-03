using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
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
    public partial class MetaDataItemsAllTabViewModel : ObservableRecipient, IDisposable
    {
        private readonly IMessenger _messenger;
        private readonly IVideoStation _videoStation;

#if DEBUG
        private readonly TimeSpan _timeout = TimeSpan.FromHours(1);
#else
        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(30);
#endif

        private bool _disposedValue;

        private Library? _library;

        [ObservableProperty]
        private bool _showProgressIndicator;

        public MetaDataItemsAllTabViewModel()
        {
            _messenger = ServiceLocator.Services.GetRequiredService<IMessenger>();
            _videoStation = ServiceLocator.Services.GetRequiredService<IVideoStation>();

            IsActive = true;
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

        public RangeObservableCollection<MediaMetaDataItem> MediaMetaDataItems { get; } = new();

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

        protected override void OnActivated() => _messenger.Register<LogoutMessage>(this, (_, _) => CleanUp());

        protected override void OnDeactivated() => _messenger.UnregisterAll(this);

        private void CleanUp()
        {
            _library = null;
            MediaMetaDataItems.Clear();
        }

        public async Task RefreshData(Library library, bool forceUpdate)
        {
            if (!forceUpdate && _library == library)
            {
                return;
            }

            ShowProgressIndicator = false;
            try
            {
                CleanUp();
                _library = library;

                var cts = new CancellationTokenSource(_timeout);

                try
                {
                    switch (library.LibraryType)
                    {
                        case LibraryType.Movie:
                            {
                                var moviesInfo = await _videoStation.MovieListAsync(library.Id, cancellationToken: cts.Token);
                                var mediaMetaDataItems = moviesInfo.Movies.Select(item => new MovieMetaDataItem(item));

                                // TODO: "BY_FOLDER"
                                // var folders = await _videoStation.FolderListAsync(library.Id, "", LibraryType.Movie, cancellationToken: cts.Token);

                                // TODO: "JUST_ADDED"
                                // mediaMetaDataItems = mediaMetaDataItems.OrderByDescending(i => i.MetaDataItem.CreateTime);

                                // TODO: "JUST_WATCHED"
                                // mediaMetaDataItems = mediaMetaDataItems.Where(i => i.MetaDataItem.RecentlyWatched).OrderByDescending(i => i.MetaDataItem.LastWatched);

                                // TODO: "JUST_RELEASED"
                                // mediaMetaDataItems = mediaMetaDataItems.Where(i => i.MetaDataItem.OriginalAvailable.HasValue).OrderByDescending(i => i.MetaDataItem.OriginalAvailable!.Value);

                                MediaMetaDataItems.AddRange(mediaMetaDataItems);
                                break;
                            }
                        case LibraryType.TvShow:
                            {
                                var tvShowsInfo = await _videoStation.TvShowListAsync(library.Id, cancellationToken: cts.Token);
                                var mediaMetaDataItems = tvShowsInfo.TvShows.Select(item => new TvShowMetaDataItem(item));
                                MediaMetaDataItems.AddRange(mediaMetaDataItems);
                                break;
                            }
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
                    var errorCode = ex switch
                    {
                        OperationCanceledException _ => ApplicationLevelErrorCodes.OPERATION_TIME_OUT,
                        WebException { Response: null } => ApplicationLevelErrorCodes.CONNECTION_WITH_THE_SERVER_COULD_NOT_BE_ESTABLISHED,
                        _ => ApplicationLevelErrorCodes.UNKNOWN_ERROR
                    };

                    var errorMessage = ApplicationLevelErrorMessages.GetErrorMessage(errorCode);
                    Trace.TraceWarning($"{errorMessage}. Exception: {ex}");
                    CleanUp();

                    if (!string.IsNullOrWhiteSpace(errorMessage))
                    {
                        await new ErrorDialog(errorMessage).ShowAsync();
                    }
                }
            }
            finally
            {
                ShowProgressIndicator = true;
            }
        }
    }
}
