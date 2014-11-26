using MyVote.UI.Forms;
using MyVote.UI.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Polls), typeof(BackgroundPageRenderer))]
[assembly: ExportRenderer(typeof(EditUser), typeof(BackgroundPageRenderer))]
[assembly: ExportRenderer(typeof(AddPoll), typeof(BackgroundPageRenderer))]
[assembly: ExportRenderer(typeof(ViewPoll), typeof(BackgroundPageRenderer))]
[assembly: ExportRenderer(typeof(PollResults), typeof(BackgroundPageRenderer))]
namespace MyVote.UI.Renderers
{
    public class BackgroundPageRenderer : PageRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged(e);

            const string Package = "MyVote.UI.Android";

            if (e.NewElement != null)
            {
                var resId = Resources.GetIdentifier("background", "drawable", Package);
                this.RootView.SetBackgroundResource(resId);
            }
        }
    }
}