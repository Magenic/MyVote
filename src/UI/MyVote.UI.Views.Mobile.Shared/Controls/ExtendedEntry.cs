using Xamarin.Forms;

namespace MyVote.UI.Controls
{
    public sealed class ExtendedEntry : Xamarin.Forms.Entry
    {
        public static readonly BindableProperty HasBorderProperty = BindableProperty.Create<ExtendedEntry, bool>(p => p.HasBorder, true);
        public bool HasBorder
        {
            get { return (bool)this.GetValue(HasBorderProperty); }
            set { SetValue(HasBorderProperty, value); }
        }
    }
}
