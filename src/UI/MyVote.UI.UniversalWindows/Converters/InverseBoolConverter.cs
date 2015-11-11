using System;
using Windows.UI.Xaml.Data;

namespace MyVote.UI.Converters
{
	public sealed class InverseBoolConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			return Invert(value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			return Invert(value);
		}

		private static object Invert(object value)
		{
			var result = false;
			if (value is bool)
			{
				return !(bool)value;
			}

			return result;
		}
	}
}
