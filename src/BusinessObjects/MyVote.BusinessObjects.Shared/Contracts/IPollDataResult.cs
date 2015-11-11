using MyVote.BusinessObjects.Core.Contracts;

namespace MyVote.BusinessObjects.Contracts
{
	public interface IPollDataResult
		: IReadOnlyBaseCore
	{
		string OptionText { get; }
		int PollOptionID { get; }
		int ResponseCount { get; }
	}
}
