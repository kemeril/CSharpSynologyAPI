using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using SynologyAPI.Exception;
using SynologyAPI.SynologyRestDAL.Vs;

namespace SynologyAPI
{
    public interface IVideoStation : IStation
    {
        /// <summary>
        /// List TV Shows
        /// </summary>
        /// <param name="libraryId">Id of a Library. Library list can be retrieve by <see cref="VideoStation.LibraryListAsync"/> method. The built in libraries has 0 value such as built in Movies, TVShows, HomeVideos, TVRecordings. User added libraries has an own id value.</param>
        /// <param name="sortBy">Add sorting by <see cref="VideoStation.SortBy"/></param>
        /// <param name="sortDirection">Add sorting direction if <paramref name="sortBy"/> is not equals to <see cref="VideoStation.SortBy.None"/></param>
        /// <param name="offset">Skip the given number of elements.  It has take effect if the value is greater than 0.</param>
        /// <param name="limit">Limit the number of the retrieved elements. It has take effect if the value is greater or equal to 0.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="TvShowsInfo"/>.</returns>
        /// <exception cref="SynoRequestException"> is throws on error</exception>
        Task<TvShowsInfo> TvShowListAsync(int libraryId, VideoStation.SortBy sortBy = VideoStation.SortBy.None, VideoStation.SortDirection sortDirection = VideoStation.SortDirection.Ascending, int offset = 0, int limit = -1, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get episodes of a TV Show
        /// </summary>
        /// <param name="libraryId">Id of a Library. Library list can be retrieve by <see cref="VideoStation.LibraryListAsync"/> method. The built in libraries has 0 value such as built in Movies, TVShows, HomeVideos, TVRecordings. User added libraries has an own id value.</param>
        /// <param name="tvShowId">Id of a TV Show</param>
        /// <param name="sortBy">Add sorting by <see cref="VideoStation.SortBy"/></param>
        /// <param name="sortDirection">Add sorting direction if <paramref name="sortBy"/> is not equals to <see cref="VideoStation.SortBy.None"/></param>
        /// <param name="offset">Skip the given number of elements.  It has take effect if the value is greater than 0.</param>
        /// <param name="limit">Limit the number of the retrieved elements. It has take effect if the value is greater or equal to 0.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="TvEpisodesInfo"/>.</returns>
        /// <exception cref="SynoRequestException"> is throws on error</exception>
        Task<TvEpisodesInfo> TvShowEpisodeListAsync(int libraryId, int tvShowId, VideoStation.SortBy sortBy = VideoStation.SortBy.None, VideoStation.SortDirection sortDirection = VideoStation.SortDirection.Ascending, int offset = 0, int limit = -1, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get info about an TV Show episode.
        /// </summary>
        /// <param name="tvShowEpisodeId"></param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="TvEpisodeInfo"/>.</returns>
        /// <exception cref="SynoRequestException"> is throws on error</exception>
        Task<TvEpisodeInfo> TvShowEpisodeGetInfoAsync(int tvShowEpisodeId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get movies
        /// </summary>
        /// <param name="libraryId">Id of a Library. Library list can be retrieve by <see cref="VideoStation.LibraryListAsync"/> method. The built in libraries has 0 value such as built in Movies, TVShows, HomeVideos, TVRecordings. User added libraries has an own id value.</param>
        /// <param name="sortBy">Add sorting by <see cref="VideoStation.SortBy"/></param>
        /// <param name="sortDirection">Add sorting direction if <paramref name="sortBy"/> is not equals to <see cref="VideoStation.SortBy.None"/></param>
        /// <param name="offset">Skip the given number of elements.  It has take effect if the value is greater than 0.</param>
        /// <param name="limit">Limit the number of the retrieved elements. It has take effect if the value is greater or equal to 0.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="MoviesInfo"/>.</returns>
        /// <exception cref="SynoRequestException"> is throws on error</exception>
        Task<MoviesInfo> MovieListAsync(int libraryId, VideoStation.SortBy sortBy = VideoStation.SortBy.None, VideoStation.SortDirection sortDirection = VideoStation.SortDirection.Ascending, int offset = 0, int limit = -1, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get Folders.
        /// </summary>
        /// <param name="libraryId">Id of a Library. Library list can be retrieve by <see cref="LibraryListAsync"/> method. The built in libraries has 0 value such as built in Movies, TVShows, HomeVideos, TVRecordings. User added libraries has an own id value.</param>
        /// <param name="libraryType"><see cref="LibraryType"/>, according the type of the <paramref name="libraryId"/></param>
        /// <param name="id">The id of the folder is requested. See <see cref="FileSystemObject.Id"/>. Optional. If not specified the root folder list is requested.</param>
        /// <param name="sortBy">Add sorting by <see cref="VideoStation.SortBy"/></param>
        /// <param name="sortDirection">Add sorting direction if <paramref name="sortBy"/> is not equals to <see cref="VideoStation.SortBy.None"/></param>
        /// <param name="offset">Skip the given number of elements.  It has take effect if the value is greater than 0.</param>
        /// <param name="limit">Limit the number of the retrieved elements. It has take effect if the value is greater or equal to 0.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="MoviesInfo"/>.</returns>
        /// <exception cref="SynoRequestException"> is thrown on error</exception>
        Task<FolderInfo> FolderListAsync(int libraryId, string id, LibraryType libraryType, VideoStation.SortBy sortBy = VideoStation.SortBy.None, VideoStation.SortDirection sortDirection = VideoStation.SortDirection.Ascending, int offset = 0, int limit = -1, CancellationToken cancellationToken = default);


        /// <summary>
        /// Get libraries.
        /// </summary>
        /// <param name="offset">Skip the given number of elements.  It has take effect if the value is greater than 0.</param>
        /// <param name="limit">Limit the number of the retrieved elements. It has take effect if the value is greater or equal to 0.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="LibrariesInfo"/>.</returns>
        /// <exception cref="SynoRequestException"> is throws on error</exception>
        Task<LibrariesInfo> LibraryListAsync(int offset = 0, int limit = -1, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronous open a video stream.
        /// By the default parameter values the method opens the video stream in <see cref="VideoStation.VideoTranscoding.Raw"/> format and with AC3PassThrough audio format with all the audio tracks.
        /// </summary>
        /// <param name="fileId">The file identifier.</param>
        /// <param name="audioTrackId">
        /// The audio track id.
        /// Ignored when <paramref name="format"/> is <see cref="VideoStation.VideoTranscoding.Raw"/> and <paramref name="ac3PassThrough"/> is <c>true</c>.
        /// 0 if there is no any audio track.
        /// </param>
        /// <param name="format">The format. See <see cref="VideoStation.VideoTranscoding"/>.</param>
        /// <param name="ac3PassThrough">if set to <c>true</c> [ac3 audio pass through].</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <exception cref="SynoRequestException">
        /// Opening error on the server side.
        /// There is a special error code: 1204. It means that the stream tried to open in non raw format but the server not supported this mode for the video by the possible reason the applied video or audio codec cannot be transcoded or remuxed. In this case the video shall be opened in raw format with AC3PassThrough settings.
        /// </exception>
        Task<VideoStreamResult> StreamingOpenAsync(int fileId, int audioTrackId = 0, VideoStation.VideoTranscoding format = VideoStation.VideoTranscoding.Raw, bool ac3PassThrough = true, CancellationToken cancellationToken = default);

        Task<VideoStreamResult> StreamingOpenAsync(int fileId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronous close am opened stream.
        /// </summary>
        /// <param name="streamId">The stream identifier, returned by <see><cref>StreamingOpenAsync</cref></see> method.</param>
        /// <param name="forceClose">if set to <c>true</c> [force close].</param>
        /// <param name="format">The format. Valid values: "raw", "hls_remux", "hls"</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">format cannot be null!</exception>
        /// <exception cref="ArgumentException">format cannot be empty! - format</exception>
        /// <exception cref="ArgumentOutOfRangeException">Invalid format value. Allowed values are: "raw", "hls_remux", "hls" - format</exception>
        /// <exception cref="SynoRequestException">Closing error on the server side.</exception>
        Task StreamingCloseAsync(string streamId, bool forceClose = true, string format = "raw", CancellationToken cancellationToken = default);

        Task<WebRequest> StreamingStreamAsync(string streamId, string format = "raw", CancellationToken cancellationToken = default);

        /// <summary>
        /// Create a <see cref="WebRequest"/> instance for download the poster image for a media.
        /// </summary>
        /// <param name="id">Id of the media whose poster image wants to be downloaded. <see cref="MetaDataItem.Id"/></param>
        /// <param name="mediaType">Select the of the media Movie, TVShow or TVShowEpisode.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="WebRequest"/> instance for download the poster image for a media.</returns>
        /// <exception cref="SynoRequestException"> is throws on error.</exception>
        Task<WebRequest> PosterGetImageAsync(int id, MediaType mediaType, CancellationToken cancellationToken = default);

        /// <summary>
        /// Create a <see cref="WebRequest"/> instance for download the backdrop image for a media.
        /// There is no backdrop image for episode parts of TVShow, please download the TVShow (collection) backdrop image instead.
        /// </summary>
        /// <param name="mapperId">MapperId of the media whose backdrop image wants to be downloaded. <see cref="MetaDataItem.MapperId"/></param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="WebRequest"/> instance for download the backdrop image for a media.</returns>
        /// <exception cref="SynoRequestException"> is throws on error.</exception>
        Task<WebRequest> BackdropGetAsync(int mapperId, CancellationToken cancellationToken = default);

        /// <summary>
        /// List subtitles.
        /// </summary>
        Task<IEnumerable<Subtitle>> SubtitleListAsync(int fileId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Create a <see cref="WebRequest"/> instance for download the subtitle for a media.
        /// </summary>
        /// <param name="fileId">FileId</param>
        /// <param name="preview">Preview</param>
        /// <param name="subtitleId">SubtitleId</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="WebRequest"/> instance for download the subtitle for a media.</returns>
        /// <remarks>id does not matter if the requested subtitle is not embedded!</remarks>
        Task<WebRequest> SubtitleGetAsync(int fileId, bool preview, string subtitleId, CancellationToken cancellationToken = default);

        /// <summary>
        /// List audio tracks.
        /// </summary>
        Task<AudioTrackInfo> AudioTrackListAsync(int fileId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets watch status information for a media.
        /// </summary>
        /// <param name="id">Id of the media whose watch status information wants to be queried. <see cref="MetaDataItem.Id"/></param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="WatchStatusInfo"/> about watch status information</returns>
        /// <exception cref="SynoRequestException"> is throws on error</exception>
        Task<WatchStatusInfo> WatchStatusGetInfoAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets watch status information for a media.
        /// </summary>
        /// <param name="id">Id of the media whose poster image wants to be downloaded. <see cref="MetaDataItem.Id"/></param>
        /// <param name="position"></param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <exception cref="SynoRequestException"> is throws on error</exception>
        Task WatchStatusSetInfoAsync(int id, long position, CancellationToken cancellationToken = default);
    }
}
