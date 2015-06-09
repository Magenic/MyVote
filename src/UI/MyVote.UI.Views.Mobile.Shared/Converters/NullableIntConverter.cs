using System;
using System.Globalization;
using Xamarin.Forms;

namespace MyVote.UI.Converters
{
	public sealed class NullableIntConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var number = (int?)value;

			return number.HasValue ? number.Value : 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var number = (int)value;

			return number == 0 ? null : (int?)number;
		}
	}
}