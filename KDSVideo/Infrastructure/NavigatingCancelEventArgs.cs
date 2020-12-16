using System;
using Windows.UI.Xaml.Navigation;

namespace KDSVideo.Infrastructure
{
    /// <summary>
    /// Provides data for the OnNavigatingFrom callback that can be used to cancel a navigation request from origination.
    /// </summary>
    public sealed class NavigatingCancelEventArgs
    {
        public NavigatingCancelEventArgs(NavigationMode navigationMode, string sourcePageKey, object parameter)
        {
            if (string.IsNullOrWhiteSpace(sourcePageKey))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(sourcePageKey));
            }
            
            Cancel = false;
            NavigationMode = navigationMode;
            SourcePageKey = sourcePageKey;
            Parameter = parameter;
        }

        /// <summary>
        /// Specifies whether a pending navigation should be canceled.
        /// </summary>
        /// <returns>**true** to cancel the pending cancelable navigation; **false** to continue with navigation.</returns>
        public bool Cancel { get;  set; }
        
        /// <summary>
        /// Gets the value of the *mode* parameter from the originating Navigate call.
        /// </summary>
        /// <returns>The value of the *mode* parameter from the originating Navigate call.</returns>
        public NavigationMode NavigationMode { get; }

        /// <summary>
        /// Gets the *SourcePageKey* value from the originating Navigate call.
        /// </summary>
        /// <returns>The *SourcePageKey* value.</returns>
        public string SourcePageKey { get; }
        
        /// <summary>
        /// Gets the navigation parameter associated with this navigation.
        /// </summary>
        /// <returns>The navigation parameter.</returns>
        public object Parameter { get; }
    }
}
