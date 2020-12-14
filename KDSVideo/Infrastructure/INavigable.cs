using Windows.UI.Xaml.Navigation;

namespace KDSVideo.Infrastructure
{
    public interface INavigable
    {
        void Navigated(in object sender, in NavigationEventArgs e);
    }
}