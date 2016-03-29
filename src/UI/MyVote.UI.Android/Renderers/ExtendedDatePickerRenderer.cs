using System;
using System.ComponentModel;
using Android.Widget;
using MyVote.UI.Controls;
using MyVote.UI.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
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
				var control = this.Control as EditText;
				control.SetTextColor(IsPlaceholderDate(model.Date) ? model.PlaceholderColor.ToAndroid() : model.TextColor.ToAndroid());
                control.SetBackground(null);
            }
        }

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			if (e.PropertyName == "Date")
			{
				var control = this.Control as EditText;
				var model = this.Element as ExtendedDatePicker;
                control.SetTextColor(IsPlaceholderDate(model.Date) ? model.PlaceholderColor.ToAndroid() : model.TextColor.ToAndroid());

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