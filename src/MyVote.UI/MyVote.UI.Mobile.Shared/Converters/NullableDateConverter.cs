using System;
using System.Globalization;
using Xamarin.Forms;

namespace MyVote.UI.Converters
{
    public class NullableDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dateTime = (DateTime?) value;

            return dateTime.HasValue ? dateTime.Value : DateTime.MinValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dateTime = (DateTime) value;

            return dateTime == DateTime.MinValue ? null : (DateTime?) dateTime;
        }
    }
}