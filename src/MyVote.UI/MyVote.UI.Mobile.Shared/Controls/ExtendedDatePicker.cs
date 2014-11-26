using System;
using Xamarin.Forms;

namespace MyVote.UI.Controls
{
    public class ExtendedDatePicker : DatePicker
    {
        public static readonly BindableProperty TextColorProperty = BindableProperty.Create<ExtendedDatePicker, Color>(p => p.TextColor, Color.Black);
        public Color TextColor
        {
            get { return (Color)this.GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value);}
        }

        public string AlternateDisplay
        {
            get
            {
                var returnValue = string.Empty;
                if (Date == DateTime.MinValue || Date == DateTime.MaxValue)
                {
                    returnValue = "Select a date";
                }
                return returnValue;
            }
        }        
    }
}