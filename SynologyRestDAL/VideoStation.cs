using StdUtils;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SynologyRestDAL
{
    namespace Vs
    {
        [DataContract]
        public class LibrariesResult : TResult<LibrariesInfo>
        {
        }

        [DataContract]
        public class TvShowsResult : TResult<TvShowsInfo>
        {
        }

        [DataContract]
        public class TvEpisodesResult : TResult<TvEpisodesInfo>
        {
        }

        [DataContract]
        public class MovieResult : TResult<MoviesInfo>
        {
        }

        [DataContract]
        public class TvEpisodeResult : TResult<TvEpisodeInfo>
        {
        }

        [DataContract]
        public class Info
        {
            [DataMember(Name = "total")]
            public int Total { get; set; }

            [DataMember(Name = "offset")]
            public int Offset { get; set; }
        }

        [DataContract]
        public class VideoStreamResult : TResult<VideoStreamInfo>
        {
        }

        [DataContract]
        public class LibrariesInfo : Info
        {
            [DataMember(Name = "libraries")]
            public IEnumerable<Library> Libraries { get; set; }
        }

        [DataContract]
        public class TvShowsInfo : Info
        {
            [DataMember(Name = "tvshows")]
            public IEnumerable<TvShow> TvShows { get; set; }
        }

        [DataContract]
        public class TvEpisodesInfo : Info
        {
            [DataMember(Name = "episodes")]
            public IEnumerable<TvEpisode> Episodes { get; set; }
        }

        [DataContract]
        public class TvEpisodeInfo : Info
        {
            [DataMember(Name = "episode")]
            public IEnumerable<TvEpisode> Episodes { get; set; }
        }

        [DataContract]
        public class MoviesInfo : Info
        {
            [DataMember(Name = "movies")]
            public IEnumerable<Movie> Movies { get; set; }
        }

        [DataContract]
        public abstract class MetaDataItem
        {
            [DataMember(Name = "certificate")]
            public string Certificate { get; set; }

            [DataMember(Name = "sort_title")]
            public string SortTitle { get; set; }

            [DataMember(Name = "title")]
            public string Title { get; set; }

            [DataMember(Name = "metadata_locked")]
            public bool MetadataLocked { get; set; }

            [DataMember(Name = "id")]
            public int Id { get; set; }

            [DataMember(Name = "library_id")]
            public int LibraryId { get; set; }

            [DataMember(Name = "mapper_id")]
            public int MapperId { get; set; }

            public DateTime? OriginalAvailable { get; private set; }

            [DataMember(Name = "original_available")]
            private string OriginalAvailableSetter
            {
                set
                {
                    var dateParts = value.Split('-');
                    if (dateParts.Length < 3) return;
                    OriginalAvailable = new DateTime(Int32.Parse(dateParts[0]), Int32.Parse(dateParts[1]),
                        Int32.Parse(dateParts[2]));
                }
                get
                {
                    return OriginalAvailable == null
                        ? null
                        : string.Format("{0}-{1}-{2}", OriginalAvailable.Value.Year, OriginalAvailable.Value.Month,
                            OriginalAvailable.Value.Day);
                }
            }

            [DataMember(Name = "rating")]
            public int Rating { get; set; }

            [DataMember(Name = "tagline")]
            public string Tagline { get; set; }

            [DataMember(Name = "create_time")]
            public int CreateTime { get; set; }

            [DataMember(Name = "last_watched")]
            public int LastWatched { get; set; }

            [DataMember(Name = "additional")]
            public Additional Additional { get; private set; }

            public override string ToString()
            {
                return
                    string.Format(
                        "SortTitle: {0}, Title: {1}, MetadataLocked: {2}, Id: {3}, MapperId: {4}, OriginalAvailable: {5}, OriginalAvailableSetter: {6}",
                        SortTitle, Title, MetadataLocked, Id, MapperId, OriginalAvailable, OriginalAvailableSetter);
            }
        }

        [DataContract]
        public class TvShow : MetaDataItem
        {

        }

        [DataContract]
        public class File
        {
            ///// <summary>
            ///// Sample value:640000
            ///// </summary>
            [DataMember(Name = "audio_bitrate")]
            public int AudioBitrate { get; set; }

            ///// <summary>
            ///// Sample value:"eac3"
            ///// </summary>
            [DataMember(Name = "audio_codec")]
            public string AudioCodec { get; set; }

            ///// <summary>
            ///// Sample value:6
            ///// </summary>
            [DataMember(Name = "channel")]
            public int Channel { get; set; }

            ///// <summary>
            ///// Sample value:"matroska,webm"
            ///// </summary>
            [DataMember(Name = "container_type")]
            public string ContainerType { get; set; }

            ///// <summary>
            ///// Sample value:false
            ///// </summary>
            [DataMember(Name = "conversion_produced")]
            public bool ConversionProduced { get; set; }

            ///// <summary>
            ///// Sample value:1280
            ///// </summary>
            [DataMember(Name = "display_x")]
            public int DisplayX { get; set; }

            ///// <summary>
            ///// Sample value:720
            ///// </summary>
            [DataMember(Name = "display_y")]
            public int DisplayY { get; set; }

            ///// <summary>
            ///// Sample value:"1:11:05"
            ///// </summary>
            [DataMember(Name = "duration")]
            // ReSharper disable once InconsistentNaming
            private string _duration { get; set; }

            public TimeSpan Duration
            {
                get
                {
                    return TimeSpanParser.Parse(_duration).Duration();
                }
            }

            ///// <summary>
            ///// Sample value:100
            ///// </summary>
            [DataMember(Name = "ff_video_profile")]
            // ReSharper disable once InconsistentNaming
            public int FFVideoProfile { get; set; }

            /// <summary>
            /// Sample value:4908079893
            /// </summary>
            [DataMember(Name = "filesize")]
            public long FileSize { get; set; }

            ///// <summary>
            ///// Sample value:9205986
            ///// </summary>
            [DataMember(Name = "frame_bitrate")]
            public int FrameBitrate { get; set; }

            ///// <summary>
            ///// Sample value:1001
            ///// </summary>
            [DataMember(Name = "frame_rate_den")]
            public int FrameRateDen { get; set; }

            ///// <summary>
            ///// Sample value:24000
            ///// </summary>
            [DataMember(Name = "frame_rate_num")]
            public int FrameRateNum { get; set; }

            ///// <summary>
            ///// Sample value:48000
            ///// </summary>
            [DataMember(Name = "frequency")]
            public int Frequency { get; set; }

            /// <summary>
            /// Sample value:22
            /// </summary>
            [DataMember(Name = "id")]
            public int Id { get; set; }

            /// <summary>
            /// Sample value:"/volume1/video/TV_Show/The.Grand.Tour/The.Grand.Tour.S01.720p.WEBRip.X264-DEFLATE/The.Grand.Tour_S01E01.mkv"
            /// </summary>
            [DataMember(Name = "path")]
            public string Path { get; set; }

            ///// <summary>
            ///// Sample value:4265
            ///// </summary>
            [DataMember(Name = "position")]
            public long Position { get; set; }

            ///// <summary>
            ///// Sample value:1280
            ///// </summary>
            [DataMember(Name = "resolutionx")]
            public int ResolutionX { get; set; }

            ///// <summary>
            ///// Sample value:720
            ///// </summary>
            [DataMember(Name = "resolutiony")]
            public int ResolutionY { get; set; }

            ///// <summary>
            ///// Sample value:0
            ///// </summary>
            [DataMember(Name = "rotation")]
            public int Rotation { get; set; }

            /// <summary>
            /// Sample value:"/video/TV_Show/The.Grand.Tour/The.Grand.Tour.S01.720p.WEBRip.X264-DEFLATE/The.Grand.Tour_S01E01.mkv"
            /// </summary>
            [DataMember(Name = "sharepath")]
            public string Sharepath { get; set; }

            ///// <summary>
            ///// Sample value:0
            ///// </summary>
            [DataMember(Name = "video_bitrate")]
            public int VideoBitrate { get; set; }

            ///// <summary>
            ///// Sample value:"h264"
            ///// </summary>
            [DataMember(Name = "video_codec")]
            public string VideoCodec { get; set; }

            ///// <summary>
            ///// Sample value:41
            ///// </summary>
            [DataMember(Name = "video_level")]
            public int VideoLevel { get; set; }

            ///// <summary>
            ///// Sample value:3
            ///// </summary>
            [DataMember(Name = "video_profile")]
            public int VideoProfile { get; set; }

            ///// <summary>
            ///// Sample value:0.3623693379790941
            ///// </summary>
            [DataMember(Name = "watched_ratio")]
            public decimal WatchedRatio { get; set; }

            public override string ToString()
            {
                return string.Format("{{ Id: {0}, Path: {1} }}", Id, Path ?? Sharepath ?? string.Empty);
            }
        }

        [DataContract]
        public class TvEpisode : MetaDataItem
        {
            /// <summary>
            /// Sample: 1, the order of the episode in a season
            /// </summary>
            [DataMember(Name = "episode")]
            public int Episode { get; set; }

            /// <summary>
            /// Sample: 1, the order rof the season
            /// </summary>
            [DataMember(Name = "season")]
            public int Season { get; set; }

            [DataMember(Name = "tvshow_id")]
            internal int TvshowId { get; set; }

            public override string ToString()
            {
                return
                    string.Format(
                        "{{ Episode: {1}, LastWatched: {2}, Season: {3}, Tagline: {4}, Summary: {5}, TvshowId: {{ {0} }} }}",
                        TvshowId, Episode, LastWatched, Season, Tagline, Additional.Summary ?? string.Empty);
            }
        }

        [DataContract]
        public class Movie : MetaDataItem
        {
        }

        [DataContract]
        public class Additional
        {
            [DataMember(Name = "summary")]
            public string Summary { get; private set; }

            /// <summary>
            /// Null on <see cref="TvShow"/>.
            /// </summary>
            [DataMember(Name = "actor")]
            public IEnumerable<string> Actors { get; private set; }

            /// <summary>
            /// Null on <see cref="TvShow"/>.
            /// </summary>
            [DataMember(Name = "director")]
            public IEnumerable<string> Directors { get; private set; }

            /// <summary>
            /// Null on <see cref="TvShow"/>.
            /// </summary>
            [DataMember(Name = "genre")]
            public IEnumerable<string> Genres { get; private set; }

            /// <summary>
            /// Null on <see cref="TvShow"/>.
            /// Sample: "{\"com.synology.TheTVDB\":{\"poster\":[\"https://www.thetvdb.com/banners/episodes/314087/5751273.jpg\"],\"rating\":{\"thetvdb\":7.1},\"reference\":{\"imdb\":\"tt6068828\",\"thetvdb\":\"5751273\"}}}\n"
            /// </summary>
            [DataMember(Name = "extra")]
            public string Extra { get; set; }

            [DataMember(Name = "is_parental_controlled")]
            public bool IsParentalControlled { get; set; }

            /// <summary>
            /// Null on <see cref="TvShow"/>.
            /// </summary>
            [DataMember(Name = "file")]
            public IEnumerable<File> Files { get; private set; }

            public override string ToString()
            {
                return string.Format("Summary: {0}, File:{{ {1} }}", Summary, Files);
            }
        }

        public enum LibraryType
        {
            Movie,
            TVShow,
            HomeVideo,
            TVRecord,
            Unknown
        }

        [DataContract]
        public class Library
        {
            [DataMember(Name = "id")]
            public int Id { get; set; }

            [DataMember(Name = "is_public")]
            public bool IsPublic { get; set; }

            [DataMember(Name = "title")]
            public string Title { get; set; }

            [DataMember(Name = "type")]
            private string Type { get; set; }

            [DataMember(Name = "visible")]
            public bool Visible { get; set; }

            public LibraryType LibraryType
            {
                get
                {
                    switch (Type)
                    {
                        case "movie": return LibraryType.Movie;
                        case "tvshow": return LibraryType.TVShow;
                        case "home_video": return LibraryType.HomeVideo;
                        case "tv_record": return LibraryType.TVRecord;
                        default: return LibraryType.Unknown;
                    }
                }
            }

            public override string ToString()
            {
                return string.Format("Id: {0}, IsPublic: {1}, Title: {2}, LibraryType: {3}, Visible: {4}", Id, IsPublic, Title ?? string.Empty, LibraryType, Visible);
            }
        }

        [DataContract]
        public class VideoStreamInfo
        {
            /// <summary>
            /// Types: "raw", "hls_remux", "mp4", "smooth"
            /// </summary>
            [DataMember(Name="format")]
            private string Format { get; set; }

            [DataMember(Name = "stream_id")]
            public string StreamId { get; set; }

            public override string ToString()
            {
                return string.Format("Format: {0}, StreamId: {1}", Format ?? string.Empty, StreamId ?? string.Empty);
            }
        }
    }
}