using System;
using Windows.UI.Xaml.Navigation;

namespace KDSVideo.Infrastructure
{
    /// <summary>Provides data for navigation methods and event handlers that cannot cancel the navigation request.</summary>
    public sealed class NavigationEventArgs
    {
        public NavigationEventArgs(NavigationMode navigationMode, string sourcePageKey, object? parameter)
        {
            if (string.IsNullOrWhiteSpace(sourcePageKey))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(sourcePageKey));
            }

            NavigationMode = navigationMode;
            SourcePageKey = sourcePageKey;
            Parameter = parameter;
        }

        /// <summary>
        /// Gets a value that indicates the direction of movement during navigation.
        /// </summary>
        /// <returns>A value of the enumeration.</returns>
        public NavigationMode NavigationMode { get; }

        /// <summary>
        /// Gets the *SourcePageKey* value from the originating Navigate call.
        /// </summary>
        /// <returns>The *SourcePageKey* value.</returns>
        public string SourcePageKey { get; }

        /// <summary>
        /// Gets any "Parameter" object passed to the target page for the navigation.
        /// </summary>
        /// <returns>An object that potentially passes parameters to the navigation target. May be null.</returns>
        public object? Parameter { get; }
    }
}
