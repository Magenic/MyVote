using MyVote.BusinessObjects.Core.Contracts;

namespace MyVote.BusinessObjects.Contracts
{
	public interface IPollSubmissionResponse
		: IBusinessBaseCore
	{
		int? PollResponseID { get; }
		int PollOptionID { get; }
		bool IsOptionSelected { get; set; }
		short OptionPosition { get; }
		string OptionText { get; }
	}
}
