using MyVote.UI.Helpers;

namespace MyVote.UI
{
	public class MyVoteApp
	{
		public void Initialize()
		{
            Ioc.Container = new Bootstrapper().Bootstrap();
            //RegisterAppStart(new MvxAppStart());
		}
	}
}
