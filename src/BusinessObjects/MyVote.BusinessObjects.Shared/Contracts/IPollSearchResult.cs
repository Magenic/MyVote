using MyVote.BusinessObjects.Core.Contracts;

namespace MyVote.BusinessObjects.Contracts
{
	public interface IPollSearchResult
		: IReadOnlyBaseCore
	{
		int Id { get; }
		string ImageLink { get; }
		string Question { get; }
		int SubmissionCount { get; }
	}
}
