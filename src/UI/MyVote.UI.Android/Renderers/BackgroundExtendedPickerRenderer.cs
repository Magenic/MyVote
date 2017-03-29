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
                control.SetBackground(null);

                control.Hint = model.PlaceholderText;
                control.SetHintTextColor(model.PlaceholderColor.ToAndroid());

                if (!string.IsNullOrWhiteSpace(model.ErrorMessage))
                {
                    control.SetError(model.ErrorMessage, Resources.GetDrawable(Resource.Drawable.ic_error_white_24dp, Context.Theme));
                    control.Error = model.ErrorMessage;
                }
            }
		}

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            var model = this.Element as ExtendedPicker<T>;
            var control = (EditText)Control;
            if (e.PropertyName == "ErrorMessage")
            {
                if (!string.IsNullOrWhiteSpace(model.ErrorMessage))
                {
                    control.Error = model.ErrorMessage;
                }
                else
                {
                    control.Error = null;
                }

            }
            else if (e.PropertyName == "Text" && !string.IsNullOrEmpty(model.ErrorMessage))
            {
                control.Error = model.ErrorMessage;
            }
        }
	}
}