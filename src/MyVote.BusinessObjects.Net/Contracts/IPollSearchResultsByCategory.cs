using MyVote.BusinessObjects.Core.Contracts;

namespace MyVote.BusinessObjects.Contracts
{
	public interface IPollSearchResultsByCategory
		: IReadOnlyBaseCore
	{
		string Category { get; }
		IReadOnlyListBaseCore<IPollSearchResult> SearchResults { get; }
	}
}
