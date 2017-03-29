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

        public static readonly BindableProperty ErrorMessageProperty = BindableProperty.Create(nameof(ErrorMessage), typeof(string), typeof(ExtendedEntry), string.Empty);
        public string ErrorMessage
        {
            get { return (string)GetValue(ErrorMessageProperty); }
            set
            {
                SetValue(ErrorMessageProperty, value); 
            }
        }
    }
}