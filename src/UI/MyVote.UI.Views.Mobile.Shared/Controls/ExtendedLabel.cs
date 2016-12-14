using Xamarin.Forms;

namespace MyVote.UI.Controls
{
	public sealed class ExtendedLabel : Label
	{
        public static readonly BindableProperty LeftPaddingProperty = BindableProperty.Create(nameof(LeftPadding), typeof(int), typeof(ExtendedLabel), 0);
        public int LeftPadding
        {
            get { return (int)this.GetValue(LeftPaddingProperty); }
            set { SetValue(LeftPaddingProperty, value); }
        }
	}
}
