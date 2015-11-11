using Android.Widget;
using MyVote.UI.Controls;
using MyVote.UI.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

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
				var control = (TextView)this.Control;
				control.SetPadding(model.LeftPadding, 0, 0, 0);
			}
		}
	}
}