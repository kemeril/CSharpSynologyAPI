using SynologyRestDAL.Vs;
using Windows.Graphics.Imaging;

namespace KDSVideo.ViewModels.NavigationViewModels
{
    public abstract class MediaMetaDataItem
    {
        protected MediaMetaDataItem(MetaDataItem metaDataItem)
        {
            MetaDataItem = metaDataItem;
        }

        public MetaDataItem MetaDataItem { get; }

        public SoftwareBitmap Poster { get; set; }
    }
}
