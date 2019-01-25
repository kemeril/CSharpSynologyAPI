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

        [TestInitialize()]
        public void Initialize()
        {
            VideoStation = VideoStationFactory.CreateVideoStation();
            var loggedIn = VideoStation.Login().GetAwaiter().GetResult();
            Assert.IsTrue(loggedIn);
            _sid = ObjectAccessor.GetField(VideoStation, "Sid") as string;
        }

        [TestCleanup()]
        public void Cleanup()
        {
            var loggedOut = VideoStation.Logout().GetAwaiter().GetResult();
            Assert.IsTrue(loggedOut);
        }

        private VideoStation VideoStation { get; set; }

        [TestMethod]
        public void Test_TVShowAndEpisode()
        {
            const int libraryId = 0; //Built in library
            var tvShowsInfo = VideoStation.TvShowList(
                    libraryId,
                    VideoStation.SortBy.Added)
                .GetAwaiter().GetResult();

            var episodesInfo = VideoStation.TvShowEpisodeList(
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

            var episodesResult = VideoStation.TvShowEpisodeGetInfo(firstEpisode.Id).GetAwaiter().GetResult();

            var firstEpisodeControl = episodesResult.Episodes.FirstOrDefault();
            Assert.IsNotNull(firstEpisodeControl);
            Assert.AreEqual(firstEpisode.Id, firstEpisodeControl.Id);
            Assert.AreEqual(firstEpisode.Episode, firstEpisodeControl.Episode);
            Assert.AreEqual(firstEpisode.Season, firstEpisodeControl.Season);
            Assert.AreEqual(firstEpisode.Tagline, firstEpisodeControl.Tagline);

            //open - stream - close

            var videoStream = VideoStation.StreamingOpen(episodeFile.Id).GetAwaiter().GetResult();
            Assert.IsTrue(videoStream.Data.StreamId != null);

            var webRequest = VideoStation.StreamingStream(videoStream.Data.StreamId).GetAwaiter().GetResult();
            Assert.IsTrue(webRequest.RequestUri.ToString() != null);

            var closeResult = VideoStation.StreamingClose(videoStream.Data.StreamId, true).GetAwaiter().GetResult();
            Assert.IsTrue(closeResult);
        }

        [TestMethod]
        public void Test_LibraryList()
        {
            var result = VideoStation.LibraryList().GetAwaiter().GetResult();
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

            var posterRequest = VideoStation.PosterGetImage(movies[0].Id, VideoStation.MediaType.Movie).GetAwaiter().GetResult();
            var posterStream = posterRequest.GetResponseAsync().GetAwaiter().GetResult();
            using (var file = System.IO.File.OpenWrite("poster" + movies[0].Id + ".jpg"))
            {
                posterStream.GetResponseStream()?.CopyTo(file, 8196);
            }
        }

        [TestMethod]
        public void Test_Poster_TVShow()
        {
            const int libraryId = 0; //Built in library
            var tvShowsInfo = VideoStation.TvShowList(
                    libraryId,
                    VideoStation.SortBy.Added)
                .GetAwaiter().GetResult().TvShows.ToList();

            var posterRequest = VideoStation.PosterGetImage(tvShowsInfo[0].Id, VideoStation.MediaType.TVShow).GetAwaiter().GetResult();
            var posterStream = posterRequest.GetResponseAsync().GetAwaiter().GetResult();
            using (var file = System.IO.File.OpenWrite("poster" + tvShowsInfo[0].Id + ".jpg"))
            {
                posterStream.GetResponseStream()?.CopyTo(file, 8196);
            }
        }

        [TestMethod]
        public void Test_Poster_TVShowEpisode()
        {
            const int libraryId = 0; //Built in library
            var tvShowsInfo = VideoStation.TvShowList(
                    libraryId,
                    VideoStation.SortBy.Added)
                .GetAwaiter().GetResult().TvShows.ToList();
            var episodesInfo = VideoStation.TvShowEpisodeList(
                    libraryId,
                    tvShowsInfo[0].Id)
                .GetAwaiter().GetResult();
            var episodes = episodesInfo.Episodes.ToList();

            var posterRequest = VideoStation.PosterGetImage(episodes[0].Id, VideoStation.MediaType.TVShowEpisode).GetAwaiter().GetResult();
            var posterStream = posterRequest.GetResponseAsync().GetAwaiter().GetResult();
            using (var file = System.IO.File.OpenWrite("poster" + episodes[0].Id + ".jpg"))
            {
                posterStream.GetResponseStream()?.CopyTo(file, 8196);
            }
        }
    }
}