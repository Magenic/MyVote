using System.ComponentModel;
using Foundation;
using MyVote.UI.Controls;
using MyVote.UI.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

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
                SetNormalBorder();

                SetErrorBorder();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == "ErrorMessage")
            {
                SetErrorBorder();
            }
        }

        private void SetNormalBorder()
        {
            var model = (ExtendedEntry)Element;
            var control = (UITextField)Control;
            control.BorderStyle = model.HasBorder ? UITextBorderStyle.RoundedRect : UITextBorderStyle.None;
            control.Layer.BorderWidth = model.HasBorder ? 1f : 0f;
            control.AttributedPlaceholder = new NSAttributedString(model.Placeholder, new UIStringAttributes()
            {
                ForegroundColor = ColorExtensions.ToUIColor(model.PlaceholderColor)
            });
        }

        private void SetErrorBorder()
        {
            var model = (ExtendedEntry)Element;
            if (!string.IsNullOrWhiteSpace(model.ErrorMessage))
            {
                var control = (UITextField)this.Control;
                control.BorderStyle = UITextBorderStyle.RoundedRect;
                control.Layer.BorderWidth = 1f;
                control.Layer.BorderColor = ((Color)Application.Current.Resources["ErrorColor"]).ToCGColor();
            }
            else
            {
                SetNormalBorder();
            }
        }
    }
}
