using System;
using Windows.UI.Xaml.Controls;
using KDSVideo.ViewModels.NavigationViewModels;
using KDSVideo.ViewModels.NavigationViewModels.TabViewModels;
using KDSVideo.Views.NavigationViews.TabViews;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace KDSVideo.Views.NavigationViews
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MoviePage : Page
    {
        public MoviePage()
        {
            this.InitializeComponent();
        }

        private void NavigationViewControl_OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            var library = (DataContext as MovieViewModel)?.Library;
            if (library == null)
            {
                return;
            }
            
            var tag = args.InvokedItemContainer.Tag?.ToString();
            switch (tag)
            {
                case "ALL":
                    if (ContentFrame.Navigate(typeof(MetaDataItemsAllTabPage), library))
                    {
                        var page = ContentFrame.Content as MetaDataItemsAllTabPage;
                        (page?.DataContext as MetaDataItemsAllTabViewModel)?.RefreshData(library, false);
                    }
                    break;
                case "BY_FOLDER":
                    break;
                case "JUST_ADDED":
                    break;
                case "JUST_WATCHED":
                    break;
                case "JUST_RELEASED":
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
