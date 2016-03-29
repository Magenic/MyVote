using Android.Widget;
using MyVote.UI.Controls;
using MyVote.UI.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(BackgroundStringExtendedPicker), typeof(BackgroundExtendedPickerRenderer<string>))]
[assembly: ExportRenderer(typeof(BackgroundIntExtendedPicker), typeof(BackgroundExtendedPickerRenderer<int>))]
namespace MyVote.UI.Renderers
{
	public sealed class BackgroundExtendedPickerRenderer<T> : PickerRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
		{
			base.OnElementChanged(e);

			var model = this.Element as ExtendedPicker<T>;
			if (model != null)
			{
				var control = this.Control as EditText;
				control.SetTextColor(model.TextColor.ToAndroid());
                control.SetBackground(null);

                control.Hint = model.PlaceholderText;
                control.SetHintTextColor(model.PlaceholderColor.ToAndroid());
            }
		}
	}
}