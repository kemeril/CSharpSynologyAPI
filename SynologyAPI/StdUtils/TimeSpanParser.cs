using System;

namespace StdUtils
{
    public class TimeSpanParser
    {
        public static readonly string[] TimeSpanFormats = {
            @"h\:mm\:ss",
            @"h\:mm\:ss\.FFFFFFF",
            @"h\:mm\:s",
            @"h\:mm\:s\.FFFFFFF",
            @"h\:m\:ss",
            @"h\:m\:ss\.FFFFFFF",
            @"h\:m\:s",
            @"h\:m\:s\.FFFFFFF",
            @"d\:h\:mm\:ss",
            @"d\:h\:mm\:ss\.FFFFFFF",
            @"d\:h\:mm\:s",
            @"d\:h\:mm\:s\.FFFFFFF",
            @"d\:h\:m\:ss",
            @"d\:h\:m\:ss\.FFFFFFF",
            @"d\:h\:m\:s",
            @"d\:h\:m\:s\.FFFFFFF"
        };

        public static TimeSpan Parse(string input, IFormatProvider formatProvider)
        {
            return TimeSpan.ParseExact(input, TimeSpanFormats, formatProvider);
        }

        public static TimeSpan Parse(string input)
        {
            return Parse(input, System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}