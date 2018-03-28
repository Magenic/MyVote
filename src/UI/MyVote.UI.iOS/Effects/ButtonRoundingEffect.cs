using System;
using Foundation;
using MyVote.UI.Effects;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName("Magenic")]
[assembly: ExportEffect(typeof(ButtonRoundingEffect), "ButtonRoundingEffect")]
namespace MyVote.UI.Effects
{
    public class ButtonRoundingEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            var control = ((UIButton)this.Control);
            control.Layer.CornerRadius = 10;
            control.ClipsToBounds = true;
        }

        protected override void OnDetached()
        {
        }
    }
}
