using SynologyAPI.SynologyRestDAL.Vs;

namespace KDSVideo.UIHelpers
{
    public static class LibraryNameConverter
    {
        public static string GetLibraryName(Library library) =>
            library.IsSystemDefault || string.IsNullOrWhiteSpace(library.Title)
                ? GetDefaultLibraryName(library)
                : library.Title;

        private static string GetDefaultLibraryName(Library library) =>
            library.LibraryType switch
            {
                LibraryType.Movie => "Movies",
                LibraryType.TvShow => "TV Shows",
                LibraryType.HomeVideo => "Home Videos",
                LibraryType.TvRecord => "TV Recordings",
                _ => library.Title ?? string.Empty
            };
    }
}
