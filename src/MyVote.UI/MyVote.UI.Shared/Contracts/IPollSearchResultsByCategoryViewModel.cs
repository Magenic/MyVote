using System.Collections.Generic;

namespace MyVote.UI.Contracts
{
    public interface IPollSearchResultsByCategoryViewModel
    {
        string Category { get; }
        IList<IPollSearchResultViewModel> SearchResults { get; }
    }
}