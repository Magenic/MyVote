using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using MyVote.BusinessObjects;
using MyVote.BusinessObjects.Contracts;
using MyVote.UI.Helpers;
using MyVote.UI.NavigationCriteria;

namespace MyVote.UI.ViewModels
{
	public sealed class PollsPageViewModel : PageViewModelBase
	{
		private readonly IObjectFactory<IPollSearchResults> objectFactory;

		public PollsPageViewModel(
			INavigation navigation,
			IObjectFactory<IPollSearchResults> objectFactory)
			: base(navigation)
		{
			this.objectFactory = objectFactory;
			this.SearchOptions = new ObservableCollection<PollSearchOptionViewModel>();
		}


		public async Task SearchPollsAsync()
		{
			this.IsBusy = true;
            if((NavigationCriteria == null) || (string.IsNullOrEmpty(NavigationCriteria.SearchQuery)))
			    this.PollSearchResults = await this.objectFactory.FetchAsync(this.SelectedSearchOption.QueryType);
            else
                this.PollSearchResults = await this.objectFactory.FetchAsync(NavigationCriteria.SearchQuery);


			this.IsBusy = false;
		}

		public void AddPoll()
		{
			this.Navigation.NavigateToViewModel<AddPollPageViewModel>();
		}

		public void ViewPoll(IPollSearchResult poll)
		{
			if (poll == null)
			{
				throw new ArgumentNullException("poll");
			}

			var criteria = new ViewPollPageNavigationCriteria
			{
				PollId = poll.Id
			};

			this.Navigation.NavigateToViewModel<ViewPollPageViewModel>(criteria);
		}

		public void SelectSearchOption(PollSearchOptionViewModel searchOption)
		{
			this.SelectedSearchOption = searchOption;
		}

#if WINDOWS_PHONE
		public void SelectPoll(System.Windows.Controls.SelectionChangedEventArgs eventArgs)
		{
			if (eventArgs != null)
			{
				if (eventArgs.AddedItems != null && eventArgs.AddedItems.Count == 1)
				{
					ViewPoll(eventArgs.AddedItems[0] as IPollSearchResult);
				}
			}
		}
#endif // WINDOWS_PHONE

		protected override void OnInitialize()
		{
		    bool keywordSearch = ((NavigationCriteria != null) && (!string.IsNullOrEmpty(NavigationCriteria.SearchQuery)));
            this.PopulateFilterOptions(keywordSearch);
		}

		protected override void DeserializeParameter(string parameter)
		{
			this.NavigationCriteria = Serializer.Deserialize<PollsPageSearchNavigationCriteria>(parameter);
		}

		private void PopulateFilterOptions(bool keywordSearch)
		{
            if (!keywordSearch)
            {
                this.SearchOptions.Add(new PollSearchOptionViewModel
                {
                    Display = "Most Popular",
                    QueryType = PollSearchResultsQueryType.MostPopular
                });
                this.SearchOptions.Add(new PollSearchOptionViewModel
                {
                    Display = "By Date (Newest First)",
                    QueryType = PollSearchResultsQueryType.Newest
                });
            }
            else
		    {
                this.SearchOptions.Add(new PollSearchOptionViewModel
                {
                    Display = "Keyword Search Result",
                    QueryType = PollSearchResultsQueryType.Newest
                });
		    }
			// TODO: Are these other options actually going to be used?
			//this.FilterOptions.Add(new PollFilterOptionViewModel
			//{
			//	Display = "My Active Polls",
			//	QueryType = PollSearchResultsQueryType.MyActivePolls
			//});
			//this.FilterOptions.Add(new PollFilterOptionViewModel
			//{
			//	Display = "My Past Polls",
			//	QueryType = PollSearchResultsQueryType.MyPastPolls
			//});

			this.SelectedSearchOption = this.SearchOptions.Last();
		}

		private void ExecuteSearch()
		{
			var task = this.SearchPollsAsync();
			var awaiter = task.GetAwaiter();
			awaiter.OnCompleted(() =>
				{
					if (task.Exception != null)
					{
						System.Diagnostics.Debug.WriteLine(task.Exception.Message);
					}
				});
		}

		public ObservableCollection<PollSearchOptionViewModel> SearchOptions { get; private set; }

		private PollSearchOptionViewModel selectedSearchOption;
		public PollSearchOptionViewModel SelectedSearchOption
		{
			get { return this.selectedSearchOption; }
			set
			{
				this.selectedSearchOption = value;
				NotifyOfPropertyChange(() => this.SelectedSearchOption);

				this.ExecuteSearch();
			}
		}

		private IPollSearchResults pollSearchResults;
		public IPollSearchResults PollSearchResults
		{
			get { return this.pollSearchResults; }
			set
			{
				this.pollSearchResults = value;
				NotifyOfPropertyChange(() => this.PollSearchResults);
			}
		}

        private PollsPageSearchNavigationCriteria NavigationCriteria { get; set; }
	}
}
