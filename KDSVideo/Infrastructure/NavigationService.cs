using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace KDSVideo.Infrastructure
{
    public class NavigationService : INavigationService
    {
        /// <summary>
        /// The key that is returned by the <see cref="CurrentPageKey"/> property when the current Page is not found.
        /// This can be the case when the navigation wasn't managed by this NavigationService,
        /// for example when it is directly triggered in the code behind, and the
        /// NavigationService was not configured for this page type.
        /// </summary>
        public const string UNKNOWN_PAGE_KEY = "-- UNKNOWN --";

        private readonly Dictionary<string, Type> _pagesByKey = new();
        private readonly Dictionary<string, string> _backNavigationTransitions = new();

        private Frame? _currentFrame;

        ///// <summary>
        ///// Occurs when the content that is being navigated to has been found and is available from the Content property, although it may not have completed loading.
        ///// </summary>
        //public event NavigatedEventHandler Navigated;

#pragma warning disable IDE1006 // Naming Styles
        // ReSharper disable InconsistentNaming
        /// <summary>
        /// Occurs when a new navigation is requested.
        /// </summary>
        public EventHandler<NavigatingCancelEventArgs>? Navigating;

        /// <summary>
        /// Occurs when the content that is being navigated to has been found and is available from the Content property, although it may not have completed loading.
        /// </summary>
        public EventHandler<NavigationEventArgs>? Navigated;
        // ReSharper restore InconsistentNaming
#pragma warning restore IDE1006 // Naming Styles

        /// <summary>
        /// Gets or sets the Frame that should be use for the navigation.
        /// If this is not set explicitly, then (Frame)Window.Current.Content is used.
        /// </summary>
        public Frame? CurrentFrame
        {
            get => _currentFrame ??= Window.Current.Content as Frame;
            set => _currentFrame = value;
        }

        /// <summary>
        /// Gets a flag indicating if the CurrentFrame can navigate backwards.
        /// </summary>
        public bool CanGoBack
        {
            get
            {
                lock (_pagesByKey)
                {
                    return GetPreviousPageKey() != null;
                }
            }
        }

        /// <summary>
        /// The key corresponding to the currently displayed page.
        /// </summary>
        public string CurrentPageKey
        {
            get
            {
                lock (_pagesByKey)
                {
                    return GetCurrentPageKey();
                }
            }
        }

        /// <summary>
        /// If possible, discards the current page and displays the previous page
        /// on the navigation stack.
        /// </summary>
        public void GoBack()
        {
            lock (_pagesByKey)
            {
                var previousPageKey = GetPreviousPageKey();
                if (previousPageKey != null && _pagesByKey.TryGetValue(previousPageKey, out var toPageKey))
                {
                    var navigatingCancelEventArgs = new NavigatingCancelEventArgs(false, NavigationMode.Back, previousPageKey, null);
                    Navigating?.Invoke(this, navigatingCancelEventArgs);
                    if (navigatingCancelEventArgs.Cancel)
                    {
                        return;
                    }

                    lock (_pagesByKey)
                    {
                        CurrentFrame?.Navigate(toPageKey, null);
                        Navigated?.Invoke(this, new NavigationEventArgs(NavigationMode.Back, previousPageKey, null));
                    }
                }
            }
        }

        /// <summary>
        /// Displays a new page corresponding to the given key. 
        /// Make sure to call the <see cref="Configure"/>
        /// method first.
        /// </summary>
        /// <param name="pageKey">The key corresponding to the page
        /// that should be displayed.</param>
        /// <exception cref="ArgumentException">When this method is called for 
        /// a key that has not been configured earlier.</exception>
        public void NavigateTo(string pageKey) => NavigateTo(pageKey, null);

        /// <summary>
        /// Displays a new page corresponding to the given key,
        /// and passes a parameter to the new page.
        /// Make sure to call the <see cref="Configure"/>
        /// method first.
        /// Navigation is recorded in the Frame's ForwardStack or BackStack.
        /// </summary>
        /// <param name="pageKey">The key corresponding to the page
        /// that should be displayed.</param>
        /// <param name="parameter">The parameter that should be passed
        /// to the new page.</param>
        /// <exception cref="ArgumentException">When this method is called for 
        /// a key that has not been configured earlier.</exception>
        public virtual void NavigateTo(string pageKey, object? parameter)
        {
            lock (_pagesByKey)
            {
                if (!_pagesByKey.ContainsKey(pageKey))
                {
                    throw new ArgumentException($"No such page: {pageKey}. Did you forget to call NavigationService.Configure?", nameof(pageKey));
                }

                var currentPageKey = GetCurrentPageKey();
                var navigationMode = pageKey.Equals(currentPageKey, StringComparison.Ordinal)
                    ? NavigationMode.New
                    : NavigationMode.Refresh;
                var navigatingCancelEventArgs = new NavigatingCancelEventArgs(false, navigationMode, pageKey, parameter);
                Navigating?.Invoke(this, navigatingCancelEventArgs);
                if (navigatingCancelEventArgs.Cancel)
                {
                    return;
                }

                CurrentFrame?.Navigate(_pagesByKey[pageKey], parameter);
                Navigated?.Invoke(this, new NavigationEventArgs(navigationMode, pageKey, parameter));
            }
        }

        /// <summary>
        /// Adds a key/page pair to the navigation service.
        /// </summary>
        /// <param name="key">The key that will be used later in the <see cref="NavigateTo(string)"/> or <see cref="NavigateTo(string, object)"/> methods.</param>
        /// <param name="pageType">The type of the page corresponding to the key.</param>
        public NavigationService Configure(string key, Type pageType)
        {

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(key));
            }

            if (pageType == null)
            {
                throw new ArgumentNullException(nameof(pageType));
            }

            lock (_pagesByKey)
            {
                if (_pagesByKey.ContainsKey(key))
                {
                    throw new ArgumentException($"This key is already used: {key}");
                }

                if (_pagesByKey.Any(p => p.Value == pageType))
                {
                    throw new ArgumentException($"This type is already configured with key {_pagesByKey.First(p => p.Value == pageType).Key}");
                }

                _pagesByKey.Add(key, pageType);
            }

            return this;
        }

        /// <summary>
        /// Add back navigation transition configuration.
        /// </summary>
        /// <param name="fromPageKey">The key that defines a page back navigation transition from.</param>
        /// <param name="toPageKey">The key that defines a page back navigation transition to.</param>
        public NavigationService ConfigureBackNavigationTransition(string fromPageKey, string toPageKey)
        {
            if (string.IsNullOrWhiteSpace(fromPageKey))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(fromPageKey));
            }

            if (string.IsNullOrWhiteSpace(toPageKey))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(toPageKey));
            }

            if (fromPageKey == toPageKey)
            {
                throw new ArgumentException("Values must not be equal.", $"{nameof(fromPageKey)}; {nameof(toPageKey)}");
            }

            lock (_pagesByKey)
            {
                if (_backNavigationTransitions.ContainsKey(fromPageKey))
                {
                    throw new ArgumentException($"This key is already used: {fromPageKey}");
                }

                _backNavigationTransitions.Add(fromPageKey, toPageKey);
            }

            return this;
        }

        /// <summary>
        /// Gets the key corresponding to a given page type.
        /// </summary>
        /// <param name="page">The type of the page for which the key must be returned.</param>
        /// <returns>The key corresponding to the page type.</returns>
        public string GetKeyForPage(Type page)
        {
            lock (_pagesByKey)
            {
                if (_pagesByKey.ContainsValue(page))
                {
                    return _pagesByKey.FirstOrDefault(p => p.Value == page).Key;
                }

                throw new ArgumentException($"The page '{page.Name}' is unknown by the NavigationService");
            }
        }

        /// <summary>
        /// Clear content of the CurrentFrame. 
        /// </summary>
        public void ClearContent()
        {
            var currentFrame = CurrentFrame;
            if (currentFrame != null)
            {
                currentFrame.Content = null;
            }
        }

        private string GetCurrentPageKey()
        {
            var currentType = CurrentFrame?.Content?.GetType();
            return currentType != null && _pagesByKey.ContainsValue(currentType)
                ? _pagesByKey.First(i => i.Value == currentType).Key
                : UNKNOWN_PAGE_KEY;
        }

        private string? GetPreviousPageKey()
        {
            var currentPageKey = GetCurrentPageKey();
            if (!string.IsNullOrEmpty(currentPageKey))
            {
                if (_backNavigationTransitions.TryGetValue(currentPageKey, out var toPageKey))
                {
                    if (_pagesByKey.ContainsKey(toPageKey))
                    {
                        return toPageKey;
                    }
                }
            }

            return null;
        }
    }
}
