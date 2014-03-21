using MyVote.BusinessObjects.Core.Contracts;

namespace MyVote.BusinessObjects.Contracts
{
	public interface IPollSearchResults
		: IReadOnlyBaseCore
	{
		IReadOnlyListBaseCore<IPollSearchResultsByCategory> SearchResultsByCategory { get; }
	}
}
