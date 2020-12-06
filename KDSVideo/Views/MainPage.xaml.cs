using System.Diagnostics;
using System.Reflection;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
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

        private void NavigationViewControl_ProcessKeyboardAccelerators(UIElement sender, ProcessKeyboardAcceleratorEventArgs args)
        {
            if (args.Modifiers == VirtualKeyModifiers.Menu && args.Key == VirtualKey.Left ||
                args.Key == VirtualKey.Back)
            {
                NavigateBack();
            }
        }

        private void NavigationViewControl_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            NavigateBack();
        }

        private void NavigateBack()
        {
            if (NavigationViewControl.IsBackEnabled)
            {
                if (ContentFrame.CanGoBack)
                {
                    ContentFrame.GoBack();
                }
                else
                {
                    (DataContext as MainViewModel)?.NavigateCommand.Execute(null);
                }
            }
        }

        private void NavigationViewControl_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            var item = args.SelectedItem as NavigationViewItem;
            var viewName = item?.Tag?.ToString();
            Trace.WriteLine(item?.Tag ?? "Unknown");
            NavigateToView(viewName);
        }

        private bool NavigateToView(string viewName)
        {
            if (string.IsNullOrWhiteSpace(viewName))
            {
                return false;
            }

            var view = Assembly.GetExecutingAssembly()
                .GetType($"KDSVideo.Views.NavigationViews.{viewName}");

            if (view == null)
            {
                return false;
            }

            ContentFrame.Navigate(view, null);
            return true;
        }


        private void ContentFrame_OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            e.Handled = true;
            Trace.WriteLine($"Navigation error occurred: {e.Exception}");
        }
    }
}