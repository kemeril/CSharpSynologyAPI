using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace KDSVideo.Views
{
    public sealed partial class ErrorDialog : ContentDialog
    {
        public ErrorDialog(string errorMessage)
        {
            this.InitializeComponent();
            ErrorMessage.Text = errorMessage;
        }
    }
}
