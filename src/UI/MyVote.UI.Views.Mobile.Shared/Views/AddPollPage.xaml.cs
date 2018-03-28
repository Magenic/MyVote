
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace MyVote.UI.Views
{
	public partial class AddPollPage : ContentPageBase
	{
		public AddPollPage()
		{
			InitializeComponent();

            Title = "Add Poll";

            pckCategory.On<iOS>().SetUpdateMode(UpdateMode.WhenFinished);
		}
	}
}
