using System.Linq;
using MonoTouch.UIKit;
using MyVote.UI.Forms;
using MyVote.UI.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Polls), typeof(BackgroundPageRenderer))]
[assembly: ExportRenderer(typeof(EditUser), typeof(BackgroundPageRenderer))]
[assembly: ExportRenderer(typeof(AddPoll), typeof(BackgroundPageRenderer))]
[assembly: ExportRenderer(typeof(ViewPoll), typeof(BackgroundPageRenderer))]
[assembly: ExportRenderer(typeof(PollResults), typeof(BackgroundPageRenderer))]
namespace MyVote.UI.Renderers
{
    public class BackgroundPageRenderer : PageRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            e.NewElement.SizeChanged += OnSizeChanged;

            if (e.NewElement != null)
            {
                this.SetBackground();
            }
        }

        private void SetBackground()
        {
            const string Landscape = "Images/background_logo_2048X1496@2x.png";
            const string Portrait = "Images/Default-Portrait.png";
            const int backgroundViewTag = 989555;

            var resourceName = this.ViewController.InterfaceOrientation == UIInterfaceOrientation.LandscapeRight
                               || this.ViewController.InterfaceOrientation == UIInterfaceOrientation.LandscapeLeft
                                   ? Landscape
                                   : Portrait;

            var foundView = this.View.Subviews.SingleOrDefault(v => v.Tag == backgroundViewTag);
            if (foundView != null)
            {
                foundView.RemoveFromSuperview();
            }

            var newView = new UIImageView(UIImage.FromBundle(resourceName));
            newView.Tag = backgroundViewTag;
            this.View.InsertSubview(newView, 0);
        }

        private void OnSizeChanged(object sender, System.EventArgs e)
        {
            this.SetBackground();
        }
    }
}