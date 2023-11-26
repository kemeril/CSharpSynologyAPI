using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SynologyAPI.SynologyRestDAL.Vs;
using CommunityToolkit.Mvvm.ComponentModel;
// ReSharper disable RedundantTypeDeclarationBody
// ReSharper disable ConvertToPrimaryConstructor

namespace KDSVideo.UIHelpers
{
    public abstract class NavigationItemBase { }

    [ObservableObject]
    public partial class NavigationCategory : NavigationItemBase
    {
        public NavigationCategory(string name, Library library)
        {
            Name = name;
            Library = library;
        }

        [ObservableProperty]
        private bool _isSelected;

        public string Name { get; }

        public Library Library { get; }
    }

    public sealed class NavigationMenuSeparator : NavigationItemBase { }

    public sealed class NavigationMenuHeader : NavigationItemBase
    {
        public string Name { get; set; } = string.Empty;
    }

    public sealed class NavigationViewItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? HeaderTemplate { get; set; }
        public DataTemplate? SeparatorTemplate { get; set; }
        public DataTemplate? ItemTemplate { get; set; }
        public DataTemplate? BuiltInMovieTemplate { get; set; }
        public DataTemplate? BuiltInTvShowTemplate { get; set; }
        public DataTemplate? MovieTemplate { get; set; }
        public DataTemplate? TvShowTemplate { get; set; }
        public DataTemplate? TvRecordingTemplate { get; set; }
        public DataTemplate? HomeVideoTemplate { get; set; }

        protected override DataTemplate? SelectTemplateCore(object item) =>
            item switch
            {
                NavigationMenuSeparator => SeparatorTemplate,
                NavigationMenuHeader => HeaderTemplate,
                NavigationCategory navigationCategory => navigationCategory.Library switch
                {
                    { Id: 0, LibraryType: LibraryType.Movie } => BuiltInMovieTemplate,
                    { Id: 0, LibraryType: LibraryType.TvShow } => BuiltInTvShowTemplate,
                    _ => navigationCategory.Library.LibraryType switch
                    {
                        LibraryType.Movie => MovieTemplate,
                        LibraryType.TvShow => TvShowTemplate,
                        LibraryType.HomeVideo => HomeVideoTemplate,
                        LibraryType.TvRecord => TvRecordingTemplate,
                        LibraryType.Unknown => ItemTemplate,
                        _ => null
                    }
                },
                _ => null
            };
    }
}
