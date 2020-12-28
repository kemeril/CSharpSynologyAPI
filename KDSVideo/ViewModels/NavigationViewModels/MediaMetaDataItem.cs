using SynologyRestDAL.Vs;
using System;
using Windows.Graphics.Imaging;

namespace KDSVideo.ViewModels.NavigationViewModels
{
    public abstract class MediaMetaDataItem
    {
        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(30);

        protected MediaMetaDataItem(MetaDataItem metaDataItem)
        {
            MetaDataItem = metaDataItem;
        }

        public MetaDataItem MetaDataItem { get; }

        public SoftwareBitmap Poster { get; set; }
    }
}