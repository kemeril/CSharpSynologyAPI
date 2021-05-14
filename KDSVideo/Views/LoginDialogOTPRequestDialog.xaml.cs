using Windows.UI.Xaml.Controls;
using KDSVideo.ViewModels;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace KDSVideo.Views
{
    public sealed partial class LoginDialogOtpRequestDialog : ContentDialog
    {
        public LoginDialogOtpRequestDialog()
        {
            this.InitializeComponent();
        }

        private void ContentDialog_Closing(ContentDialog _, ContentDialogClosingEventArgs args)
        {
            if (args.Result == ContentDialogResult.Primary
                && DataContext is LoginViewModel dataContext && (dataContext.OtpCode == null || dataContext.OtpCode.Length != 6))
            {
                args.Cancel = true;
            }
        }
    }
}
