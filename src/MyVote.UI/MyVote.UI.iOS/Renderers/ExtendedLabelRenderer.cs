using System.Drawing;

using MonoTouch.UIKit;

using MyVote.UI.Controls;
using MyVote.UI.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ExtendedLabel), typeof(ExtendedLabelRenderer))]
namespace MyVote.UI.Renderers
{
    public class ExtendedLabelRenderer : LabelRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            var model = this.Element as ExtendedLabel;
            if (model != null)
            {
                var control = (UILabel)this.Control;
                control.LayoutMargins.InsetRect(new RectangleF(model.LeftPadding, 0, control.Bounds.Width - model.LeftPadding, control.Bounds.Height));
            }
        }
    }
}