using MyVote.BusinessObjects.Core;
using MyVote.BusinessObjects.Core.Contracts;

namespace MyVote.BusinessObjects.Contracts
{
	public interface IPollDataResults
		: IReadOnlyBaseCore
	{
		int PollID { get; }
		string Question { get; }
		IReadOnlyListBaseCore<IPollDataResult> Results { get; }
	}
}
