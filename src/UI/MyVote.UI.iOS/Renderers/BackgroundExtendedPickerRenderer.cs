using Foundation;
using MyVote.UI.Controls;
using MyVote.UI.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

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
				var control = this.Control as UITextField;
				control.TextColor = model.TextColor.ToUIColor();
                control.BorderStyle = UITextBorderStyle.None;

                control.Placeholder = model.PlaceholderText;
                control.AttributedPlaceholder = new NSAttributedString(model.PlaceholderText, new UIStringAttributes()
                {
                    ForegroundColor = ColorExtensions.ToUIColor(model.PlaceholderColor)
                });
            }
		}
	}
}