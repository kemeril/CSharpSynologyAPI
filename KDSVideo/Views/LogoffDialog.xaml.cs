using Windows.UI.Xaml.Controls;
using KDSVideo.ViewModels;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace KDSVideo.Views
{
    public sealed partial class LogoffDialog : ContentDialog
    {
        public LogoffDialog()
        {
            this.InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            (DataContext as LogoffViewModel)?.LogoffCommand.Execute(null);
        }
    }
}
