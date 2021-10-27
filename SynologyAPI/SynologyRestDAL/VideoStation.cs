using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using StdUtils;

namespace SynologyAPI.SynologyRestDAL
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
        public class FolderResult : TResult<FolderInfo>
        {
        }

        [DataContract]
        public class TvEpisodeResult : TResult<TvEpisodeInfo>
        {
        }

        [DataContract]
        public class SubtitlesResult : TResult<IEnumerable<Subtitle>>
        {
        }

        [DataContract]
        public class AudioTrackResult : TResult<AudioTrackInfo>
        {
        }

        [DataContract]
        public class VideoStreamResult : TResult<VideoStreamInfo>
        {
        }

        [DataContract]
        public class WatchStatusResult : TResult<WatchStatusInfo>
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
        public class FolderInfo : Info
        {
            [DataMember(Name = "objects")]
            public IEnumerable<FileSystemObject> FileSystemObjects { get; set; }
        }

        [DataContract]
        public class AudioTrackInfo : Info
        {
            [DataMember(Name = "trackinfo")]
            public IEnumerable<AudioTrack> AudioTracks { get; set; }
        }

        [DataContract]
        public class WatchStatusInfo
        {
            [DataMember(Name = "watch_status")]
            public WatchStatus WatchStatus { get; set; }
        }

        public enum MediaType
        {
            Unknown,
            Movie,
            TvShow,
            TvShowEpisode
        }

        [DataContract]
        public abstract class MetaDataItem
        {
            /// <summary>
            /// Sample: "TV-PG"
            /// </summary>
            [DataMember(Name = "certificate")]
            public string Certificate { get; set; }

            /// <summary>
            /// Sample: "Alps from Above"
            /// </summary>
            [DataMember(Name = "sort_title")]
            public string SortTitle { get; set; }

            /// <summary>
            /// Sample: "The Alps from Above"
            /// </summary>
            [DataMember(Name = "title")]
            public string Title { get; set; }

            [DataMember(Name = "metadata_locked")]
            public bool MetadataLocked { get; set; }

            [DataMember(Name = "id")]
            public int Id { get; set; }

            [DataMember(Name = "library_id")]
            public int LibraryId { get; set; }

            /// <summary>
            /// Sample: 14091
            /// </summary>
            [DataMember(Name = "mapper_id")]
            public int MapperId { get; set; }

            public DateTime? OriginalAvailable { get; private set; }

            /// <summary>
            /// Sample: "2003-10-07"
            /// </summary>
            [DataMember(Name = "original_available")]
            private string OriginalAvailableSetter
            {
                set
                {
                    try
                    {
                        var dateParts = value.Split('-');
                        if (dateParts.Length < 3)
                        {
                            return;
                        }

                        OriginalAvailable = new DateTime(int.Parse(dateParts[0]), int.Parse(dateParts[1]),
                            int.Parse(dateParts[2]));
                    }
                    catch
                    {
                        Trace.TraceInformation("Invalid date format:{0}", value);
                    }
                }
                get => OriginalAvailable == null
                    ? null
                    : $"{OriginalAvailable.Value.Year}-{OriginalAvailable.Value.Month}-{OriginalAvailable.Value.Day}";
            }

            /// <summary>
            /// Sample: 75
            /// </summary>
            [DataMember(Name = "rating")]
            public int Rating { get; set; }

            /// <summary>
            /// Sample: "Von den Karawanken nach Graz"
            /// </summary>
            [DataMember(Name = "tagline")]
            public string Tagline { get; set; }

            [DataMember(Name = "create_time")]
            public int CreateTime { get; set; }

            [DataMember(Name = "last_watched")]
            public int LastWatched { get; set; }

            [DataMember(Name = "additional")]
            public Additional Additional { get; private set; }

            public virtual MediaType MediaType { get; } = MediaType.Unknown;

            public override string ToString()
            {
                return
                    $"SortTitle: {SortTitle}, Title: {Title}, MetadataLocked: {MetadataLocked}, Id: {Id}, MapperId: {MapperId}, OriginalAvailable: {OriginalAvailable}, OriginalAvailableSetter: {OriginalAvailableSetter}";
            }
        }

        [DataContract]
        public class TvShow : MetaDataItem
        {
            public override MediaType MediaType => MediaType.TvShow;
        }

        [DataContract]
        public class File
        {
            /// <summary>
            /// Sample value:640000
            /// </summary>
            [DataMember(Name = "audio_bitrate")]
            public int AudioBitrate { get; set; }

            /// <summary>
            /// Sample value:"eac3"
            /// </summary>
            [DataMember(Name = "audio_codec")]
            public string AudioCodec { get; set; }

            /// <summary>
            /// Sample value:6
            /// </summary>
            [DataMember(Name = "channel")]
            public int Channel { get; set; }

            /// <summary>
            /// Sample value:"matroska,webm"
            /// </summary>
            [DataMember(Name = "container_type")]
            public string ContainerType { get; set; }

            /// <summary>
            /// Sample value:false
            /// </summary>
            [DataMember(Name = "conversion_produced")]
            public bool ConversionProduced { get; set; }

            /// <summary>
            /// Sample value:1280
            /// </summary>
            [DataMember(Name = "display_x")]
            public int DisplayX { get; set; }

            /// <summary>
            /// Sample value:720
            /// </summary>
            [DataMember(Name = "display_y")]
            public int DisplayY { get; set; }

            /// <summary>
            /// Sample value:"1:11:05"
            /// </summary>
            [DataMember(Name = "duration")]
            // ReSharper disable once InconsistentNaming
            public string DurationRaw { get; private set; }

            public TimeSpan? Duration
            {
                get
                {

                    try
                    {
                        return TimeSpanParser.Parse(DurationRaw, CultureInfo.InvariantCulture).Duration();
                    }
                    catch
                    {
                        return null;
                    }
                }
            }

            /// <summary>
            /// Sample value:100
            /// </summary>
            [DataMember(Name = "ff_video_profile")]
            // ReSharper disable once InconsistentNaming
            public int FFVideoProfile { get; set; }

            /// <summary>
            /// File size in bytes.
            /// Sample value:4908079893
            /// </summary>
            [DataMember(Name = "filesize")]
            public long FileSize { get; set; }

            /// <summary>
            /// Sample value:9205986
            /// </summary>
            [DataMember(Name = "frame_bitrate")]
            public int FrameBitrate { get; set; }

            /// <summary>
            /// Sample value:1001
            /// </summary>
            [DataMember(Name = "frame_rate_den")]
            public int FrameRateDen { get; set; }

            /// <summary>
            /// Sample value:24000
            /// </summary>
            [DataMember(Name = "frame_rate_num")]
            public int FrameRateNum { get; set; }

            /// <summary>
            /// Sample value:48000
            /// </summary>
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

            /// <summary>
            /// The position where the video seeing has been ended last time in seconds. 
            /// Sample value:4265
            /// </summary>
            [DataMember(Name = "position")]
            public long PositionRaw { get; set; }

            public TimeSpan Position => TimeSpan.FromSeconds(PositionRaw);

            /// <summary>
            /// Sample value:1280
            /// </summary>
            [DataMember(Name = "resolutionx")]
            public int ResolutionX { get; set; }

            ///// <summary>
            ///// Sample value:720
            ///// </summary>
            [DataMember(Name = "resolutiony")]
            public int ResolutionY { get; set; }

            /// <summary>
            /// Sample value:0
            /// </summary>
            [DataMember(Name = "rotation")]
            public int Rotation { get; set; }

            /// <summary>
            /// Sample value:"/video/TV_Show/The.Grand.Tour/The.Grand.Tour.S01.720p.WEBRip.X264-DEFLATE/The.Grand.Tour_S01E01.mkv"
            /// </summary>
            [DataMember(Name = "sharepath")]
            public string Sharepath { get; set; }

            /// <summary>
            /// Sample value:0
            /// </summary>
            [DataMember(Name = "video_bitrate")]
            public int VideoBitrate { get; set; }

            /// <summary>
            /// Sample value:"h264"
            /// </summary>
            [DataMember(Name = "video_codec")]
            public string VideoCodec { get; set; }

            ///// <summary>
            ///// Sample value:41
            ///// </summary>
            [DataMember(Name = "video_level")]
            public int VideoLevel { get; set; }

            /// <summary>
            /// Sample value:3
            /// </summary>
            [DataMember(Name = "video_profile")]
            public int VideoProfile { get; set; }

            /// <summary>
            /// Sample value:0.3623693379790941
            /// </summary>
            [DataMember(Name = "watched_ratio")]
            public decimal WatchedRatio { get; set; }

            public override string ToString()
            {
                return $"{{ Id: {Id}, Path: {Path ?? Sharepath ?? string.Empty} }}";
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
            internal int TvShowId { get; set; }

            [DataMember(Name = "tvshow_mapper_id")]
            internal int TvShowMapperId { get; set; }

            public DateTime? TvShowOriginalAvailable { get; private set; }

            /// <summary>
            /// Sample: "2003-10-07"
            /// </summary>
            [DataMember(Name = "tvshow_original_available")]
            private string TvShowOriginalAvailableSetter
            {
                set
                {
                    try
                    {
                        var dateParts = value.Split('-');
                        if (dateParts.Length < 3)
                        {
                            return;
                        }

                        TvShowOriginalAvailable = new DateTime(int.Parse(dateParts[0]), int.Parse(dateParts[1]),
                            int.Parse(dateParts[2]));
                    }
                    catch
                    {
                        Trace.TraceInformation($"Invalid date format:{value}");
                    }
                }
                get =>
                    TvShowOriginalAvailable == null
                        ? null
                        : $"{TvShowOriginalAvailable.Value.Year}-{TvShowOriginalAvailable.Value.Month}-{TvShowOriginalAvailable.Value.Day}";
            }

            public override MediaType MediaType => MediaType.TvShowEpisode;

            public override string ToString()
            {
                return
                    string.Format(
                        "{{ Episode: {1}, LastWatched: {2}, Season: {3}, Tagline: {4}, Summary: {5}, TvshowId: {{ {0} }} }}",
                        TvShowId, Episode, LastWatched, Season, Tagline, Additional.Summary ?? string.Empty);
            }
        }

        [DataContract]
        public class Movie : MetaDataItem
        {
            public override MediaType MediaType => MediaType.Movie;
        }

        [DataContract]
        public class PreviewVideo : TvEpisode
        {
        }

        [DataContract]
        public class FileSystemObject
        {
            /// <summary>
            /// Number of files. Min value is 0.
            /// Sample: 5
            /// </summary>
            [DataMember(Name = "file_count")]
            public int FileCount { get; set; }

            /// <summary>
            /// Path of the containing folder.
            /// Sample: "/volume1/video/TV_Show/Alps from Above"
            /// </summary>
            [DataMember(Name = "id")]
            public string Id { get; set; }

            [DataMember(Name = "preview_video")]
            public IEnumerable<PreviewVideo> PreviewVideos { get; set; }

            /// <summary>
            /// Path of the containing share folder.
            /// Optional.
            /// Sample: "/video/TV_Show/Alps from Above"
            /// </summary>
            [DataMember(Name = "sharepath")]
            public string SharePath { get; set; }

            /// <summary>
            /// Title.
            /// Optional.
            /// Sample: "Alps from Above"
            /// </summary>
            [DataMember(Name = "title")]
            public string Title { get; set; }

            /// <summary>
            /// Type.
            /// Optional.
            /// Sample: "folder", "file"
            /// </summary>
            [DataMember(Name = "type")]
            public string Type { get; set; }
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
            /// Optional.
            /// </summary>
            [DataMember(Name = "tvshow_summary")]
            public string TvShowSummary { get; set; }

            /// <summary>
            /// Sample value:0.3623693379790941
            /// </summary>
            [DataMember(Name = "watched_ratio")]
            public decimal? WatchedRatio { get; set; }

            /// <summary>
            /// Optional.
            /// Sample value: "2020-05-06 20:12:47.66867"
            /// </summary>
            [DataMember(Name = "poster_mtime")]
            private string PosterMTimeSetter { get; set; }

            /// <summary>
            /// Poster modification time.
            /// Optional.
            /// </summary>
            public DateTime? PosterMTime
            {
                get
                {
                    if (string.IsNullOrWhiteSpace(PosterMTimeSetter))
                    {
                        return null;
                    }

                    try
                    {
                        var formats = new[] { "yyyy-M-d H:m:s", "yyyy-M-d H:m:s.FFFFFF" };
                        var result = DateTime.ParseExact(PosterMTimeSetter, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
                        return result;
                    }
                    catch (System.Exception ex)
                    {
                        Trace.TraceWarning($"Invalid date format received from the NAS server: {PosterMTimeSetter}. Exception: {ex}.");
                        return null;
                    }
                }
                set => PosterMTimeSetter = value?.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            }

            /// <summary>
            /// Null on <see cref="TvShow"/>.
            /// </summary>
            [DataMember(Name = "file")]
            public IEnumerable<File> Files { get; private set; }

            public override string ToString()
            {
                return $"Summary: {Summary}, Files count:{{ {(Files ?? Enumerable.Empty<File>()).Count()} }}";
            }
        }

        public enum LibraryType
        {
            Movie,
            TvShow,
            HomeVideo,
            TvRecord,
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
                        case "tvshow": return LibraryType.TvShow;
                        case "home_video": return LibraryType.HomeVideo;
                        case "tv_record": return LibraryType.TvRecord;
                        default: return LibraryType.Unknown;
                    }
                }
            }

            public override string ToString()
            {
                return
                    $"Id: {Id}, IsPublic: {IsPublic}, Title: {Title ?? string.Empty}, LibraryType: {LibraryType}, Visible: {Visible}";
            }
        }

        [DataContract]
        public class VideoStreamInfo
        {
            /// <summary>
            /// Types: "raw", "hls_remux", "hls"
            /// </summary>
            [DataMember(Name = "format")]
            public string Format { get; set; }

            [DataMember(Name = "stream_id")]
            public string StreamId { get; set; }

            public override string ToString()
            {
                return $"Format: {Format ?? string.Empty}, StreamId: {StreamId ?? string.Empty}";
            }
        }

        [DataContract]
        public class Subtitle
        {
            [DataMember(Name = "embedded")]
            public bool Embedded { get; set; }

            /// <summary>
            /// Sample: "srt"
            /// </summary>
            [DataMember(Name = "format")]
            public string Format { get; set; }

            /// <summary>
            /// Samples: 1
            ///          2
            ///          "/volume1/video/TV_Show/The.Grand.Tour/The.Grand.Tour.S01.720p.WEBRip.X264-DEFLATE/The.Grand.Tour_S01E01.hun.srt"
            /// </summary>
            [DataMember(Name = "id")]
            public string Id { get; set; }

            /// <summary>
            /// Samples: "hun"
            ///          "eng"
            /// </summary>
            [DataMember(Name = "lang")]
            public string Language { get; set; }

            /// <summary>
            /// Samples: ""
            ///          "English"
            ///          "English (SDH)"
            /// </summary>
            [DataMember(Name = "title")]
            public string Title { get; set; }

            [DataMember(Name = "need_preview")]
            public bool NeedPreview { get; set; }

            public override string ToString()
            {
                return
                    $"Embedded: {Embedded}, Format: {Format}, Language: {Language}, Title: {Title}, NeedPreview: {NeedPreview}, Id: {Id}";
            }
        }

        [DataContract]
        public class AudioTrack
        {
            /// <summary>
            /// Id of audio track.
            /// Samples:
            /// 1
            /// 2
            /// </summary>
            [DataMember(Name = "id")]
            public int Id { get; set; }

            /// <summary>
            /// Track of audio track. Usually has the same value as <see cref="Id"/>.
            /// Samples:
            /// 1
            /// 2
            /// </summary>
            [DataMember(Name = "track")]
            public int Track { get; set; }

            /// <summary>
            /// Language of the audio track.
            /// Samples:
            /// "hun"
            /// "eng"
            /// </summary>
            [DataMember(Name = "language")]
            public string Language { get; set; }

            /// <summary>
            /// Audio stream id.
            /// Samples:
            /// 0
            /// </summary>
            [DataMember(Name = "streamid")]
            public int StreamId { get; set; }

            /// <summary>
            /// Indicate whether this is the default audio track.
            /// Samples:
            /// true
            /// false
            /// </summary>
            [DataMember(Name = "is_default")]
            public bool IsDefault { get; set; }

            /// <summary>
            /// Audio bitrate.
            /// Samples:
            /// 192000
            /// 448000
            /// </summary>
            [DataMember(Name = "bitrate")]
            public int Bitrate { get; set; }

            /// <summary>
            /// Audio sample rate.
            /// Samples:
            /// 48000
            /// </summary>
            [DataMember(Name = "sample_rate")]
            public int SampleRate { get; set; }

            /// <summary>
            /// Number of audio channels.
            /// Samples:
            /// 2
            /// 6
            /// </summary>
            [DataMember(Name = "channel")]
            public int Channel { get; set; }

            /// <summary>
            /// Layout name of <see cref="Channel"/>.
            /// Samples:
            /// "stereo"
            /// "5.1(side)"
            /// </summary>
            [DataMember(Name = "channel_layout")]
            public string ChannelLayout { get; set; }

            /// <summary>
            /// Name of the audio codec.
            /// Samples:
            /// "ac3"
            /// "DTS"
            /// </summary>
            [DataMember(Name = "codec")]
            public string Codec { get; set; }

            /// <summary>
            /// Raw name of the audio codec.
            /// Samples:
            /// "ac3"
            /// "DTS"
            /// </summary>
            [DataMember(Name = "codec_raw")]
            public string CodecRaw { get; set; }

            /// <summary>
            /// Audio frequency in Hz
            /// Samples:
            /// 48000
            /// </summary>
            [DataMember(Name = "frequency")]
            public int Frequency { get; set; }

            [DataMember(Name = "profile")]
            public string Profile { get; set; }

            public override string ToString()
            {
                return
                    $"Id: {Id}, Track: {Track}, Language: {Language}, StreamId: {StreamId}, IsDefault: {IsDefault}, Bitrate: {Bitrate}, SampleRate: {SampleRate}, Channel: {Channel}, ChannelLayout: {ChannelLayout}, Codec: {Codec}, CodecRaw: {CodecRaw}, Frequency: {Frequency}, Profile: {Profile}";
            }
        }

        [DataContract]
        public class WatchStatus
        {
            [DataMember(Name = "last_update")]
            private long? LastUpdateSetter
            {
                get =>
                    LastUpdate == null
                        ? (long?)null
                        : DateTimeConverter.ToUnixTime(LastUpdate.Value);
                set =>
                    LastUpdate = value.HasValue
                        ? DateTimeConverter.FromUnixTime(value.Value)
                        : (DateTime?)null;
            }

            /// <summary>
            /// When was last updated the position. Null if this information is not available.
            /// Samples:
            /// 1554724904
            /// and it means: Mon Apr 08 2019 13:01:44 GMT+0200 (CEST)
            /// </summary>
            public DateTime? LastUpdate { get; private set; }

            /// <summary>
            /// The last position.
            /// Samples:
            /// "13"
            /// </summary>
            [DataMember(Name = "position")]
            private string PositionSetter
            {
                get => Position?.ToString();
                set => Position = long.TryParse(value, out var position) ? (long?)position : null;
            }

            public long? Position { get; private set; }

            public override string ToString()
            {
                return $"LastUpdate: {(LastUpdate.HasValue ? LastUpdate.ToString() : "Unknown")}, Position: {(Position.HasValue ? Position.ToString() : "Unknown")}";
            }
        }
    }
}
