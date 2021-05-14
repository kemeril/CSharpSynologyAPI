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
                        return "Movies";
                    case LibraryType.TvShow:
                        return "TV Shows";
                    case LibraryType.HomeVideo:
                        return "Home Videos";
                    case LibraryType.TvRecord:
                        return "TV Recordings";
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
