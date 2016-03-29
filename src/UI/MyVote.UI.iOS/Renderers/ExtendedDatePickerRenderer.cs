using System;
using System.ComponentModel;
using MyVote.UI.Controls;
using MyVote.UI.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using DatePicker = Xamarin.Forms.DatePicker;

[assembly: ExportRenderer(typeof(ExtendedDatePicker), typeof(ExtendedDatePickerRenderer))]
namespace MyVote.UI.Renderers
{
	public sealed class ExtendedDatePickerRenderer : DatePickerRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
		{
			base.OnElementChanged(e);
			var model = this.Element as ExtendedDatePicker;
			if (model != null)
			{
				var control = this.Control;
				control.TextColor = IsPlaceholderDate(model.Date) ? model.PlaceholderColor.ToUIColor() : model.TextColor.ToUIColor();
                control.BorderStyle = UITextBorderStyle.None;;
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			if (e.PropertyName == "Date")
			{
				var control = this.Control;
				var model = this.Element as ExtendedDatePicker;
                control.TextColor = IsPlaceholderDate(model.Date) ? model.PlaceholderColor.ToUIColor() : model.TextColor.ToUIColor();
            }
        }

        private bool IsPlaceholderDate(DateTime target)
        {
            if (target.Date == DateTime.Today.Date || target.Date == DateTime.MinValue.Date ||
                target.Date == DateTime.MaxValue.Date || target.Date == DateTime.Parse("1/1/1900"))
            {
                return true;
            }
            return false;
        }
    }
}