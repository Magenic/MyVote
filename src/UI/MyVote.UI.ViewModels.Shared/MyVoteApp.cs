using MvvmCross.Core.ViewModels;
using MyVote.UI.Helpers;

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
