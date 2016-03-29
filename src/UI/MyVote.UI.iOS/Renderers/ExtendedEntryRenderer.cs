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
                var control = (UITextField)this.Control;
                control.BorderStyle = model.HasBorder ? UITextBorderStyle.RoundedRect : UITextBorderStyle.None;
                control.AttributedPlaceholder = new NSAttributedString(model.Placeholder, new UIStringAttributes()
                {
                    ForegroundColor = ColorExtensions.ToUIColor(model.PlaceholderColor)
                });
            }
        }
    }
}
