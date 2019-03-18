﻿using SynologyAPI.Exception;
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
            TvShow,
            TvShowEpisode
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
        /// <returns></returns>
        public async Task<TvShowsInfo> TvShowListAsync(int libraryId, SortBy sortBy = SortBy.None, SortDirection sortDirection = SortDirection.Ascending, int offset = 0, int limit = -1, CancellationToken cancellationToken = default(CancellationToken))
        {
            var tvShowsResult = await CallMethodAsync<TvShowsResult>(ApiSynoVideoStationTvShow, "list",
                new ReqParams
                {
                    {"library_id", libraryId.ToString()},
                    {"offset", "0"},
                    {"additional", Additional}
                }.SortBy(sortBy, sortDirection).Offset(offset).Limit(limit), cancellationToken).ConfigureAwait(false);

            if (!tvShowsResult.Success)
                throw new SynoRequestException(@"Synology error code " + tvShowsResult.Error);

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
        /// <returns></returns>
        public async Task<TvEpisodesInfo> TvShowEpisodeListAsync(int libraryId, int tvShowId, SortBy sortBy = SortBy.None, SortDirection sortDirection = SortDirection.Ascending, int offset = 0, int limit = -1, CancellationToken cancellationToken = default(CancellationToken))
        {
            var tvEpisodesResult = await CallMethodAsync<TvEpisodesResult>(ApiSynoVideoStationTvShowEpisode, "list",
                    new ReqParams
                    {
                        {"library_id", libraryId.ToString()},
                        {"tvshow_id", tvShowId.ToString()},
                        {"additional", Additional}
                    }.SortBy(sortBy, sortDirection).Offset(offset).Limit(limit), cancellationToken).ConfigureAwait(false);
            if (!tvEpisodesResult.Success)
                throw new SynoRequestException(@"Synology error code " + tvEpisodesResult.Error);

            return tvEpisodesResult.Data;
        }

        /// <summary>
        /// Get info about an TV Show episode.
        /// </summary>
        /// <param name="tvShowEpisodeId"></param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns></returns>
        public async Task<TvEpisodeInfo> TvShowEpisodeGetInfoAsync(int tvShowEpisodeId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var tvEpisodesResult = await CallMethodAsync<TvEpisodeResult>(ApiSynoVideoStation2TvShowEpisode, "getinfo", new ReqParams
            {
                {"additional", Additional},
                {"id", "["+tvShowEpisodeId+"]"}
            }, cancellationToken).ConfigureAwait(false);
            if (!tvEpisodesResult.Success)
                throw new SynoRequestException(@"Synology error code " + tvEpisodesResult.Error);

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
        /// <returns></returns>
        public async Task<MoviesInfo> MovieList(int libraryId, SortBy sortBy = SortBy.None, SortDirection sortDirection = SortDirection.Ascending, int offset = 0, int limit = -1, CancellationToken cancellationToken = default(CancellationToken))
        {
            var movieResult = await CallMethodAsync<MovieResult>(ApiSynoVideoStationMovie, "list",
                new ReqParams
                {
                    {"library_id", libraryId.ToString()},
                    {"additional", Additional}
                }.SortBy(sortBy, sortDirection).Offset(offset).Limit(limit), cancellationToken).ConfigureAwait(false);
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
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns></returns>
        public async Task<LibrariesInfo> LibraryListAsync(int offset = 0, int limit = -1, CancellationToken cancellationToken = default(CancellationToken))
        {
            var librariesResult =  await CallMethodAsync<LibrariesResult>(ApiSynoVideoStationLibrary, "list", new ReqParams().Offset(offset).Limit(limit), cancellationToken).ConfigureAwait(false);

            if (!librariesResult.Success)
                throw new SynoRequestException(@"Synology error code " + librariesResult.Error);

            return librariesResult.Data;
        }

        #endregion

        #region Streaming

        public async Task<VideoStreamResult> StreamingOpenAsync(int fileId, string format = "raw", CancellationToken cancellationToken = default(CancellationToken))
        {
            if (format == null) throw new ArgumentNullException(nameof(format));
            if (string.IsNullOrEmpty(format)) throw new ArgumentException("format cannot be empty!", nameof(format));

            return await CallMethodAsync<VideoStreamResult>(ApiSynoVideoStationStreaming, "open", new ReqParams
            {
                {"id", fileId.ToString()},
                {"accept_format", format}
            }, cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> StreamingCloseAsync(string streamId, bool forceClose, string format = "raw", CancellationToken cancellationToken = default(CancellationToken))
        {
            if (format == null) throw new ArgumentNullException(nameof(format));
            if (string.IsNullOrEmpty(format)) throw new ArgumentException("format cannot be empty!", nameof(format));

            var closeResult = await CallMethodAsync<TResult<object>>(ApiSynoVideoStationStreaming, "close", new ReqParams
            {
                {"id", streamId},
                {"format", format},
                {"force_close", forceClose.ToString().ToLower()}
            }, cancellationToken).ConfigureAwait(false);
            return closeResult.Success;
        }


        //Todo: pass result to vlc
        public async Task<WebRequest> StreamingStreamAsync(string streamId, string format = "raw", CancellationToken cancellationToken = default(CancellationToken))
        {
            if (format == null) throw new ArgumentNullException(nameof(format));
            if (string.IsNullOrEmpty(format)) throw new ArgumentException("format cannot be empty!", nameof(format));

            return await GetWebRequestAsync(ApiSynoVideoStationStreaming, "stream", new ReqParams
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
        public async Task<WebRequest> PosterGetImageAsync(int id, MediaType mediaType, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await GetWebRequestAsync(ApiSynoVideoStationPoster, "getimage", new ReqParams
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
        public async Task<WebRequest> BackdropGetAsync(int mapperId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await GetWebRequestAsync(ApiSynoVideoStationBackdrop, "get", new ReqParams
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
            var subtitlesResult = await CallMethodAsync<SubtitlesResult>(ApiSynoVideoStationSubtitle, "list",
                new ReqParams
                {
                    {"id", fileId.ToString()}
                }, cancellationToken).ConfigureAwait(false);

            if (!subtitlesResult.Success)
                throw new SynoRequestException(@"Synology error code " + subtitlesResult.Error);

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

            return await GetWebRequestAsync(ApiSynoVideoStationSubtitle, "get", new ReqParams
            {
                {"id", fileId.ToString()},
                {"preview", preview.ToString().ToLower()},
                {"subtitle_id", subtitleId},
            }, cancellationToken).ConfigureAwait(false);
        }

        #endregion
    }
}