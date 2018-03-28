using System.ComponentModel;
using Android.Support.V4.Content;
using Android.Widget;
using MyVote.UI.Controls;
using MyVote.UI.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ExtendedEntry), typeof(ExtendedEntryRenderer))]
namespace MyVote.UI.Renderers
{
    public sealed class ExtendedEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            var model = this.Element as ExtendedEntry;
            if (model != null)
            {
                var control = (EditText)this.Control;
                control.SetHintTextColor(model.PlaceholderColor.ToAndroid());
                if (!model.HasBorder)
                {
                    control.SetBackground(null);
                    if (!string.IsNullOrWhiteSpace(model.ErrorMessage))
                    {
                        control.SetError(model.ErrorMessage, ContextCompat.GetDrawable(Context, Resource.Drawable.ic_error_white_24dp));

                        control.Error = model.ErrorMessage;
                    }
                }
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            var model = this.Element as ExtendedEntry;
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
            if (e.PropertyName == "Text" && !string.IsNullOrEmpty(model.ErrorMessage))
            {
                control.Error = model.ErrorMessage;
            }
        }
    }
}
