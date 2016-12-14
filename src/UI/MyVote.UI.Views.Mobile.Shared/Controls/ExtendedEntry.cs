using Xamarin.Forms;

namespace MyVote.UI.Controls
{
    public sealed class ExtendedEntry : Entry
    {
        public static readonly BindableProperty HasBorderProperty = BindableProperty.Create(nameof(HasBorder), typeof(bool), typeof(ExtendedEntry), true);
        public bool HasBorder
        {
            get { return (bool)GetValue(HasBorderProperty); }
            set { SetValue(HasBorderProperty, value); }
        }
    }
}
