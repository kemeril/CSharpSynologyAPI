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
        private const string TextControlForegroundFocusedKey = "TextControlForegroundFocused";

        public LoginPage()
        {
            this.InitializeComponent();
        }

        private Brush GetTextControlForegroundFocusedBrush() => (Brush)Application.Current.Resources[TextControlForegroundFocusedKey];

        private void Host_GotFocus(object sender, RoutedEventArgs e)
        {
            var foreground = GetTextControlForegroundFocusedBrush();
            HostIcon.Foreground = foreground;
        }

        private void Host_LostFocus(object sender, RoutedEventArgs e)
        {
            HostIcon.Foreground = Host.Foreground;
        }

        private void Account_GotFocus(object sender, RoutedEventArgs e)
        {
            AccountIcon.Foreground = GetTextControlForegroundFocusedBrush();
        }

        private void Account_LostFocus(object sender, RoutedEventArgs e)
        {
            AccountIcon.Foreground = Account.Foreground;
        }

        private void Password_GotFocus(object sender, RoutedEventArgs e)
        {
            PasswordIcon.Foreground = GetTextControlForegroundFocusedBrush();
        }

        private void Password_LostFocus(object sender, RoutedEventArgs e)
        {
            PasswordIcon.Foreground = Password.Foreground;
        }

        private void LoginPage_OnProcessKeyboardAccelerators(UIElement sender, ProcessKeyboardAcceleratorEventArgs args)
        {
            if (args.Key == VirtualKey.Enter && args.Modifiers == VirtualKeyModifiers.None)
            {
                (DataContext as LoginViewModel)?.LoginCommand.Execute(null);
            }
        }
    }
}