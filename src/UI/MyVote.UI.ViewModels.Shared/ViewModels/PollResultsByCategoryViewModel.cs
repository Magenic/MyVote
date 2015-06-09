using MyVote.BusinessObjects.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MyVote.UI.ViewModels
{
    public sealed class PollResultsByCategoryViewModel
    {
		private IPollSearchResultsByCategory pollSearchResultsByCategory;
        private IList<PollSearchResultViewModel> searchResults;
        private readonly IObjectFactory<IPollSubmissionCommand> objectFactory;
        
        public PollResultsByCategoryViewModel(IPollSearchResultsByCategory pollResultsByCategory
               , IObjectFactory<IPollSubmissionCommand> objectFactory)
        {
            this.pollSearchResultsByCategory = pollResultsByCategory;
            this.searchResults = new ObservableCollection<PollSearchResultViewModel>();
            this.objectFactory = objectFactory;

            foreach (IPollSearchResult searchResult in pollResultsByCategory.SearchResults)
            {
                this.searchResults.Add(new PollSearchResultViewModel(searchResult, this.objectFactory));
            }
        }
        
        public string Category
        {
            get
            {
                return this.pollSearchResultsByCategory.Category;
            }
        }

        public IList<PollSearchResultViewModel> SearchResults
        {
            get { return this.searchResults; }
        }
    }
}
