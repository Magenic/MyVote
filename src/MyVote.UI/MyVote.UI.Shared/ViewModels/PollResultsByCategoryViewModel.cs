using System.Collections.Generic;
using System.Collections.ObjectModel;
using MyVote.BusinessObjects.Contracts;
using MyVote.UI.Contracts;

namespace MyVote.UI.ViewModels
{
    public class PollResultsByCategoryViewModel : IPollSearchResultsByCategoryViewModel
    {
        private IPollSearchResultsByCategory pollSearchResultsByCategory;
        private IList<IPollSearchResultViewModel> searchResults;
        private readonly IObjectFactory<IPollSubmissionCommand> objectFactory;
        
        public PollResultsByCategoryViewModel(IPollSearchResultsByCategory pollResultsByCategory
               , IObjectFactory<IPollSubmissionCommand> objectFactory)
        {
            this.pollSearchResultsByCategory = pollResultsByCategory;
            this.searchResults = new ObservableCollection<IPollSearchResultViewModel>();
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

        public IList<IPollSearchResultViewModel> SearchResults
        {
            get { return this.searchResults; }
        }
    }
}