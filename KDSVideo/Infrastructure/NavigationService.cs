using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace KDSVideo.Infrastructure
{
    public class NavigationService : INavigationService
    {
        /// <summary>
        /// The key that is returned by the <see cref="CurrentPageKey"/> property when the current Page is the root page.
        /// </summary>
        public const string RootPageKey = "-- ROOT --";

        /// <summary>
        /// The key that is returned by the <see cref="CurrentPageKey"/> property when the current Page is not found.
        /// This can be the case when the navigation wasn't managed by this NavigationService,
        /// for example when it is directly triggered in the code behind, and the
        /// NavigationService was not configured for this page type.
        /// </summary>
        public const string UnknownPageKey = "-- UNKNOWN --";

        private readonly Dictionary<string, Type> _pagesByKey = new Dictionary<string, Type>();
        private readonly Dictionary<string, string> _backNavigationTransitions = new Dictionary<string, string>();

        private Frame _currentFrame;

        /// <summary>
        /// Gets or sets the Frame that should be use for the navigation.
        /// If this is not set explicitly, then (Frame)Window.Current.Content is used.
        /// </summary>
        public Frame CurrentFrame
        {
            get => _currentFrame ?? (_currentFrame = Window.Current.Content as Frame);
            set => _currentFrame = value;
        }

        /// <summary>
        /// Gets a flag indicating if the CurrentFrame can navigate backwards.
        /// </summary>
        public bool CanGoBack => GetPreviousPageKey() != null;

        /// <summary>
        /// Gets a flag indicating if the CurrentFrame can navigate forward.
        /// </summary>
        public bool CanGoForward => false;

        /// <summary>
        /// Check if the CurrentFrame can navigate forward, and if yes, performs
        /// a forward navigation.
        /// </summary>
        public void GoForward()
        {
            if (CanGoForward)
            {
                CurrentFrame.GoForward();
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
                    if (CurrentFrame == null)
                    {
                        return UnknownPageKey;
                    }

                    if (CurrentFrame.BackStackDepth == 0)
                    {
                        return RootPageKey;
                    }

                    if (CurrentFrame.Content == null)
                    {
                        return UnknownPageKey;
                    }

                    var currentType = CurrentFrame.Content.GetType();

                    if (_pagesByKey.All(p => p.Value != currentType))
                    {
                        return UnknownPageKey;
                    }

                    var item = _pagesByKey.FirstOrDefault(
                        i => i.Value == currentType);

                    return item.Key;
                }
            }
        }

        /// <summary>
        /// If possible, discards the current page and displays the previous page
        /// on the navigation stack.
        /// </summary>
        public void GoBack()
        {
            var previousPageKey = GetPreviousPageKey();
            if (previousPageKey != null)
            {
                NavigateTo(previousPageKey);
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
        public void NavigateTo(string pageKey)
        {
            NavigateTo(pageKey, null);
        }

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
        public virtual void NavigateTo(string pageKey, object parameter)
        {
            lock (_pagesByKey)
            {
                if (!_pagesByKey.ContainsKey(pageKey))
                {
                    throw new ArgumentException($"No such page: {pageKey}. Did you forget to call NavigationService.Configure?", nameof(pageKey));
                }

                CurrentFrame.Navigate(_pagesByKey[pageKey], parameter);
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

        private string GetPreviousPageKey() => _backNavigationTransitions.TryGetValue(CurrentPageKey, out var toPageKey)
                ? toPageKey
                : null;
    }
}
