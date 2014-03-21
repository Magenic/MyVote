using System;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core.Contracts;
using MyVote.UI.W8.Tests.Mocks.Base;

namespace MyVote.UI.W8.Tests.Mocks
{
	public class PollSearchResultsByCategoryMock : BusinessBaseCoreMock, IPollSearchResultsByCategory
	{
		public string Category { get; set; }

		public Func<IReadOnlyListBaseCore<IPollSearchResult>> SearchResultsDelegate { get; set; }
		public IReadOnlyListBaseCore<IPollSearchResult> SearchResults
		{
			get
			{
				if (SearchResultsDelegate != null)
				{
					return SearchResultsDelegate();
				}
				else
				{
					return null;
				}
			}
		}
	}
}
