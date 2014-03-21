using System;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core.Contracts;
using MyVote.UI.W8.Tests.Mocks.Base;

namespace MyVote.UI.W8.Tests.Mocks
{
	public class PollSearchResultsMock : BusinessBaseCoreMock, IPollSearchResults
	{
		public Func<IReadOnlyListBaseCore<IPollSearchResultsByCategory>> SearchResultsByCategoryDelegate { get; set; }
		public IReadOnlyListBaseCore<IPollSearchResultsByCategory> SearchResultsByCategory
		{
			get
			{
				if (SearchResultsByCategory != null)
				{
					return SearchResultsByCategoryDelegate();
				}
				else
				{
					return null;
				}
			}
		}
	}
}
