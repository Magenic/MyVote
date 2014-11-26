using MonoTouch.UIKit;
using MyVote.UI.Controls;
using MyVote.UI.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(BackgroundExtendedPicker<string>), typeof(BackgroundExtendedPickerRenderer<string>))]
[assembly: ExportRenderer(typeof(BackgroundExtendedPicker<int>), typeof(BackgroundExtendedPickerRenderer<int>))]
namespace MyVote.UI.Renderers
{
    public class BackgroundExtendedPickerRenderer<T> : PickerRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);

            var model = this.Element as BackgroundExtendedPicker<T>;
            if (model != null)
            {
                var control = this.Control as UITextField;
                control.TextColor = model.TextColor.ToUIColor();
            }
        }
    }
}