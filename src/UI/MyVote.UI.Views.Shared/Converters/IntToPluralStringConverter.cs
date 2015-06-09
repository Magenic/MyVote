using System;
using Windows.UI.Xaml.Data;

namespace MyVote.UI.Converters
{
	public sealed class IntToPluralStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
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

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}
