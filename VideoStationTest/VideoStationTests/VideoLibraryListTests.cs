using System.Diagnostics;
using SynologyAPI.Exception;

namespace SynologyApiTest.VideoStationTests
{
    using System;
    using System.Linq;
    using SynologyAPI;
    using NUnit.Framework;

    [TestFixture]
    public class VideoLibraryListTests : BaseSynologyTests
    {
        [SetUp]
        [Test]
        public void Setup()
        {
            var vs = new VideoStation(new Uri(Host), CreateProxy(Proxy));
            try
            {
                vs.LoginAsync(Username, Password).GetAwaiter().GetResult();
                VideoStation = vs;
            }
            catch (SynoRequestException)
            {
                VideoStation = null;
            }

            Assert.That(VideoStation, Is.Not.Null);
        }

        private VideoStation VideoStation { get; set; }

        [Test]
        public void CanListVideosInLibrary()
        {
            var result = VideoStation.TvShowListAsync(0).GetAwaiter().GetResult();

#if DEBUG
            var tvShowsList = result.TvShows.ToList();

            var longest = tvShowsList.OrderByDescending(t => t.Title.Length).First().Title.Length;
            var tvShows = tvShowsList.OrderBy(t => t.SortTitle);

            foreach (var show in tvShows)
            {
                Debug.WriteLine("#{0} | {1} {3}| {2}", show.Id, show.Title, show.OriginalAvailable,
                    new String(' ', longest - show.Title.Length));
            }
#endif

            Assert.That(result.TvShows.Count(), Is.GreaterThan(0));
        }

        [Test]
        public void TvShowList_ListContainsShows()
        {
            var result = VideoStation.TvShowListAsync(0).GetAwaiter().GetResult().TvShows.ToArray();

            Assert.That(result.Length, Is.GreaterThan(0));
        }

        [Test]
        public void TvShowList_ShowsHaveTitles()
        {
            var result = VideoStation.TvShowListAsync(0).GetAwaiter().GetResult().TvShows;

            Assert.That(result.First().Title, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void TvShowList_ShowsHaveIds()
        {
            var result = VideoStation.TvShowListAsync(0).GetAwaiter().GetResult().TvShows;

            Assert.That(result.First().Id, Is.Not.Null.And.GreaterThan(0));
        }

        [Test]
        public void TvShowList_ShouldBeAbleToSort()
        {
            var result = VideoStation.TvShowListAsync(0).GetAwaiter().GetResult().TvShows.ToList();

            Assert.That(result, Is.Not.Null);

            var resultOrdered = result.OrderBy(s => s.SortTitle);
            var resultOrderedDesc = result.OrderByDescending(s => s.SortTitle);

            Assert.That(result, Is.Not.Null.And.Not.Empty);
            Assert.That(resultOrdered, Is.Not.Null.And.Not.Empty);
            Assert.That(resultOrderedDesc, Is.Not.Null.And.Not.Empty);

            Assert.That(resultOrdered.SequenceEqual(result), Is.False);
            Assert.That(resultOrdered.SequenceEqual(resultOrderedDesc), Is.False);
            Assert.That(resultOrderedDesc.SequenceEqual(result), Is.False);
        }

        [Test]
        public void TvShowEpisode_ShouldListEpisodesForSeries()
        {
            var show = VideoStation.TvShowListAsync(0).GetAwaiter().GetResult().TvShows.First();

            var tvEpisodes = VideoStation.TvShowEpisodeListAsync(0, show.Id).GetAwaiter().GetResult().Episodes;

            Assert.That(tvEpisodes, Is.Not.Null);
        }

        [Test]
        public void TvShowEpisode_EpisodeShouldHaveTitle()
        {
            var show = VideoStation.TvShowListAsync(0).GetAwaiter().GetResult().TvShows.First();

            var episodes = VideoStation.TvShowEpisodeListAsync(0, show.Id).GetAwaiter().GetResult().Episodes;
            var firstEpisode = episodes.First();

            Assert.That(show,           Is.Not.Null);
            Assert.That(episodes,       Is.Not.Null.And.Not.Empty);
            Assert.That(firstEpisode,   Is.Not.Null);
        }

        [Test]
        public void TvShowEpisode_ShouldHaveShowInformation()
        {
            var show = VideoStation.TvShowListAsync(0).GetAwaiter().GetResult().TvShows.First();

            var episode = VideoStation.TvShowEpisodeListAsync(0, show.Id).GetAwaiter().GetResult().Episodes.First(e => e.Additional.Summary.Length > 0);

            //Assert.That(show, Is.EqualTo(episode.Show));

            Assert.That(episode.Additional.Summary, Is.Not.Null.And.Not.Empty);
            Assert.That(episode.Tagline, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        [TestCase(@"Mother")]
        [TestCase(@"Big Bang")]
        [TestCase(@"Broke Girls")]
        [TestCase(@"House of cards")]
        public void TvShowEpisode_CanListEpisodesForShow(string showTitle)
        {
            var data = VideoStation.TvShowListAsync(0).GetAwaiter().GetResult();

#if DEBUG
            var show = data.TvShows.First(s => s.Title.IndexOf(showTitle, StringComparison.OrdinalIgnoreCase) >= 0);
            var episodes = VideoStation.TvShowEpisodeListAsync(0, show.Id).GetAwaiter().GetResult().Episodes.ToList();
            var longestEpisodeLength = episodes.OrderByDescending(s => s.Tagline.Length).First().Tagline.Length;

            foreach (var episode in episodes)
            {
                var summary = string.Empty;
                if (episode.Additional.Summary != null)
                    summary = episode.Additional.Summary.Length > 40 ? episode.Additional.Summary.Substring(0, 40) : episode.Additional.Summary;

                Debug.WriteLine("#{0} | S{3}E{4} {1} {5}| {2}",
                    episode.Id,
                    episode.Tagline,
                    summary,
                    episode.Season < 10 ? "0" + episode.Season : episode.Season.ToString(),
                    episode.Episode < 10 ? "0" + episode.Episode : episode.Episode.ToString(),
                    new string(' ', longestEpisodeLength - episode.Tagline.Length));
            }
#endif

            //Assert.That(data.Success, Is.True);
        }

        //[Test]
        //[ExpectedException(exceptionType: typeof(InvalidDataException))]
        //public void TvShowEpisode_ShouldGetExceptionIfSearchingEpisodesForNullShow()
        //{
        //    // ReSharper disable once UnusedVariable
        //    var tvEpisodes = VideoStation.TvShowEpisodeListAsync(null).GetAwaiter().GetResult().Episodes;
        //}
    }
}