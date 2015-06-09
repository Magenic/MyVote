using MyVote.BusinessObjects.Core.Contracts;

namespace MyVote.BusinessObjects.Contracts
{
	public interface IUserIdentity
		: ICslaIdentityCore
	{
		string ProfileID { get; }
		int? UserID { get; }
		string UserName { get; }
	}
}
