using System;
using System.Globalization;
using Windows.Globalization.DateTimeFormatting;
using Windows.UI.Xaml.Data;

namespace KDSVideo.UIHelpers
{
    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value == null)
                {
                    return string.Empty;
                }

                // Retrieving the "proper" CurrentCulture
                // https://www.pedrolamas.com/2015/11/02/cultureinfo-changes-in-uwp/
                var cultureName = new DateTimeFormatter("longdate", new[] { "US" }).ResolvedLanguage;
                var cultureInfo = new CultureInfo(cultureName);

                var dateTime = (DateTime)value;
                return dateTime.ToString("d", cultureInfo);
            }
            catch
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
