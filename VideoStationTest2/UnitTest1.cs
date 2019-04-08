using System;
using System.Linq;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SynologyAPI;

namespace VideoStationTest2
{
    [TestClass]
    public class UnitTest1
    {
        private string _sid;
        private string Sid => _sid;

        [TestInitialize()]
        public void Initialize()
        {
            VideoStation = VideoStationFactory.CreateVideoStation();
            var loggedIn = VideoStation.LoginAsync().GetAwaiter().GetResult();
            Assert.IsTrue(loggedIn);
            _sid = ObjectAccessor.GetField(VideoStation, "Sid") as string;
            Console.WriteLine("_sid (for debug purpose only): " + Sid);
        }

        [TestCleanup()]
        public void Cleanup()
        {
            var loggedOut = VideoStation.LogoutAsync().GetAwaiter().GetResult();
            Assert.IsTrue(loggedOut);
        }

        private VideoStation VideoStation { get; set; }

        [TestMethod]
        public void Test_TVShowAndEpisode()
        {
            const int libraryId = 0; //Built in library
            var tvShowsInfo = VideoStation.TvShowListAsync(
                    libraryId,
                    VideoStation.SortBy.Added)
                .GetAwaiter().GetResult();

            var episodesInfo = VideoStation.TvShowEpisodeListAsync(
                    libraryId,
                    tvShowsInfo.TvShows.First(item => item.SortTitle == "Grand Tour").Id)
                .GetAwaiter().GetResult();
            Assert.IsNotNull(episodesInfo);

            var episodes = episodesInfo.Episodes.ToList();
            Assert.IsTrue(episodes.Any());

            var firstEpisode = episodes.First();
            Assert.IsTrue(firstEpisode.Id > 0);

            var episodeFile = firstEpisode.Additional.Files.FirstOrDefault();
            Assert.IsNotNull(episodeFile);

            var duration = episodeFile.Duration;
            Assert.IsTrue(duration.Ticks >= 0);

            var episodesResult = VideoStation.TvShowEpisodeGetInfoAsync(firstEpisode.Id).GetAwaiter().GetResult();

            var firstEpisodeControl = episodesResult.Episodes.FirstOrDefault();
            Assert.IsNotNull(firstEpisodeControl);
            Assert.AreEqual(firstEpisode.Id, firstEpisodeControl.Id);
            Assert.AreEqual(firstEpisode.Episode, firstEpisodeControl.Episode);
            Assert.AreEqual(firstEpisode.Season, firstEpisodeControl.Season);
            Assert.AreEqual(firstEpisode.Tagline, firstEpisodeControl.Tagline);

            //open - stream - close

            var videoStream = VideoStation.StreamingOpenAsync(episodeFile.Id).GetAwaiter().GetResult();
            Assert.IsTrue(videoStream.Data.StreamId != null);

            var webRequest = VideoStation.StreamingStreamAsync(videoStream.Data.StreamId).GetAwaiter().GetResult();
            Assert.IsTrue(webRequest.RequestUri.ToString() != null);

            var closeResult = VideoStation.StreamingCloseAsync(videoStream.Data.StreamId, true).GetAwaiter().GetResult();
            Assert.IsTrue(closeResult);
        }

        [TestMethod]
        public void Test_LibraryList()
        {
            var result = VideoStation.LibraryListAsync().GetAwaiter().GetResult();
            var libraries = result.Libraries.ToList();

            Assert.IsTrue(libraries.Any());
            Assert.IsTrue(libraries.Any(it => it.Id == 0)); //has least one built in library
        }

        [TestMethod]
        public void Test_MovieList()
        {
            const int libraryId = 0; //Built in library
            var result = VideoStation.MovieList(libraryId, VideoStation.SortBy.Added, VideoStation.SortDirection.Descending, 0, 10).GetAwaiter().GetResult();
            var movies = result.Movies.ToList();
            var files = movies[0].Additional.Files.ToList();
            var fileDescription = files[0].ToString();

            Console.WriteLine(fileDescription);

            Assert.IsTrue(movies.Any());
            Assert.IsTrue(movies.Any(it => it.LibraryId == 0)); //has least one built in library
        }

        [TestMethod]
        public void Test_Poster_Movie()
        {
            const int libraryId = 0; //Built in library
            var result = VideoStation.MovieList(libraryId, VideoStation.SortBy.Added, VideoStation.SortDirection.Descending, 0, 10).GetAwaiter().GetResult();
            var movies = result.Movies.ToList();

            var posterRequest = VideoStation.PosterGetImageAsync(movies[0].Id, VideoStation.MediaType.Movie).GetAwaiter().GetResult();
            using (var posterStream = posterRequest.GetResponseAsync().GetAwaiter().GetResult())
            {
                using (var file = System.IO.File.OpenWrite("poster" + movies[0].Id + ".jpg"))
                {
                    posterStream.GetResponseStream()?.CopyTo(file, 8196);
                }
            }
        }

        [TestMethod]
        public void Test_Poster_TVShow()
        {
            const int libraryId = 0; //Built in library
            var tvShowsInfo = VideoStation.TvShowListAsync(
                    libraryId,
                    VideoStation.SortBy.Added)
                .GetAwaiter().GetResult().TvShows.ToList();

            var posterRequest = VideoStation.PosterGetImageAsync(tvShowsInfo[0].Id, VideoStation.MediaType.TvShow).GetAwaiter().GetResult();
            using (var posterStream = posterRequest.GetResponseAsync().GetAwaiter().GetResult())
            {
                using (var file = System.IO.File.OpenWrite("poster" + tvShowsInfo[0].Id + ".jpg"))
                {
                    posterStream.GetResponseStream()?.CopyTo(file, 8196);
                }
            }
        }

        [TestMethod]
        public void Test_Poster_TVShowEpisode()
        {
            const int libraryId = 0; //Built in library
            var tvShowsInfo = VideoStation.TvShowListAsync(
                    libraryId,
                    VideoStation.SortBy.Added)
                .GetAwaiter().GetResult().TvShows.ToList();
            var episodesInfo = VideoStation.TvShowEpisodeListAsync(
                    libraryId,
                    tvShowsInfo[0].Id)
                .GetAwaiter().GetResult();
            var episodes = episodesInfo.Episodes.ToList();

            var posterRequest = VideoStation.PosterGetImageAsync(episodes[0].Id, VideoStation.MediaType.TvShowEpisode).GetAwaiter().GetResult();
            using (var posterStream = posterRequest.GetResponseAsync().GetAwaiter().GetResult())
            {
                using (var file = System.IO.File.OpenWrite("poster" + episodes[0].Id + ".jpg"))
                {
                    posterStream.GetResponseStream()?.CopyTo(file, 8196);
                }
            }
        }

        [TestMethod]
        public void Test_Backdrop_Movie_0()
        {
            const int libraryId = 0; //Built in library
            var result = VideoStation.MovieList(libraryId, VideoStation.SortBy.Added, VideoStation.SortDirection.Descending, 0, 10).GetAwaiter().GetResult();
            var movies = result.Movies.ToList();

            var backdropRequest = VideoStation.BackdropGetAsync(movies[0].MapperId).GetAwaiter().GetResult();

            try
            {
                using (var backdropStream = backdropRequest.GetResponseAsync().GetAwaiter().GetResult())
                {
                    using (var file = System.IO.File.OpenWrite("backdrop" + movies[0].MapperId + ".jpg"))
                    {
                        backdropStream.GetResponseStream()?.CopyTo(file, 8196);
                    }
                }
            }
            catch (WebException ex)
            {
                var status = (ex.Response as HttpWebResponse)?.StatusCode;
                if (status != null && status == HttpStatusCode.NotFound)
                {
                    Console.WriteLine("It has not backdrop image!");
                }
                else
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        [TestMethod]
        public void Test_Backdrop_Movie_1()
        {
            const int libraryId = 0; //Built in library
            var result = VideoStation.MovieList(libraryId, VideoStation.SortBy.Added, VideoStation.SortDirection.Descending, 0, 10).GetAwaiter().GetResult();
            var movies = result.Movies.ToList();

            var backdropRequest = VideoStation.BackdropGetAsync(movies[1].MapperId).GetAwaiter().GetResult();

            try
            {
                using (var backdropStream = backdropRequest.GetResponseAsync().GetAwaiter().GetResult())
                {
                    using (var file = System.IO.File.OpenWrite("backdrop" + movies[1].MapperId + ".jpg"))
                    {
                        backdropStream.GetResponseStream()?.CopyTo(file, 8196);
                    }
                }
            }
            catch (WebException ex)
            {
                var status = (ex.Response as HttpWebResponse)?.StatusCode;
                if (status != null && status == HttpStatusCode.NotFound)
                {
                    Console.WriteLine("It has not backdrop image!");
                }
                else
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        [TestMethod]
        public void Test_Backdrop_TVShow()
        {
            const int libraryId = 0; //Built in library
            var tvShowsInfo = VideoStation.TvShowListAsync(
                    libraryId,
                    VideoStation.SortBy.Added)
                .GetAwaiter().GetResult().TvShows.ToList();

            var backdropRequest = VideoStation.BackdropGetAsync(tvShowsInfo[0].MapperId).GetAwaiter().GetResult();
            using (var backdropStream = backdropRequest.GetResponseAsync().GetAwaiter().GetResult())
            {
                using (var file = System.IO.File.OpenWrite("backdrop" + tvShowsInfo[0].MapperId + ".jpg"))
                {
                    backdropStream.GetResponseStream()?.CopyTo(file, 8196);
                }
            }
        }

        [TestMethod]
        public void Test_Backdrop_TVShowEpisode_WithFallback()
        {
            const int libraryId = 0; //Built in library
            var tvShowsInfo = VideoStation.TvShowListAsync(
                    libraryId,
                    VideoStation.SortBy.Added)
                .GetAwaiter().GetResult().TvShows.ToList();

            var tvShowInfo = tvShowsInfo[0];

            var episodesInfo = VideoStation.TvShowEpisodeListAsync(
                    libraryId,
                    tvShowInfo.Id)
                .GetAwaiter().GetResult();
            var episodes = episodesInfo.Episodes.ToList();
            int? backdropMapperId = null;

            var backdropRequest = VideoStation.BackdropGetAsync(episodes[0].MapperId).GetAwaiter().GetResult();
            try
            {
                using (var backdropStream = backdropRequest.GetResponseAsync().GetAwaiter().GetResult())
                {
                    using (var file = System.IO.File.OpenWrite("backdrop" + episodes[0].MapperId + ".jpg"))
                    {
                        backdropStream.GetResponseStream()?.CopyTo(file, 8196);
                        backdropMapperId = episodes[0].MapperId;
                    }
                }
            }
            catch (WebException ex)
            {
                var status = (ex.Response as HttpWebResponse)?.StatusCode;
                if (status != null && status == HttpStatusCode.NotFound)
                {
                    Console.WriteLine("It has not backdrop image!");

                    backdropRequest = VideoStation.BackdropGetAsync(tvShowInfo.MapperId).GetAwaiter().GetResult();
                    using (var backdropStream = backdropRequest.GetResponseAsync().GetAwaiter().GetResult())
                    {
                        using (var file = System.IO.File.OpenWrite("backdrop" + episodes[0].MapperId + ".jpg"))
                        {
                            backdropStream.GetResponseStream()?.CopyTo(file, 8196);
                            backdropMapperId = tvShowInfo.MapperId;
                        }
                    }
                }
                else
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            Console.WriteLine($"backdropMapperId: {(backdropMapperId.HasValue ? backdropMapperId.ToString() : "Unknown")}");
        }

        [TestMethod]
        public void Test_Backdrop_TVShowEpisode_Direct()
        {
            //There is no backdrop image for episode parts of TVShow, please to download the TVShow(collection) backdrop image instead.

            const int libraryId = 0; //Built in library
            var tvShowsInfo = VideoStation.TvShowListAsync(
                    libraryId,
                    VideoStation.SortBy.Added)
                .GetAwaiter().GetResult().TvShows.ToList();

            var tvShowInfo = tvShowsInfo[0];

            var backdropRequest = VideoStation.BackdropGetAsync(tvShowInfo.MapperId).GetAwaiter().GetResult();
            using (var backdropStream = backdropRequest.GetResponseAsync().GetAwaiter().GetResult())
            {
                using (var file = System.IO.File.OpenWrite("backdrop" + tvShowInfo.MapperId + ".jpg"))
                {
                    backdropStream.GetResponseStream()?.CopyTo(file, 8196);
                }
            }
        }

        [TestMethod]
        public void Test_SubtitleList_TVShowAndEpisode()
        {
            const int libraryId = 0; //Built in library
            var tvShowsInfo = VideoStation.TvShowListAsync(
                    libraryId,
                    VideoStation.SortBy.Added)
                .GetAwaiter().GetResult();

            var episodesInfo = VideoStation.TvShowEpisodeListAsync(
                    libraryId,
                    tvShowsInfo.TvShows.First(item => item.SortTitle == "Grand Tour").Id)
                .GetAwaiter().GetResult();
            Assert.IsNotNull(episodesInfo);

            var episodes = episodesInfo.Episodes.ToList();
            Assert.IsTrue(episodes.Any());

            var firstEpisode = episodes.First();
            Assert.IsTrue(firstEpisode.Id > 0);

            var episodeFile = firstEpisode.Additional.Files.FirstOrDefault();
            Assert.IsNotNull(episodeFile);

            var subtitles = VideoStation.SubtitleListAsync(episodeFile.Id).GetAwaiter().GetResult();

            foreach (var subtitle in subtitles)
            {
                Console.WriteLine(subtitle);
            }
        }

        [TestMethod]
        public void Test_SubtitleGet_TVShowAndEpisode_SimpleTest()
        {
            //var subtitleRequest = VideoStation.SubtitleGet(27, false, "/volume1/video/TV_Show/The.Grand.Tour/The.Grand.Tour.S01.720p.WEBRip.X264-DEFLATE/The.Grand.Tour_S01E01.hun.srt").GetAwaiter().GetResult();
            var subtitleRequest = VideoStation.SubtitleGetAsync(27, false, "/volume1/video/TV_Show/The.Grand.Tour/The.Grand.Tour.S01.720p.WEBRip.X264-DEFLATE/The.Grand.Tour_S01E01.hun.srt").GetAwaiter().GetResult();
            using (var subtitleStream = subtitleRequest.GetResponseAsync().GetAwaiter().GetResult())
            {
                using (var file = System.IO.File.OpenWrite("subtitleRequest_27.srt"))
                {
                    subtitleStream.GetResponseStream()?.CopyTo(file, 8196);
                }
            }
        }

        [TestMethod]
        public void Test_SubtitleGet_TVShowAndEpisode()
        {
            const int libraryId = 0; //Built in library
            var tvShowsInfo = VideoStation.TvShowListAsync(
                    libraryId,
                    VideoStation.SortBy.Added)
                .GetAwaiter().GetResult();

            var episodesInfo = VideoStation.TvShowEpisodeListAsync(
                    libraryId,
                    tvShowsInfo.TvShows.First(item => item.SortTitle == "Grand Tour").Id)
                .GetAwaiter().GetResult();
            Assert.IsNotNull(episodesInfo);

            var episodes = episodesInfo.Episodes.ToList();
            Assert.IsTrue(episodes.Any());

            var firstEpisode = episodes.First();
            Assert.IsTrue(firstEpisode.Id > 0);

            var episodeFile = firstEpisode.Additional.Files.FirstOrDefault();
            Assert.IsNotNull(episodeFile);

            var subtitles = VideoStation.SubtitleListAsync(episodeFile.Id).GetAwaiter().GetResult()
                .ToList();
            if (!subtitles.Any()) return;

            foreach (var requestedSubtitle in subtitles)
            {
                //var requestedSubtitle = subtitles.FirstOrDefault(s => !s.Embedded);

                var subtitleRequest = VideoStation.SubtitleGetAsync(episodeFile.Id, false, requestedSubtitle.Id).GetAwaiter().GetResult();
                using (var subtitleStream = subtitleRequest.GetResponseAsync().GetAwaiter().GetResult())
                {
                    var subtitleEncodedId = WebUtility.UrlEncode(requestedSubtitle.Id);
                    var filename = $"subtitleRequest_{episodeFile.Id}_{requestedSubtitle.Language ?? "Unknown"}_{subtitleEncodedId}.{requestedSubtitle.Format ?? "srt"}";
                    using (var file = System.IO.File.OpenWrite(filename))
                    {
                        subtitleStream.GetResponseStream()?.CopyTo(file, 8196);
                    }
                }
            }
        }

        [TestMethod]
        public void Test_SubtitleGet_Movie()
        {
            const int libraryId = 0; //Built in library
            var result = VideoStation.MovieList(libraryId, VideoStation.SortBy.Added, VideoStation.SortDirection.Descending, 0, 10).GetAwaiter().GetResult();
            var movies = result.Movies.ToList();

            var movie = movies.FirstOrDefault();
            Assert.IsNotNull(movie);

            var movieFile = movie.Additional.Files.FirstOrDefault();
            Assert.IsNotNull(movieFile);

            var subtitles = VideoStation.SubtitleListAsync(movieFile.Id).GetAwaiter().GetResult()
                .ToList();
            if (!subtitles.Any()) return;

            foreach (var requestedSubtitle in subtitles)
            {
                //var requestedSubtitle = subtitles.FirstOrDefault(s => !s.Embedded);

                var subtitleRequest = VideoStation.SubtitleGetAsync(movieFile.Id, false, requestedSubtitle.Id).GetAwaiter().GetResult();
                using (var subtitleStream = subtitleRequest.GetResponseAsync().GetAwaiter().GetResult())
                {
                    var subtitleEncodedId = WebUtility.UrlEncode(requestedSubtitle.Id);
                    var filename = $"subtitleRequest_{movieFile.Id}_{requestedSubtitle.Language ?? "Unknown"}_{subtitleEncodedId}.{requestedSubtitle.Format ?? "srt"}";
                    using (var file = System.IO.File.OpenWrite(filename))
                    {
                        subtitleStream.GetResponseStream()?.CopyTo(file, 8196);
                    }
                }
            }
        }

        [TestMethod]
        public void Test_AudioTrackGet_Movie()
        {
            const int libraryId = 0; //Built in library
            var result = VideoStation.MovieList(libraryId, VideoStation.SortBy.Added, VideoStation.SortDirection.Descending, 0, 10).GetAwaiter().GetResult();
            var movies = result.Movies.ToList();

            var movie = movies.FirstOrDefault();
            Assert.IsNotNull(movie);

            var movieFile = movie.Additional.Files.FirstOrDefault();
            Assert.IsNotNull(movieFile);

            var audioTrackInfo = VideoStation.AudioTrackListAsync(movieFile.Id).GetAwaiter().GetResult();
            var audioTracks = audioTrackInfo.AudioTracks.ToList();
            if (!audioTracks.Any()) return;
            
            Assert.AreEqual(2, audioTracks.Count);
        }

        [TestMethod]
        public void Test_WatchStatusGetInfo_Movie()
        {
            const int libraryId = 0; //Built in library
            var result = VideoStation.MovieList(libraryId, VideoStation.SortBy.Added, VideoStation.SortDirection.Descending, 0, 10).GetAwaiter().GetResult();
            var movies = result.Movies.ToList();

            var movie = movies.FirstOrDefault();
            Assert.IsNotNull(movie);

            var movieFile = movie.Additional.Files.FirstOrDefault();
            Assert.IsNotNull(movieFile);

            var watchStatusResult = VideoStation.WatchStatusGetInfoAsync(movie.Id).GetAwaiter().GetResult();
            Assert.IsNotNull(watchStatusResult);
        }

        [TestMethod]
        public void Test_WatchStatusSetInfo_Movie()
        {
            const int libraryId = 0; //Built in library
            var result = VideoStation.MovieList(libraryId, VideoStation.SortBy.Added, VideoStation.SortDirection.Descending, 0, 10).GetAwaiter().GetResult();
            var movies = result.Movies.ToList();

            var movie = movies.FirstOrDefault();
            Assert.IsNotNull(movie);

            var movieFile = movie.Additional.Files.FirstOrDefault();
            Assert.IsNotNull(movieFile);

            VideoStation.WatchStatusSetInfoAsync(movie.Id, 99).GetAwaiter();
        }
    }
}