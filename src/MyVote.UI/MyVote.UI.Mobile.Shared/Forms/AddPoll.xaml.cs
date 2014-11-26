
using MyVote.UI.Controls;
using MyVote.UI.ViewModels;
using Xamarin.Forms;

namespace MyVote.UI.Forms
{
    public partial class AddPoll : ContentPageBase
	{
		public AddPoll()
		{
			InitializeComponent();

            lblNewPoll.Font = Font.SystemFontOfSize(16, FontAttributes.Bold);
		}
	}
}
