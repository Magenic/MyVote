using MyVote.BusinessObjects.Core.Contracts;

namespace MyVote.BusinessObjects.Contracts
{
	public interface IPollResults
		: IBusinessBaseCore
	{
		int PollID { get; }
		bool IsActive { get; }
		bool IsPollOwnedByUser { get; }
		IPollDataResults PollDataResults { get; }
		IPollComments PollComments { get; }
		string PollImageLink { get; }
	}
}
