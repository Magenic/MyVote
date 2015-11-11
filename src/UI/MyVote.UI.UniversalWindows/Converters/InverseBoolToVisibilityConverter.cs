using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MyVote.UI.Converters
{
	public sealed class InverseBoolToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			var result = Visibility.Collapsed;
			if (value is bool)
			{
				result = (bool)value ? Visibility.Collapsed : Visibility.Visible;
			}

			return result;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}