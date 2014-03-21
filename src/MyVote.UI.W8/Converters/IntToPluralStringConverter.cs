using System;
#if WINDOWS_PHONE
using System.Windows.Data;
using System.Windows;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#endif // WINDOWS_PHONE

namespace MyVote.UI.Converters
{
	public sealed class IntToPluralStringConverter : IValueConverter
	{
#if WINDOWS_PHONE
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
#else
		public object Convert(object value, Type targetType, object parameter, string language)
#endif // WINDOWS_PHONE
		{
			if (parameter == null)
			{
				return string.Empty;
			}

			var result = parameter.ToString();
			if (value is int)
			{
				if ((int)value == 0 || (int)value >= 2)
				{
					result += "s";
				}
			}

			return result;
		}

#if WINDOWS_PHONE
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
#else
		public object ConvertBack(object value, Type targetType, object parameter, string language)
#endif // WINDOWS_PHONE
		{
			throw new NotImplementedException();
		}
	}
}
