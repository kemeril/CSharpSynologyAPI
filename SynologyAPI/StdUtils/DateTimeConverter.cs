namespace StdUtils
{
    using System;

    public static class DateTimeConverter
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime FromUnixTime(long unixTime)
        {
            return Epoch.AddSeconds(unixTime);
        }

        public static long ToUnixTime(DateTime utcTimestamp)
        {
            if (utcTimestamp.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException("The " + nameof(utcTimestamp) + " must be in Utc format.", nameof(utcTimestamp));
            }

            return Convert.ToInt64((utcTimestamp - Epoch).TotalSeconds);
        }
    }
}