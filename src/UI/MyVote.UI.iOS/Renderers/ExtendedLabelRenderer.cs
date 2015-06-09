using System.Drawing;

using MyVote.UI.Controls;
using MyVote.UI.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;

[assembly: ExportRenderer(typeof(ExtendedLabel), typeof(ExtendedLabelRenderer))]
namespace MyVote.UI.Renderers
{
	public sealed class ExtendedLabelRenderer : LabelRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
		{
			base.OnElementChanged(e);

			var model = this.Element as ExtendedLabel;
			if (model != null)
			{
				var control = (UILabel)this.Control;
				control.LayoutMargins.InsetRect(new RectangleF(model.LeftPadding, 0, (float)(control.Bounds.Width - model.LeftPadding), (float)control.Bounds.Height));
			}
		}
	}
}