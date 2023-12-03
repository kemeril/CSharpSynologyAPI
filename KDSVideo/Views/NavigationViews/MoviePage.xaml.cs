using System;
using System.Linq;
using Windows.UI.Xaml.Controls;
using KDSVideo.ViewModels.NavigationViewModels;
using KDSVideo.ViewModels.NavigationViewModels.TabViewModels;
using KDSVideo.Views.NavigationViews.TabViews;
using Windows.UI.Xaml;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace KDSVideo.Views.NavigationViews
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MoviePage : Page
    {
        private bool _forceUpdate = true;

        public MoviePage()
        {
            InitializeComponent();
        }

        private void NavigationViewControl_OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            Refresh(args.InvokedItemContainer.Tag.ToString() ?? string.Empty);
        }

        private void NavigationViewControl_Loaded(object sender, RoutedEventArgs e)
        {
            _forceUpdate = true;
            if (NavigationViewControl.SelectedItem == null && NavigationViewControl.MenuItems.FirstOrDefault(menu => (menu as Framework​Element)?.Tag?.ToString() == "ALL") is NavigationViewItem ALL)
            {
                ALL.IsSelected = true;
            }
            else
            {
                var tag = (NavigationViewControl.MenuItems.FirstOrDefault(item => item is Framework​Element) as FrameworkElement)?.Tag?.ToString() ?? string.Empty;
                Refresh(tag);
            }
        }

        private void Refresh(string tag)
        {
            var library = (DataContext as MovieViewModel)?.Library;
            if (library == null)
            {
                return;
            }

            switch (tag)
            {
                case "ALL":
                    if (ContentFrame.Navigate(typeof(MetaDataItemsAllTabPage), library))
                    {
                        var page = ContentFrame.Content as MetaDataItemsAllTabPage;
                        (page?.DataContext as MetaDataItemsAllTabViewModel)?.RefreshData(library, _forceUpdate);
                        _forceUpdate = false;
                    }
                    break;
                case "BY_FOLDER":
                    break;
                case "JUST_ADDED":
                    // TODO: ORDER BY CreateTime DESC
                    break;
                case "JUST_WATCHED":
                    // TODO: LAST_WATCHED > 0
                    break;
                case "JUST_RELEASED":
                    // TODO: ORDER BY OriginalAvailable DESC
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
