using SynologyAPI.Exception;
using SynologyRestDAL;
using SynologyRestDAL.Vs;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

//https://github.com/kwent/syno/wiki/Video-Station-API

namespace SynologyAPI
{
    public sealed class VideoStation : Station
    {
        private const string ApiSynoVideoStationTvShow = "SYNO.VideoStation.TVShow";
        private const string ApiSynoVideoStationStreaming = "SYNO.VideoStation.Streaming";
        private const string ApiSynoVideoStationTvShowEpisode = "SYNO.VideoStation.TVShowEpisode";
        private const string ApiSynoVideoStation2TvShowEpisode = "SYNO.VideoStation2.TVShowEpisode";
        private const string ApiSynoVideoStationMovie = "SYNO.VideoStation.Movie";
        private const string ApiSynoVideoStationLibrary = "SYNO.VideoStation.Library";
        private const string ApiSynoVideoStationPoster = "SYNO.VideoStation.Poster";
        private const string ApiSynoVideoStationBackdrop = "SYNO.VideoStation.Backdrop";
        private const string ApiSynoVideoStationSubtitle = "SYNO.VideoStation.Subtitle";
        private const string Additional = @"[""summary"",""actor"",""file"",""extra"",""genre"",""writer"",""director"",""collection"",""poster_mtime"",""watched_ratio"",""conversion_produced"",""backdrop_mtime"",""parental_control""]";


        #region Boilerplate

        public VideoStation()
        {
            InternalSession = "VideoStation";
        }

        public VideoStation(Uri url, string username, string password, IWebProxy proxy)
            : base(url, username, password, proxy)
        {
            InternalSession = "VideoStation";
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
            implementedApi.Add("SYNO.VideoStation.AudioTrack", 1);
            implementedApi.Add("SYNO.VideoStation.Folder", 1);
            implementedApi.Add("SYNO.VideoStation.WatchStatus", 1);

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
            TVShow,
            TVShowEpisode
        }

        #endregion

        #region TvShow

        /// <summary>
        /// List TV Shows
        /// </summary>
        /// <param name="libraryId">Id of a Library. Library list can be retrieve by <see cref="LibraryList"/> method. The built in libraries has 0 value such as built in Movies, TVShows, HomeVideos, TVRecordings. User added libraries has an own id value.</param>
        /// <param name="sortBy">Add sorting by <see cref="SortBy"/></param>
        /// <param name="sortDirection">Add sorting direction if <paramref name="sortBy"/> is not equals to <see cref="SortBy.None"/></param>
        /// <param name="offset">Skip the given number of elements.  It has take effect if the value is greater than 0.</param>
        /// <param name="limit">Limit the number of the retrieved elements. It has take effect if the value is greater or equal to 0.</param>
        /// <returns></returns>
        public async Task<TvShowsInfo> TvShowList(int libraryId, SortBy sortBy = SortBy.None, SortDirection sortDirection = SortDirection.Ascending, int offset = 0, int limit = -1)
        {
            var tvShowsResult = await CallMethod<TvShowsResult>(ApiSynoVideoStationTvShow, "list",
                new ReqParams
                {
                    {"library_id", libraryId.ToString()},
                    {"offset", "0"},
                    {"additional", Additional}
                }.SortBy(sortBy, sortDirection).Offset(offset).Limit(limit));

            if (!tvShowsResult.Success)
                throw new SynoRequestException(@"Synology error code " + tvShowsResult.Error);

            return tvShowsResult.Data;
        }

        /// <summary>
        /// Get episodes of a TV Show
        /// </summary>
        /// <param name="libraryId">Id of a Library. Library list can be retrieve by <see cref="LibraryList"/> method. The built in libraries has 0 value such as built in Movies, TVShows, HomeVideos, TVRecordings. User added libraries has an own id value.</param>
        /// <param name="tvShowId">Id of a TV Show</param>
        /// <param name="sortBy">Add sorting by <see cref="SortBy"/></param>
        /// <param name="sortDirection">Add sorting direction if <paramref name="sortBy"/> is not equals to <see cref="SortBy.None"/></param>
        /// <param name="offset">Skip the given number of elements.  It has take effect if the value is greater than 0.</param>
        /// <param name="limit">Limit the number of the retrieved elements. It has take effect if the value is greater or equal to 0.</param>
        /// <returns></returns>
        public async Task<TvEpisodesInfo> TvShowEpisodeList(int libraryId, int tvShowId, SortBy sortBy = SortBy.None, SortDirection sortDirection = SortDirection.Ascending, int offset = 0, int limit = -1)
        {
            var tvEpisodesResult = await CallMethod<TvEpisodesResult>(ApiSynoVideoStationTvShowEpisode, "list",
                    new ReqParams
                    {
                        {"library_id", libraryId.ToString()},
                        {"tvshow_id", tvShowId.ToString()},
                        {"additional", Additional}
                    }.SortBy(sortBy, sortDirection).Offset(offset).Limit(limit));
            if (!tvEpisodesResult.Success)
                throw new SynoRequestException(@"Synology error code " + tvEpisodesResult.Error);

            return tvEpisodesResult.Data;
        }

        /// <summary>
        /// Get info about an TV Show episode.
        /// </summary>
        /// <param name="tvShowEpisodeId"></param>
        /// <returns></returns>
        public async Task<TvEpisodeInfo> TvShowEpisodeGetInfo(int tvShowEpisodeId)
        {
            var tvEpisodesResult = await CallMethod<TvEpisodeResult>(ApiSynoVideoStation2TvShowEpisode, "getinfo", new ReqParams
            {
                {"additional", Additional},
                {"id", "["+tvShowEpisodeId+"]"}
            });
            if (!tvEpisodesResult.Success)
                throw new SynoRequestException(@"Synology error code " + tvEpisodesResult.Error);

            return tvEpisodesResult.Data;
        }

        #endregion

        #region Movie

        /// <summary>
        /// Get movies
        /// </summary>
        /// <param name="libraryId">Id of a Library. Library list can be retrieve by <see cref="LibraryList"/> method. The built in libraries has 0 value such as built in Movies, TVShows, HomeVideos, TVRecordings. User added libraries has an own id value.</param>
        /// <param name="sortBy">Add sorting by <see cref="SortBy"/></param>
        /// <param name="sortDirection">Add sorting direction if <paramref name="sortBy"/> is not equals to <see cref="SortBy.None"/></param>
        /// <param name="offset">Skip the given number of elements.  It has take effect if the value is greater than 0.</param>
        /// <param name="limit">Limit the number of the retrieved elements. It has take effect if the value is greater or equal to 0.</param>
        /// <returns></returns>
        public async Task<MoviesInfo> MovieList(int libraryId, SortBy sortBy = SortBy.None, SortDirection sortDirection = SortDirection.Ascending, int offset = 0, int limit = -1)
        {
            var movieResult = await CallMethod<MovieResult>(ApiSynoVideoStationMovie, "list",
                new ReqParams
                {
                    {"library_id", libraryId.ToString()},
                    {"additional", Additional}
                }.SortBy(sortBy, sortDirection).Offset(offset).Limit(limit));
            if (!movieResult.Success)
                throw new SynoRequestException(@"Synology error code " + movieResult.Error);

            return movieResult.Data;
        }

        #endregion

        #region Library

        /// <summary>
        /// Get libraries.
        /// </summary>
        /// <param name="offset">Skip the given number of elements.  It has take effect if the value is greater than 0.</param>
        /// <param name="limit">Limit the number of the retrieved elements. It has take effect if the value is greater or equal to 0.</param>
        /// <returns></returns>
        public async Task<LibrariesInfo> LibraryList(int offset = 0, int limit = -1)
        {
            var librariesResult =  await CallMethod<LibrariesResult>(ApiSynoVideoStationLibrary, "list", new ReqParams().Offset(offset).Limit(limit));

            if (!librariesResult.Success)
                throw new SynoRequestException(@"Synology error code " + librariesResult.Error);

            return librariesResult.Data;
        }

        #endregion

        #region Streaming

        public async Task<VideoStreamResult> StreamingOpen(int fileId, string format = "raw")
        {
            if (format == null) throw new ArgumentNullException(nameof(format));
            if (string.IsNullOrEmpty(format)) throw new ArgumentException("format cannot be empty!", nameof(format));

            return await CallMethod<VideoStreamResult>(ApiSynoVideoStationStreaming, "open", new ReqParams
            {
                {"id", fileId.ToString()},
                {"accept_format", format}
            });
        }

        public async Task<bool> StreamingClose(string streamId, bool forceClose, string format = "raw")
        {
            if (format == null) throw new ArgumentNullException(nameof(format));
            if (string.IsNullOrEmpty(format)) throw new ArgumentException("format cannot be empty!", nameof(format));

            var closeResult = await CallMethod<TResult<object>>(ApiSynoVideoStationStreaming, "close", new ReqParams
            {
                {"id", streamId},
                {"format", format},
                {"force_close", forceClose.ToString().ToLower()}
            });
            return closeResult.Success;
        }


        //Todo: pass result to vlc
        public async Task<WebRequest> StreamingStream(string streamId, string format = "raw")
        {
            if (format == null) throw new ArgumentNullException(nameof(format));
            if (string.IsNullOrEmpty(format)) throw new ArgumentException("format cannot be empty!", nameof(format));

            return await GetWebRequest(ApiSynoVideoStationStreaming, "stream", new ReqParams
            {
                {"id", streamId},
                {"format", format}
            });
        }

        #endregion

        #region Poster

        private static string MediaTypeToString(MediaType mediaType)
        {
            switch (mediaType)
            {
                case MediaType.Movie: return "movie";
                case MediaType.TVShow: return "tvshow";
                case MediaType.TVShowEpisode: return "tvshow_episode";
                default:
                    throw new ArgumentOutOfRangeException(nameof(mediaType), mediaType, "Unsupported MediaType. Supported Media types are: Movie, TVShow, TVShowEpisode");
            }
        }

        /// <summary>
        /// Create a <see cref="WebRequest"/> instance for download the poster image for a media.
        /// </summary>
        /// <param name="id">Id of the media whose poster image wants to be downloaded. <see cref="MetaDataItem.Id"/></param>
        /// <param name="mediaType">Select the of the media Movie, TVShow or TVShowEpisode</param>
        /// <returns>The <see cref="WebRequest"/> instance for download the poster image for a media.</returns>
        public async Task<WebRequest> PosterGetImage(int id, MediaType mediaType)
        {
            return await GetWebRequest(ApiSynoVideoStationPoster, "getimage", new ReqParams
            {
                {"id", id.ToString()},
                {"type", MediaTypeToString(mediaType)}
            });
        }

        #endregion

        #region Backdrop

        /// <summary>
        /// Create a <see cref="WebRequest"/> instance for download the backdrop image for a media.
        /// There is no backdrop image for episode parts of TVShow, please download the TVShow (collection) backdrop image instead.
        /// </summary>
        /// <param name="mapperId">MapperId of the media whose backdrop image wants to be downloaded. <see cref="MetaDataItem.MapperId"/></param>
        /// <returns>The <see cref="WebRequest"/> instance for download the backdrop image for a media.</returns>
        public async Task<WebRequest> BackdropGet(int mapperId)
        {
            return await GetWebRequest(ApiSynoVideoStationBackdrop, "get", new ReqParams
            {
                {"mapper_id", mapperId.ToString()},
            });
        }

        #endregion

        #region Subtitles

        /// <summary>
        /// List subtitles.
        /// </summary>
        public async Task<IEnumerable<Subtitle>> SubtitleList(int fileId)
        {
            var subtitlesResult = await CallMethod<SubtitlesResult>(ApiSynoVideoStationSubtitle, "list",
                new ReqParams
                {
                    {"id", fileId.ToString()}
                });

            if (!subtitlesResult.Success)
                throw new SynoRequestException(@"Synology error code " + subtitlesResult.Error);

            return subtitlesResult.Data;
        }

        /// <summary>
        /// Create a <see cref="WebRequest"/> instance for download the subtitle for a media.
        /// </summary>
        /// <returns>The <see cref="WebRequest"/> instance for download the subtitle for a media.</returns>
        /// <remarks>id does not matter if the requested subtitle is not embedded!</remarks>
        public async Task<WebRequest> SubtitleGet(int fileId, bool preview, string subtitleId)
        {
            if (subtitleId == null) throw new ArgumentNullException(nameof(subtitleId));
            if (string.IsNullOrEmpty(subtitleId)) throw new ArgumentException("subtitleId cannot be empty!", nameof(subtitleId));

            return await GetWebRequest(ApiSynoVideoStationSubtitle, "get", new ReqParams
            {
                {"id", fileId.ToString()},
                {"preview", preview.ToString().ToLower()},
                {"subtitle_id", subtitleId},
            });
        }

        #endregion
    }
}