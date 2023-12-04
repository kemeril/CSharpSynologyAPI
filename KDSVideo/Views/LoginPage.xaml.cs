using KDSVideo.ViewModels;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace KDSVideo.Views
{
    public sealed partial class LoginPage : Page
    {
        private const string TEXT_CONTROL_FOREGROUND_FOCUSED_KEY = "TextControlForegroundFocused";

        public LoginPage()
        {
            InitializeComponent();
        }

        private static Brush GetTextControlForegroundFocusedBrush() => (Brush)Application.Current.Resources[TEXT_CONTROL_FOREGROUND_FOCUSED_KEY];

        private void Host_GotFocus(object sender, RoutedEventArgs _)
        {
            var foreground = GetTextControlForegroundFocusedBrush();
            HostIcon.Foreground = foreground;
        }

        private void Host_LostFocus(object sender, RoutedEventArgs _)
        {
            HostIcon.Foreground = Host.Foreground;
        }

        private void Account_GotFocus(object sender, RoutedEventArgs _)
        {
            AccountIcon.Foreground = GetTextControlForegroundFocusedBrush();
        }

        private void Account_LostFocus(object sender, RoutedEventArgs _)
        {
            AccountIcon.Foreground = Account.Foreground;
        }

        private void Password_GotFocus(object sender, RoutedEventArgs _)
        {
            PasswordIcon.Foreground = GetTextControlForegroundFocusedBrush();
        }

        private void Password_LostFocus(object sender, RoutedEventArgs _)
        {
            PasswordIcon.Foreground = Password.Foreground;
        }

        private void LoginPage_OnProcessKeyboardAccelerators(UIElement sender, ProcessKeyboardAcceleratorEventArgs args)
        {
            if (args is { Key: VirtualKey.Enter, Modifiers: VirtualKeyModifiers.None })
            {
                (DataContext as LoginViewModel)?.LoginCommand.Execute(null);
            }
        }
    }
}
