using System;
using Windows.UI.Xaml.Controls;

namespace KDSVideo.Infrastructure
{
    /// <summary>
    /// An interface defining how navigation between pages should
    /// be performed in various frameworks such as Windows,
    /// Windows Phone, Android, iOS etc.
    /// </summary>
    public interface INavigationServiceEx
    {
        /// <summary>
        /// The key corresponding to the currently displayed page.
        /// </summary>
        string CurrentPageKey { get; }

        /// <summary>
        /// Gets the key corresponding to a given page type.
        /// </summary>
        /// <param name="page">The type of the page for which the key must be returned.</param>
        /// <returns>The key corresponding to the page type.</returns>
        string GetKeyForPage(Type page);

        /// <summary>
        /// If possible, instructs the navigation service
        /// to discard the current page and display the previous page
        /// on the navigation stack.
        /// </summary>
        void GoBack();

        /// <summary>
        /// Instructs the navigation service to display a new page
        /// corresponding to the given key. Depending on the platforms,
        /// the navigation service might have to be configured with a
        /// key/page list.
        /// </summary>
        /// <param name="pageKey">The key corresponding to the page
        /// that should be displayed.</param>
        void NavigateTo(string pageKey);

        /// <summary>
        /// Instructs the navigation service to display a new page
        /// corresponding to the given key, and passes a parameter
        /// to the new page.
        /// Depending on the platforms, the navigation service might
        /// have to be Configure with a key/page list.
        /// Navigation is recorded in the Frame's ForwardStack or BackStack.
        /// </summary>
        /// <param name="pageKey">The key corresponding to the page
        /// that should be displayed.</param>
        /// <param name="parameter">The parameter that should be passed
        /// to the new page.</param>
        void NavigateTo(string pageKey, object parameter);

        /// <summary>
        /// Gets or sets the Frame that should be use for the navigation.
        /// If this is not set explicitly, then (Frame)Window.Current.Content is used.
        /// </summary>
        Frame CurrentFrame { get; set; }

        /// <summary>
        /// Gets a flag indicating if the CurrentFrame can navigate backwards.
        /// </summary>
        bool CanGoBack { get; }

        /// <summary>
        /// Gets a flag indicating if the CurrentFrame can navigate forward.
        /// </summary>
        bool CanGoForward { get; }

        /// <summary>
        /// Check if the CurrentFrame can navigate forward, and if yes, performs
        /// a forward navigation.
        /// </summary>
        void GoForward();

        /// <summary>
        /// Clear content of the CurrentFrame. 
        /// </summary>
        void ClearContent();
    }
}
