using MyVote.UI.Helpers;
using MvvmCross.Core.ViewModels;
using MyVote.UI.ViewModels;

namespace MyVote.UI
{
	public class MyVoteApp : MvxApplication
	{
		public override void Initialize()
		{
		    RegisterAppStart(new MvxAppStart());
		}
	}
}
