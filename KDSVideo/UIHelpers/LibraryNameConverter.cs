using SynologyAPI.SynologyRestDAL.Vs;

namespace KDSVideo.UIHelpers
{
    public static class LibraryNameConverter
    {
        public static string GetLibraryName(Library library)
        {
            if (library.Id == 0)
            {
                switch (library.LibraryType)
                {
                    case LibraryType.Movie:
                        return "Movies";
                    case LibraryType.TvShow:
                        return "TV Shows";
                    case LibraryType.HomeVideo:
                        return "Home Videos";
                    case LibraryType.TvRecord:
                        return "TV Recordings";
                }
            }

            return string.IsNullOrEmpty(library.Title) ? string.Empty : library.Title;
        }
    }
}
