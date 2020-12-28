using SynologyRestDAL.Vs;

namespace KDSVideo.ViewModels.NavigationViewModels
{
    public class TvShowMetaDataItem : MediaMetaDataItem
    {
        public TvShowMetaDataItem(TvShow tvShow)
            : base(tvShow)
        {
            TvShow = tvShow;
        }
        
        public TvShow TvShow { get; set; }
    }
}
