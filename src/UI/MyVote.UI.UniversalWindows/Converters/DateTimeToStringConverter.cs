using System;
using Windows.UI.Xaml.Data;

namespace MyVote.UI.Converters
{
	public sealed class DateTimeToStringConverter : IValueConverter
	{
		public bool IsDateTimeNullable { get; set; }

		public object Convert(object value, Type targetType, object parameter, string language)
		{
			var result = string.Empty;
			if (value is DateTime)
			{
				result = ((DateTime)value).ToString("d");
			}

			return result;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}

			DateTime date;
			if (DateTime.TryParse(value.ToString(), out date))
			{
				return date;
			}
			else
			{
				if (this.IsDateTimeNullable)
				{
					return null;
				}
				else
				{
					return new DateTime();
				}
			}
		}
	}
}
