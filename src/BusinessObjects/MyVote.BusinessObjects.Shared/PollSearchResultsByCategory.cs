using Csla;
using MyVote.BusinessObjects.Attributes;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;
using MyVote.BusinessObjects.Core.Contracts;
using System;
using System.Collections.Generic;

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
			var resultList = this.pollSearchResultsFactory.FetchChild();

			resultList.SwitchReadOnlyStatus();

			foreach (var result in results)
			{
				resultList.Add(this.pollSearchResultFactory.FetchChild(result));
			}

			resultList.SwitchReadOnlyStatus();
			this.SearchResults = resultList;
		}

		[NonSerialized]
		private IObjectFactory<ReadOnlySwitchList<IPollSearchResult>> pollSearchResultsFactory;
		[Dependency]
		public IObjectFactory<ReadOnlySwitchList<IPollSearchResult>> PollSearchResultsFactory
		{
			get { return this.pollSearchResultsFactory; }
			set { this.pollSearchResultsFactory = value; }
		}

		[NonSerialized]
		private IObjectFactory<IPollSearchResult> pollSearchResultFactory;
		[Dependency]
		public IObjectFactory<IPollSearchResult> PollSearchResultFactory
		{
			get { return this.pollSearchResultFactory; }
			set { this.pollSearchResultFactory = value; }
		}
#endif

		public static readonly PropertyInfo<string> CategoryProperty =
			PollSearchResultsByCategory.RegisterProperty<string>(_ => _.Category);
		public string Category
		{
			get { return this.ReadProperty(PollSearchResultsByCategory.CategoryProperty); }
			private set { this.LoadProperty(PollSearchResultsByCategory.CategoryProperty, value); }
		}

		public static readonly PropertyInfo<ReadOnlySwitchList<IPollSearchResult>> SearchResultsProperty =
			PollSearchResultsByCategory.RegisterProperty<ReadOnlySwitchList<IPollSearchResult>>(_ => _.SearchResults);
		public IReadOnlyListBaseCore<IPollSearchResult> SearchResults
		{
			get { return this.ReadProperty(PollSearchResultsByCategory.SearchResultsProperty); }
			private set { this.LoadProperty(PollSearchResultsByCategory.SearchResultsProperty, value); }
		}
	}
}
