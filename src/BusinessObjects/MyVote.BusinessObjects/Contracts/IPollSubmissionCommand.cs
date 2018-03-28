using MyVote.BusinessObjects.Core.Contracts;

namespace MyVote.BusinessObjects.Contracts
{
	public interface IPollSubmissionCommand
		: ICommandBaseCore
	{
		int PollID { get; set; }
		IPollSubmission Submission { get; }
		int UserID { get; set; }
	}
}
