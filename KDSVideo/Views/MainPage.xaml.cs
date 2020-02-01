using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using KDSVideo.ViewModels;

namespace KDSVideo.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            //var goBack = new KeyboardAccelerator {Key = VirtualKey.GoBack};
            //goBack.Invoked += BackInvoked;
            //KeyboardAccelerators.Add(goBack);
        }

        private void BackInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            args.Handled = true;
            (DataContext as MainViewModel)?.NavigateCommand.Execute(null);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            //e.Cancel = e.NavigationMode == NavigationMode.Back;
        }
    }
}