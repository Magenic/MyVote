using System.Security.Principal;

namespace MyVote.Services.AppServer.Auth
{
	public class MyVotePrincipal : IPrincipal
	{
		public IIdentity Identity { get; private set; }

		public MyVotePrincipal(string userName)
		{
			Identity = new GenericIdentity(userName);
		}
		
		public bool IsInRole(string role)
		{
			return false;
		}
	}
}