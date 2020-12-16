namespace KDSVideo.Infrastructure
{
    public interface INavigable
    {
        void Navigated(in object sender, in NavigationEventArgs args);
    }
}