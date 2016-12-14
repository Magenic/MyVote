using Xamarin.Forms;

namespace MyVote.UI.Controls
{
	public sealed class ExtendedDatePicker : DatePicker
	{
        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(ExtendedDatePicker), Color.Black);
		public Color TextColor
		{
			get { return (Color)this.GetValue(TextColorProperty); }
			set { SetValue(TextColorProperty, value); }
		}

        public static readonly BindableProperty PlaceholderColorProperty = BindableProperty.Create(nameof(PlaceholderColor), typeof(Color), typeof(ExtendedDatePicker), Color.Black);
        public Color PlaceholderColor
        {
            get { return (Color)this.GetValue(PlaceholderColorProperty); }
            set { SetValue(PlaceholderColorProperty, value); }
        }
	}
}