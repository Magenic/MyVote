using MyVote.BusinessObjects.Contracts;

namespace MyVote.AppServer.Auth
{
	public interface IMyVoteAuthentication
	{
		MyVotePrincipal CurrentPrincipal { get; }
		IUser GetCurrentUser();
		int? GetCurrentUserID();
	}
}