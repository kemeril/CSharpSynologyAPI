using KDSVideo.ViewModels;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace KDSVideo.Views
{
    public sealed partial class MainPage : Page
    {
        public Frame NavigationFrame => ContentFrame;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void NavigationViewControl_ProcessKeyboardAccelerators(UIElement sender, ProcessKeyboardAcceleratorEventArgs args)
        {
            if (args.Modifiers == VirtualKeyModifiers.Menu && args.Key == VirtualKey.Left)
            {
                (DataContext as MainViewModel)?.NavigateBackCommand.Execute(null);
            }
        }
    }
}