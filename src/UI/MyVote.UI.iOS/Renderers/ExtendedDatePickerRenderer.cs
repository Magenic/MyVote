using System.ComponentModel;
using MyVote.UI.Controls;
using MyVote.UI.Renderers;
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
				if (model.AlternateDisplay != string.Empty)
				{
					control.Text = model.AlternateDisplay;
				}
				control.TextColor = model.TextColor.ToUIColor();
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			if (e.PropertyName == "Date")
			{
				var control = this.Control;
				var model = this.Element as ExtendedDatePicker;
				if (control != null && model != null && model.AlternateDisplay != string.Empty)
				{
					control.Text = model.AlternateDisplay;
				}
			}
		}
	}
}