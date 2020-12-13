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
        public Frame NavigationFrame => ContentFrame;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void BackInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            args.Handled = true;
            //(DataContext as MainViewModel)?.NavigateCommand.Execute(null);
        }

        private void NavigationViewControl_ProcessKeyboardAccelerators(UIElement sender, ProcessKeyboardAcceleratorEventArgs args)
        {
            if (args.Modifiers == VirtualKeyModifiers.Menu && args.Key == VirtualKey.Left ||
                args.Key == VirtualKey.Back)
            {
                //NavigateBack();
            }
        }
    }
}