using SynologyRestDAL.Vs;

namespace KDSVideo.ViewModels.NavigationViewModels
{
    public class MovieMetaDataItem: MediaMetaDataItem
    {
        public MovieMetaDataItem(Movie movie)
            : base(movie)
        {
            Movie = movie;
        }
        
        public Movie Movie { get; set; }
    }
}