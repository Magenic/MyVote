using MyVote.BusinessObjects.Contracts;

namespace MyVote.Services.AppServer.Auth
{
	public interface IMyVoteAuthentication
	{
		MyVotePrincipal CurrentPrincipal { get; }
		IUser GetCurrentUser();
		int? GetCurrentUserID();
	}
}