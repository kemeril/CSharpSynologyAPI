using System;
using CommonServiceLocator;
using KDSVideo.ViewModels.NavigationViewModels;
using SynologyAPI;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace KDSVideo.Views.NavigationViews.TabViews
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MetaDataItemsAllTabPage : Page
    {
        private readonly IVideoStation _videoStation;

        public MetaDataItemsAllTabPage()
        {
            this.InitializeComponent();
            _videoStation = ServiceLocator.Current.GetInstance<IVideoStation>();
        }

        private void ListViewBase_OnContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            // Sample pattern: https://docs.microsoft.com/en-us/windows/uwp/debug-test-perf/optimize-gridview-and-listview
            if (args.Phase != 0)
            {
                throw new Exception("We should be in phase 0, but we are not.");
            }


            args.RegisterUpdateCallback(ShowImage);
            args.Handled = true;
        }

        private async void ShowImage(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.Phase != 1)
            {
                throw new Exception("We should be in phase 1, but we are not.");
            }

            if (!(args.Item is MediaMetaDataItem mediaMetaDataItem) || mediaMetaDataItem.Poster != null)
            {
                return;
            }

            var templateRoot = args.ItemContainer.ContentTemplateRoot as Grid;
            var image = templateRoot?.Children.OfType<Image>().FirstOrDefault();
            if (image == null || image.Source != null)
            {
                return;
            }

            mediaMetaDataItem.Poster = await mediaMetaDataItem.GetPoster(_videoStation, (uint)image.Width, (uint)image.Height);
            var imageSource = new SoftwareBitmapSource();
            await imageSource.SetBitmapAsync(mediaMetaDataItem.Poster);
            image.Source = imageSource;
        }
    }
}
