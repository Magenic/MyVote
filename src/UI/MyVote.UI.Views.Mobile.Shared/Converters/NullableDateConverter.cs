using System;
using System.Globalization;
using Xamarin.Forms;

namespace MyVote.UI.Converters
{
	public sealed class NullableDateConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var dateTime = (DateTime?)value;

			return dateTime.HasValue ? dateTime.Value : DateTime.Today.Date;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var dateTime = (DateTime)value;

			return dateTime.Date == DateTime.Today.Date ? null : (DateTime?)dateTime;
		}
    }
}
