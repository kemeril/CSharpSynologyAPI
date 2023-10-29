using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SynologyAPI.SynologyRestDAL.Vs;

namespace KDSVideo.UIHelpers
{
    public abstract class NavigationItemBase { }

    public sealed class NavigationCategory : NavigationItemBase, INotifyPropertyChanged
    {
        private bool _isSelected;
        public string Name { get; set; }
        public Library Library { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public sealed class NavigationMenuSeparator : NavigationItemBase { }

    public sealed class NavigationMenuHeader : NavigationItemBase
    {
        public string Name { get; set; }
    }

    public sealed class NavigationViewItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate HeaderTemplate { get; set; }
        public DataTemplate SeparatorTemplate { get; set; }
        public DataTemplate ItemTemplate { get; set; }
        public DataTemplate BuiltInMovieTemplate { get; set; }
        public DataTemplate BuiltInTvShowTemplate { get; set; }
        public DataTemplate MovieTemplate { get; set; }
        public DataTemplate TvShowTemplate { get; set; }
        public DataTemplate TvRecordingTemplate { get; set; }
        public DataTemplate HomeVideoTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            return item is NavigationMenuSeparator
                ? SeparatorTemplate
                : item is NavigationMenuHeader
                    ? HeaderTemplate
                    : item is NavigationCategory navigationCategory
                        ? SelectTemplate(navigationCategory)
                        : null;
        }

        private DataTemplate SelectTemplate(NavigationCategory navigationCategory)
        {
            if (navigationCategory.Library.Id == 0 && navigationCategory.Library.LibraryType == LibraryType.Movie)
            {
                return BuiltInMovieTemplate;
            }

            if (navigationCategory.Library.Id == 0 && navigationCategory.Library.LibraryType == LibraryType.TvShow)
            {
                return BuiltInTvShowTemplate;
            }

            return navigationCategory.Library.LibraryType switch
            {
                LibraryType.Movie => MovieTemplate,
                LibraryType.TvShow => TvShowTemplate,
                LibraryType.HomeVideo => HomeVideoTemplate,
                LibraryType.TvRecord => TvRecordingTemplate,
                LibraryType.Unknown => ItemTemplate,
                _ => throw new ArgumentOutOfRangeException(nameof(navigationCategory.Library.LibraryType), navigationCategory.Library.LibraryType, null)
            };
        }
    }
}
