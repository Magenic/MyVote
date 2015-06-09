using MyVote.UI.Helpers;

namespace MyVote.UI.Views
{
    public partial class LandingPagePhone : ContentPageBase
	{
		public LandingPagePhone()
		{
			InitializeComponent();

			this.ResolveAutofacDependencies();

			//Issue with PCL, need to reference something in Xamarin.Forms.Labs or it gets compiled out
			var imageButton = btnMicrosoft;
		}
	}
}
