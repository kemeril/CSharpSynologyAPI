using SynologyAPI.Exception;
using SynologyRestDAL;
using SynologyRestDAL.Vs;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

//https://github.com/kwent/syno/wiki/Video-Station-API

namespace SynologyAPI
{
    public sealed class VideoStation : Station
    {
        // ReSharper disable InconsistentNaming
        private const string ApiSynoVideoStationTvShow = "SYNO.VideoStation.TVShow";
        private const string ApiSynoVideoStationStreaming = "SYNO.VideoStation.Streaming";
        private const string ApiSynoVideoStationTvShowEpisode = "SYNO.VideoStation.TVShowEpisode";
        private const string ApiSynoVideoStation2TvShowEpisode = "SYNO.VideoStation2.TVShowEpisode";
        private const string ApiSynoVideoStationMovie = "SYNO.VideoStation.Movie";
        private const string ApiSynoVideoStationLibrary = "SYNO.VideoStation.Library";
        private const string ApiSynoVideoStationPoster = "SYNO.VideoStation.Poster";
        private const string ApiSynoVideoStationBackdrop = "SYNO.VideoStation.Backdrop";
        private const string ApiSynoVideoStationSubtitle = "SYNO.VideoStation.Subtitle";
        private const string ApiSynoVideoStationAudioTrack = "SYNO.VideoStation.AudioTrack";
        private const string ApiSynoVideoStationWatchStatus = "SYNO.VideoStation.WatchStatus";
        // ReSharper restore InconsistentNaming

        private const string MethodList = "list";
        private const string MethodGetInfo = "getinfo";
        private const string MethodSetInfo = "setinfo";
        private const string MethodOpen = "open";
        private const string MethodClose = "close";
        private const string MethodStream = "stream";
        private const string MethodGet = "get";
        private const string MethodGetImage = "getimage";


        private const string Additional = @"[""summary"",""actor"",""file"",""extra"",""genre"",""writer"",""director"",""collection"",""poster_mtime"",""watched_ratio"",""conversion_produced"",""backdrop_mtime"",""parental_control""]";


        #region Boilerplate
        
        public VideoStation(Uri url, IWebProxy proxy = null)
            : base(url, proxy)
        {
        }
        
        protected override string GetSessionName()
        {
            return "VideoStation";
        }

        protected override Dictionary<string, int> GetImplementedApi()
        {
            var implementedApi = base.GetImplementedApi();
            implementedApi.Add(ApiSynoVideoStationTvShow, 2);
            implementedApi.Add("SYNO.VideoStation.Info", 1);
            implementedApi.Add(ApiSynoVideoStationTvShowEpisode, 2);
            implementedApi.Add("SYNO.VideoStation2.TVShowEpisode", 1);
            implementedApi.Add(ApiSynoVideoStationLibrary, 1);
            implementedApi.Add("SYNO.VideoController.Device", 1);
            implementedApi.Add(ApiSynoVideoStationStreaming, 1);
            implementedApi.Add("SYNO.VideoStation2.Streaming", 1);
            implementedApi.Add(ApiSynoVideoStationPoster, 2);
            implementedApi.Add(ApiSynoVideoStationBackdrop, 1);
            implementedApi.Add("SYNO.VideoStation.Rating", 1);
            implementedApi.Add("SYNO.VideoStation.Collection", 1);
            implementedApi.Add("SYNO.VideoStation.TVRecording", 1);
            implementedApi.Add("SYNO.VideoStation.HomeVideo", 1);
            implementedApi.Add("SYNO.VideoStation.Video", 1);
            implementedApi.Add("SYNO.VideoStation.Movie", 1);
            implementedApi.Add(ApiSynoVideoStationSubtitle, 2);
            implementedApi.Add(ApiSynoVideoStationAudioTrack, 1);
            implementedApi.Add("SYNO.VideoStation.Folder", 1);
            implementedApi.Add(ApiSynoVideoStationWatchStatus, 1);

            return implementedApi;
        }

        public enum SortBy
        {
            Added,
            Title,
            None
        }

        public enum SortDirection
        {
            Ascending,
            Descending
        }

        public enum MediaType
        {
            Movie,
            TvShow,
            TvShowEpisode
        }


        /// <summary>
        /// Video transcoding options
        /// See: https://www.synology.com/en-us/knowledgebase/DSM/tutorial/Multimedia/How_do_I_stream_videos_smoothly_via_Video_Station_DS_video
        /// </summary>
        public enum VideoTranscoding
        {
            /// <summary>
            /// Raw (original) format.
            /// There is no video transcoding.
            /// </summary>
            Raw,

            /// <summary>
            /// The high quality.
            /// There is no video transcoding but the format is remuxed.
            /// </summary>
            HighQuality,

            /// <summary>
            /// The medium quality.
            /// There is video transcoding.
            /// </summary>
            MediumQuality,

            /// <summary>
            /// The low quality.
            /// There is video transcoding.
            /// </summary>
            LowQuality
        }

        #endregion

        #region TvShow

        /// <summary>
        /// List TV Shows
        /// </summary>
        /// <param name="libraryId">Id of a Library. Library list can be retrieve by <see cref="LibraryListAsync"/> method. The built in libraries has 0 value such as built in Movies, TVShows, HomeVideos, TVRecordings. User added libraries has an own id value.</param>
        /// <param name="sortBy">Add sorting by <see cref="SortBy"/></param>
        /// <param name="sortDirection">Add sorting direction if <paramref name="sortBy"/> is not equals to <see cref="SortBy.None"/></param>
        /// <param name="offset">Skip the given number of elements.  It has take effect if the value is greater than 0.</param>
        /// <param name="limit">Limit the number of the retrieved elements. It has take effect if the value is greater or equal to 0.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="TvShowsInfo"/>.</returns>
        /// <exception cref="SynoRequestException"> is throws on error</exception>
        public async Task<TvShowsInfo> TvShowListAsync(int libraryId, SortBy sortBy = SortBy.None, SortDirection sortDirection = SortDirection.Ascending, int offset = 0, int limit = -1, CancellationToken cancellationToken = default(CancellationToken))
        {
            var tvShowsResult = await CallMethodAsync<TvShowsResult>(ApiSynoVideoStationTvShow, MethodList,
                new ReqParams
                {
                    {"library_id", libraryId.ToString()},
                    {"offset", "0"},
                    {"additional", Additional}
                }.SortBy(sortBy, sortDirection).Offset(offset).Limit(limit), cancellationToken).ConfigureAwait(false);

            if (!tvShowsResult.Success)
                throw new SynoRequestException(ApiSynoVideoStationTvShow, MethodList, tvShowsResult.Error.Code);

            return tvShowsResult.Data;
        }

        /// <summary>
        /// Get episodes of a TV Show
        /// </summary>
        /// <param name="libraryId">Id of a Library. Library list can be retrieve by <see cref="LibraryListAsync"/> method. The built in libraries has 0 value such as built in Movies, TVShows, HomeVideos, TVRecordings. User added libraries has an own id value.</param>
        /// <param name="tvShowId">Id of a TV Show</param>
        /// <param name="sortBy">Add sorting by <see cref="SortBy"/></param>
        /// <param name="sortDirection">Add sorting direction if <paramref name="sortBy"/> is not equals to <see cref="SortBy.None"/></param>
        /// <param name="offset">Skip the given number of elements.  It has take effect if the value is greater than 0.</param>
        /// <param name="limit">Limit the number of the retrieved elements. It has take effect if the value is greater or equal to 0.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="TvEpisodesInfo"/>.</returns>
        /// <exception cref="SynoRequestException"> is throws on error</exception>
        public async Task<TvEpisodesInfo> TvShowEpisodeListAsync(int libraryId, int tvShowId, SortBy sortBy = SortBy.None, SortDirection sortDirection = SortDirection.Ascending, int offset = 0, int limit = -1, CancellationToken cancellationToken = default(CancellationToken))
        {
            var tvEpisodesResult = await CallMethodAsync<TvEpisodesResult>(ApiSynoVideoStationTvShowEpisode, MethodList,
                    new ReqParams
                    {
                        {"library_id", libraryId.ToString()},
                        {"tvshow_id", tvShowId.ToString()},
                        {"additional", Additional}
                    }.SortBy(sortBy, sortDirection).Offset(offset).Limit(limit), cancellationToken).ConfigureAwait(false);
            if (!tvEpisodesResult.Success)
                throw new SynoRequestException(ApiSynoVideoStationTvShowEpisode, MethodList, tvEpisodesResult.Error.Code);

            return tvEpisodesResult.Data;
        }

        /// <summary>
        /// Get info about an TV Show episode.
        /// </summary>
        /// <param name="tvShowEpisodeId"></param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="TvEpisodeInfo"/>.</returns>
        /// <exception cref="SynoRequestException"> is throws on error</exception>
        public async Task<TvEpisodeInfo> TvShowEpisodeGetInfoAsync(int tvShowEpisodeId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var tvEpisodesResult = await CallMethodAsync<TvEpisodeResult>(ApiSynoVideoStation2TvShowEpisode, MethodGetInfo, new ReqParams
            {
                {"additional", Additional},
                {"id", "["+tvShowEpisodeId+"]"}
            }, cancellationToken).ConfigureAwait(false);
            if (!tvEpisodesResult.Success)
                throw new SynoRequestException(ApiSynoVideoStation2TvShowEpisode, MethodGetInfo, tvEpisodesResult.Error.Code);

            return tvEpisodesResult.Data;
        }

        #endregion

        #region Movie

        /// <summary>
        /// Get movies
        /// </summary>
        /// <param name="libraryId">Id of a Library. Library list can be retrieve by <see cref="LibraryListAsync"/> method. The built in libraries has 0 value such as built in Movies, TVShows, HomeVideos, TVRecordings. User added libraries has an own id value.</param>
        /// <param name="sortBy">Add sorting by <see cref="SortBy"/></param>
        /// <param name="sortDirection">Add sorting direction if <paramref name="sortBy"/> is not equals to <see cref="SortBy.None"/></param>
        /// <param name="offset">Skip the given number of elements.  It has take effect if the value is greater than 0.</param>
        /// <param name="limit">Limit the number of the retrieved elements. It has take effect if the value is greater or equal to 0.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="MoviesInfo"/>.</returns>
        /// <exception cref="SynoRequestException"> is throws on error</exception>
        public async Task<MoviesInfo> MovieList(int libraryId, SortBy sortBy = SortBy.None, SortDirection sortDirection = SortDirection.Ascending, int offset = 0, int limit = -1, CancellationToken cancellationToken = default(CancellationToken))
        {
            var movieResult = await CallMethodAsync<MovieResult>(ApiSynoVideoStationMovie, MethodList,
                new ReqParams
                {
                    {"library_id", libraryId.ToString()},
                    {"additional", Additional}
                }.SortBy(sortBy, sortDirection).Offset(offset).Limit(limit), cancellationToken).ConfigureAwait(false);
            if (!movieResult.Success)
                throw new SynoRequestException(ApiSynoVideoStationMovie, MethodList, movieResult.Error.Code);

            return movieResult.Data;
        }

        #endregion

        #region Library

        /// <summary>
        /// Get libraries.
        /// </summary>
        /// <param name="offset">Skip the given number of elements.  It has take effect if the value is greater than 0.</param>
        /// <param name="limit">Limit the number of the retrieved elements. It has take effect if the value is greater or equal to 0.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="LibrariesInfo"/>.</returns>
        /// <exception cref="SynoRequestException"> is throws on error</exception>
        public async Task<LibrariesInfo> LibraryListAsync(int offset = 0, int limit = -1, CancellationToken cancellationToken = default(CancellationToken))
        {
            var librariesResult =  await CallMethodAsync<LibrariesResult>(ApiSynoVideoStationLibrary, MethodList, new ReqParams().Offset(offset).Limit(limit), cancellationToken).ConfigureAwait(false);

            if (!librariesResult.Success)
                throw new SynoRequestException(ApiSynoVideoStationLibrary, MethodList, librariesResult.Error.Code);

            return librariesResult.Data;
        }

        #endregion

        #region Streaming

        //TODO: What shall be the value of audioTrackNumber if there is no any audio track? Suggested to be 0.
        /// <summary>
        /// Streaming the open asynchronous new.
        /// </summary>
        /// <param name="fileId">The file identifier.</param>
        /// <param name="audioTrackNumber">
        /// The audio track (index) number.
        /// Started at 1. Ignored when <paramref name="format"/> is <see cref="VideoTranscoding.Raw"/> and <paramref name="ac3PassThrough"/> is <c>true</c>.
        /// Assumed: 0 if there is no any audio track.
        /// </param>
        /// <param name="format">The format.</param>
        /// <param name="ac3PassThrough">if set to <c>true</c> [ac3 pass through].</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<VideoStreamResult> StreamingOpenAsync_new(int fileId, int audioTrackNumber, VideoTranscoding format = VideoTranscoding.Raw, bool ac3PassThrough = true,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var reqParams = new ReqParams();

            switch (format)
            {
                case VideoTranscoding.Raw:
                    if (ac3PassThrough || audioTrackNumber < 1)
                    {
                        reqParams.Add("raw", "{}");
                    }
                    else
                    {
                        reqParams.Add("hls_remux", string.Format("{{\"audio_track\":{0}}}", audioTrackNumber));
                    }
                    break;
                case VideoTranscoding.HighQuality:
                    //TODO: Implement this
                    break;
                case VideoTranscoding.MediumQuality:
                    //TODO: Implement this
                    break;
                case VideoTranscoding.LowQuality:
                    //TODO: Implement this
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }

            reqParams.Add("file", string.Format("{{\"id\":{0},\"path\":\"\"}}", fileId));

            var videoStreamResult = await CallMethodAsync<VideoStreamResult>(ApiSynoVideoStationStreaming, MethodOpen, reqParams, cancellationToken)
                .ConfigureAwait(false);

            if (!videoStreamResult.Success)
                throw new SynoRequestException(ApiSynoVideoStationStreaming, MethodOpen, videoStreamResult.Error.Code);

            return videoStreamResult;
        }

        public async Task<VideoStreamResult> StreamingOpenAsync(int fileId, string format = "raw", CancellationToken cancellationToken = default(CancellationToken))
        {
            if (format == null) throw new ArgumentNullException(nameof(format));
            if (string.IsNullOrEmpty(format)) throw new ArgumentException("format cannot be empty!", nameof(format));

            var videoStreamResult = await CallMethodAsync<VideoStreamResult>(ApiSynoVideoStationStreaming, MethodOpen, new ReqParams
            {
                {"id", fileId.ToString()},
                {"accept_format", format}
            }, cancellationToken).ConfigureAwait(false);

            if (!videoStreamResult.Success)
                throw new SynoRequestException(ApiSynoVideoStationStreaming, MethodOpen, videoStreamResult.Error.Code);

            return videoStreamResult;
        }

        public async Task StreamingCloseAsync(string streamId, bool forceClose, string format = "raw", CancellationToken cancellationToken = default(CancellationToken))
        {
            if (format == null) throw new ArgumentNullException(nameof(format));
            if (string.IsNullOrEmpty(format)) throw new ArgumentException("format cannot be empty!", nameof(format));

            var closeResult = await CallMethodAsync<TResult<object>>(ApiSynoVideoStationStreaming, MethodClose, new ReqParams
            {
                {"id", streamId},
                {"format", format},
                {"force_close", forceClose.ToString().ToLower()}
            }, cancellationToken).ConfigureAwait(false);

            if (!closeResult.Success)
                throw new SynoRequestException(ApiSynoVideoStationStreaming, MethodClose, closeResult.Error.Code);
        }


        //Todo: pass result to vlc
        public async Task<WebRequest> StreamingStreamAsync(string streamId, string format = "raw", CancellationToken cancellationToken = default(CancellationToken))
        {
            if (format == null) throw new ArgumentNullException(nameof(format));
            if (string.IsNullOrEmpty(format)) throw new ArgumentException("format cannot be empty!", nameof(format));

            return await GetWebRequestAsync(ApiSynoVideoStationStreaming, MethodStream, new ReqParams
            {
                {"id", streamId},
                {"format", format}
            }, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region Poster

        private static string MediaTypeToString(MediaType mediaType)
        {
            switch (mediaType)
            {
                case MediaType.Movie: return "movie";
                case MediaType.TvShow: return "tvshow";
                case MediaType.TvShowEpisode: return "tvshow_episode";
                default:
                    throw new ArgumentOutOfRangeException(nameof(mediaType), mediaType, "Unsupported MediaType. Supported Media types are: Movie, TVShow, TVShowEpisode");
            }
        }

        /// <summary>
        /// Create a <see cref="WebRequest"/> instance for download the poster image for a media.
        /// </summary>
        /// <param name="id">Id of the media whose poster image wants to be downloaded. <see cref="MetaDataItem.Id"/></param>
        /// <param name="mediaType">Select the of the media Movie, TVShow or TVShowEpisode</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="WebRequest"/> instance for download the poster image for a media.</returns>
        /// <exception cref="SynoRequestException"> is throws on error</exception>
        public async Task<WebRequest> PosterGetImageAsync(int id, MediaType mediaType, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await GetWebRequestAsync(ApiSynoVideoStationPoster, MethodGetImage, new ReqParams
            {
                {"id", id.ToString()},
                {"type", MediaTypeToString(mediaType)}
            }, cancellationToken);
        }

        #endregion

        #region Backdrop

        /// <summary>
        /// Create a <see cref="WebRequest"/> instance for download the backdrop image for a media.
        /// There is no backdrop image for episode parts of TVShow, please download the TVShow (collection) backdrop image instead.
        /// </summary>
        /// <param name="mapperId">MapperId of the media whose backdrop image wants to be downloaded. <see cref="MetaDataItem.MapperId"/></param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="WebRequest"/> instance for download the backdrop image for a media.</returns>
        /// <exception cref="SynoRequestException"> is throws on error</exception>
        public async Task<WebRequest> BackdropGetAsync(int mapperId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await GetWebRequestAsync(ApiSynoVideoStationBackdrop, MethodGet, new ReqParams
            {
                {"mapper_id", mapperId.ToString()},
            }, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region Subtitles

        /// <summary>
        /// List subtitles.
        /// </summary>
        public async Task<IEnumerable<Subtitle>> SubtitleListAsync(int fileId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var subtitlesResult = await CallMethodAsync<SubtitlesResult>(ApiSynoVideoStationSubtitle, MethodList,
                new ReqParams
                {
                    {"id", fileId.ToString()}
                }, cancellationToken).ConfigureAwait(false);

            if (!subtitlesResult.Success)
                throw new SynoRequestException(ApiSynoVideoStationSubtitle, MethodList, subtitlesResult.Error.Code);

            return subtitlesResult.Data;
        }

        /// <summary>
        /// Create a <see cref="WebRequest"/> instance for download the subtitle for a media.
        /// </summary>
        /// <param name="fileId">FileId</param>
        /// <param name="preview">Preview</param>
        /// <param name="subtitleId">SubtitleId</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="WebRequest"/> instance for download the subtitle for a media.</returns>
        /// <remarks>id does not matter if the requested subtitle is not embedded!</remarks>
        public async Task<WebRequest> SubtitleGetAsync(int fileId, bool preview, string subtitleId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (subtitleId == null) throw new ArgumentNullException(nameof(subtitleId));
            if (string.IsNullOrEmpty(subtitleId)) throw new ArgumentException("subtitleId cannot be empty!", nameof(subtitleId));

            return await GetWebRequestAsync(ApiSynoVideoStationSubtitle, MethodGet, new ReqParams
            {
                {"id", fileId.ToString()},
                {"preview", preview.ToString().ToLower()},
                {"subtitle_id", subtitleId},
            }, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region Audio track

        /// <summary>
        /// List audio tracks.
        /// </summary>
        public async Task<AudioTrackInfo> AudioTrackListAsync(int fileId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var audioTracksResult = await CallMethodAsync<AudioTrackResult>(ApiSynoVideoStationAudioTrack, MethodList,
                new ReqParams
                {
                    {"id", fileId.ToString()}
                }, cancellationToken).ConfigureAwait(false);

            if (!audioTracksResult.Success)
                throw new SynoRequestException(ApiSynoVideoStationAudioTrack, MethodList, audioTracksResult.Error.Code);

            return audioTracksResult.Data;
        }

        #endregion

        #region WatchStatus

        /// <summary>
        /// Gets watch status information for a media.
        /// </summary>
        /// <param name="id">Id of the media whose watch status information wants to be queried. <see cref="MetaDataItem.Id"/></param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="WatchStatusInfo"/> about watch status information</returns>
        /// <exception cref="SynoRequestException"> is throws on error</exception>
        public async Task<WatchStatusInfo> WatchStatusGetInfoAsync(int id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var watchStatusResult = await CallMethodAsync<WatchStatusResult>(ApiSynoVideoStationWatchStatus, MethodGetInfo,
                new ReqParams
                {
                    {"id", id.ToString()}
                }, cancellationToken).ConfigureAwait(false);

            if (!watchStatusResult.Success)
                throw new SynoRequestException(ApiSynoVideoStationWatchStatus, MethodGetInfo, watchStatusResult.Error.Code);

            return watchStatusResult.Data ?? new WatchStatusInfo { WatchStatus = new WatchStatus() };
        }

        /// <summary>
        /// Sets watch status information for a media.
        /// </summary>
        /// <param name="id">Id of the media whose poster image wants to be downloaded. <see cref="MetaDataItem.Id"/></param>
        /// <param name="position"></param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <exception cref="SynoRequestException"> is throws on error</exception>
        public async Task WatchStatusSetInfoAsync(int id, long position, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await CallMethodAsync<Result>(ApiSynoVideoStationWatchStatus, MethodSetInfo,
                new ReqParams
                {
                    {"id", id.ToString()},
                    {"position", position.ToString()}
                }, cancellationToken).ConfigureAwait(false);

            if (!result.Success)
                throw new SynoRequestException(ApiSynoVideoStationWatchStatus, MethodSetInfo, result.Error.Code);
        }


        #endregion
    }
}