using Xamarin.Forms;

namespace MyVote.UI.Controls
{
    public class CheckboxCell : Cell
    {
        public CheckboxCell()
        {
        }

        public static readonly BindableProperty TextProperty = BindableProperty.Create<CheckboxCell, string>(p => p.Text, string.Empty);
        public string Text
        {
            get { return (string)this.GetValue(TextProperty); }
            set { this.SetValue(TextProperty, value);}
        }

        public static readonly BindableProperty CheckedProperty = BindableProperty.Create<CheckboxCell, bool>(p => p.Checked, false);
        public bool Checked
        {
            get { return (bool)this.GetValue(CheckedProperty); }
            set { this.SetValue(CheckedProperty, value); }
        }
    }
}
