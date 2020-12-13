namespace KDSVideo.Infrastructure
{
    public class NavigationServiceEx : GalaSoft.MvvmLight.Views.NavigationService, INavigationServiceEx
    {
        public void ClearNavigationHistory()
        {
            var currentFrame = CurrentFrame;
            if (currentFrame == null)
            {
                return;
            }

            currentFrame.ForwardStack.Clear();
            currentFrame.BackStack.Clear();
        }

        public void ClearContent()
        {
            var currentFrame = CurrentFrame;
            if (currentFrame != null)
            {
                currentFrame.Content = null;
            }
        }
    }
}
