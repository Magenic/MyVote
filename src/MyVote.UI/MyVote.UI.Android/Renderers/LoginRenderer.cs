using MyVote.UI.Forms;
using MyVote.UI.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(EditUser), typeof(LoginRenderer))]
[assembly: ExportRenderer(typeof(Login), typeof(LoginRenderer))]
namespace MyVote.UI.Renderers
{
    public class LoginRenderer : PageRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged(e);

            const string Package = "MyVote.UI.Android";

            if (e.NewElement != null)
            {
                var resId = Resources.GetIdentifier("login_background", "drawable", Package);
                this.RootView.SetBackgroundResource(resId);
            }
        }
    }
}