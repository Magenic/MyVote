using System.Threading;
using System.Web;
using MyVote.BusinessObjects.Contracts;

namespace MyVote.AppServer.Auth
{
	public class MyVoteAuthentication : IMyVoteAuthentication
	{
		public IObjectFactory<IUser> UserFactory { get; set; }

		public static void SetCurrentPrincipal(MyVotePrincipal principal)
		{
			Thread.CurrentPrincipal = principal;
			if (HttpContext.Current != null)
				HttpContext.Current.User = principal;
		}

		public MyVotePrincipal CurrentPrincipal
		{
			get { return Thread.CurrentPrincipal as MyVotePrincipal; }
		}

		public IUser GetCurrentUser()
		{
			if (CurrentPrincipal != null)
			{
				return UserFactory.Fetch(CurrentPrincipal.Identity.Name);
			}
			return null;
		}

		public int? GetCurrentUserID()
		{
			var currentUser = GetCurrentUser();
			if (currentUser != null)
				return currentUser.UserID;
			return null;
		}
	}
}