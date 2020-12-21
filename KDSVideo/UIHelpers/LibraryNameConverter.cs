using SynologyRestDAL.Vs;
using System;

namespace KDSVideo.UIHelpers
{
    public static class LibraryNameConverter
    {
        public static string GetLibraryName(Library library)
        {
            if (library == null)
            {
                return string.Empty;
            }
            
            if (library.Id == 0)
            {
                switch (library.LibraryType)
                {
                    case LibraryType.Movie:
                        return "Movie";
                    case LibraryType.TvShow:
                        return "TV Show";
                    case LibraryType.HomeVideo:
                        return "Home Video";
                    case LibraryType.TvRecord:
                        return "TV Recording";
                    case LibraryType.Unknown:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(library.LibraryType));
                }
            }

            return string.IsNullOrEmpty(library.Title) ? string.Empty : library.Title;
        }
    }
}
