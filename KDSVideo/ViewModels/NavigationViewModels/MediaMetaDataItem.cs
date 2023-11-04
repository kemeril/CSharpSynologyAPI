using Windows.Graphics.Imaging;
using SynologyAPI.SynologyRestDAL.Vs;

namespace KDSVideo.ViewModels.NavigationViewModels
{
    public abstract class MediaMetaDataItem
    {
        protected MediaMetaDataItem(MetaDataItem metaDataItem)
        {
            MetaDataItem = metaDataItem;
        }

        public MetaDataItem MetaDataItem { get; }

        public SoftwareBitmap? Poster { get; set; }
    }
}
