using Windows.UI.Xaml.Controls;

namespace KDSVideo.Infrastructure
{
    public interface INavigationServiceEx : GalaSoft.MvvmLight.Views.INavigationService
    {
        Frame CurrentFrame { get; set; }
        void ClearNavigationHistory();
        void ClearContent();
    }
}
