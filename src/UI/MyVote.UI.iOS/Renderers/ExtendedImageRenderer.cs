using MyVote.UI.Controls;
using MyVote.UI.Renderers;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ExtendedImage), typeof(ExtendedImageRenderer))]
namespace MyVote.UI.Renderers
{
	public sealed class ExtendedImageRenderer : ImageRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
		{
			base.OnElementChanged(e);

			if (Control == null || e.OldElement != null || Element == null)
				return;

			double min = Math.Min(Element.Width, Element.Height);
			Control.Layer.CornerRadius = (float)(min / 2.0);
			Control.Layer.MasksToBounds = false;
			Control.Layer.BorderWidth = 0;
			Control.ClipsToBounds = true;
		}


		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (Control == null) return;

			if (e.PropertyName == VisualElement.HeightProperty.PropertyName
				|| e.PropertyName == VisualElement.WidthProperty.PropertyName)
			{
				double min = Math.Min(Element.Width, Element.Height);
				Control.Layer.CornerRadius = (float)(min / 2.0);
				Control.Layer.MasksToBounds = false;
				Control.ClipsToBounds = true;
			}
		}
	}
}