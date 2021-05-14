using KDSVideo.ViewModels;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace KDSVideo.Views
{
    public sealed partial class LoginDialogHistoricalData : ContentDialog
    {
        public LoginDialogHistoricalData()
        {
            this.InitializeComponent();
        }

        private void ContentDialog_Closing(ContentDialog _, ContentDialogClosingEventArgs args)
        {
            if (args.Result == ContentDialogResult.Primary
                && DataContext is LoginViewModel dataContext && dataContext.SelectedHistoricalLoginData == null)
            {
                args.Cancel = true;
            }
        }
    }
}
