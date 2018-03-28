using MyVote.BusinessObjects.Contracts;
using System;

namespace MyVote.Services.AppServer.Auth
{
	// TODO: This should be using OAuth authentication
	// and whatnot, but for now, just have 1 user in the DB
	// that has an ID of "1". This HAS to get changed 
	// in the very near future,
	public sealed class MyVoteAuthentication 
		: IMyVoteAuthentication
	{
		private readonly IObjectFactory<IUser> userFactory;

		public MyVoteAuthentication(IObjectFactory<IUser> userFactory)
		{
			if (userFactory == null)
			{
				throw new ArgumentNullException(nameof(userFactory));
			}

			this.userFactory = userFactory;
		}

		public MyVotePrincipal CurrentPrincipal
		{
			get
			{
				return new MyVotePrincipal("test");
			}
		}

		public IUser GetCurrentUser()
		{
			return this.userFactory.Fetch("test");
		}

		public int? GetCurrentUserID()
		{
			return 1;
		}
	}
}