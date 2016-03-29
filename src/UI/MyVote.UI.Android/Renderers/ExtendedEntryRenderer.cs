using System.ComponentModel;
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
                }
            }
        }
    }
}
