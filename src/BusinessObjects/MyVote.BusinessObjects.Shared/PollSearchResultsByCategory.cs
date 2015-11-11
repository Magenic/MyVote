using System.Collections.Generic;
using Csla;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;
using MyVote.BusinessObjects.Core.Contracts;

namespace MyVote.BusinessObjects
{
	[System.Serializable]
	internal sealed class PollSearchResultsByCategory
		: ReadOnlyBaseCore<PollSearchResultsByCategory>, IPollSearchResultsByCategory
	{
#if !NETFX_CORE && !MOBILE
        private void Child_Fetch(List<PollSearchResultsData> results)
		{
			this.Category = results[0].Category;
			var resultList = DataPortal.FetchChild<ReadOnlySwitchList<IPollSearchResult>>();

			resultList.SwitchReadOnlyStatus();

			foreach (var result in results)
			{
				resultList.Add(DataPortal.FetchChild<PollSearchResult>(result));
			}

			resultList.SwitchReadOnlyStatus();
			this.SearchResults = resultList;
		}
#endif

		public static PropertyInfo<string> CategoryProperty =
			PollSearchResultsByCategory.RegisterProperty<string>(_ => _.Category);
		public string Category
		{
			get { return this.ReadProperty(PollSearchResultsByCategory.CategoryProperty); }
			private set { this.LoadProperty(PollSearchResultsByCategory.CategoryProperty, value); }
		}

		public static PropertyInfo<ReadOnlySwitchList<IPollSearchResult>> SearchResultsProperty =
			PollSearchResultsByCategory.RegisterProperty<ReadOnlySwitchList<IPollSearchResult>>(_ => _.SearchResults);
		public IReadOnlyListBaseCore<IPollSearchResult> SearchResults
		{
			get { return this.ReadProperty(PollSearchResultsByCategory.SearchResultsProperty); }
			private set { this.LoadProperty(PollSearchResultsByCategory.SearchResultsProperty, value); }
		}
	}
}
