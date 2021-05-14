using System;

namespace StdUtils
{
    public class FileSizeUtils
    {
        private const int SCALE = 1024;

        public static string FormatBytes(long bytes)
        {
            var orders = new[] { "GB", "MB", "KB", "Bytes" };
            var max = (long)Math.Pow(SCALE, orders.Length - 1);

            foreach (string order in orders)
            {
                if (bytes > max)
                {
                    return $"{decimal.Divide(bytes, max):##.##} {order}";
                }

                max /= SCALE;
            }

            return "0 Bytes";
        }
    }
}
