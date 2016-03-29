using System;
using Xamarin.Forms;

namespace MyVote.UI.Controls
{
	public sealed class ExtendedDatePicker : DatePicker
	{
		public static readonly BindableProperty TextColorProperty = BindableProperty.Create<ExtendedDatePicker, Color>(p => p.TextColor, Color.Black);
		public Color TextColor
		{
			get { return (Color)this.GetValue(TextColorProperty); }
			set { SetValue(TextColorProperty, value); }
		}

        public static readonly BindableProperty PlaceholderColorProperty = BindableProperty.Create<ExtendedDatePicker, Color>(p => p.PlaceholderColor, Color.Black);
        public Color PlaceholderColor
        {
            get { return (Color)this.GetValue(PlaceholderColorProperty); }
            set { SetValue(PlaceholderColorProperty, value); }
        }
	}
}