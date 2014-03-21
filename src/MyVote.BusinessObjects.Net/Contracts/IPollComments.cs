using MyVote.BusinessObjects.Core.Contracts;

namespace MyVote.BusinessObjects.Contracts
{
	public interface IPollComments
		: IBusinessBaseCore
	{
		IPollCommentCollection Comments { get; }
		int PollID { get; }
	}
}
