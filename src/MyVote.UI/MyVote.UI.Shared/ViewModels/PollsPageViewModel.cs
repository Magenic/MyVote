using Cirrious.MvvmCross.ViewModels;
using MyVote.BusinessObjects;
using MyVote.BusinessObjects.Contracts;
using MyVote.UI.Contracts;
using MyVote.UI.Helpers;
using MyVote.UI.NavigationCriteria;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin;

namespace MyVote.UI.ViewModels
{
    public sealed class PollsPageViewModel : PageViewModelBase
    {
        private readonly IObjectFactory<IPollSearchResults> objectFactory;
        private readonly IObjectFactory<IPollSubmissionCommand> pollSubmissionFactory;
        private readonly IAppSettings appSettings;

        public PollsPageViewModel(IObjectFactory<IPollSearchResults> objectFactory,
            IObjectFactory<IPollSubmissionCommand> pollSubmissionFactory,
            IAppSettings appSettings)
        {
            this.objectFactory = objectFactory;
            this.pollSubmissionFactory = pollSubmissionFactory;
            this.SearchOptions = new ObservableCollection<PollSearchOptionViewModel>();
            this.PollSearchResults = new ObservableCollection<IPollSearchResultsByCategoryViewModel>();
            this.appSettings = appSettings;
        }

        public async Task SearchPollsAsync()
        {
            this.IsBusy = true;
            if ((this.NavigationCriteria == null) || (string.IsNullOrEmpty(this.NavigationCriteria.SearchQuery)))
            {
                var searchResults = await this.objectFactory.FetchAsync(this.SelectedSearchOption.QueryType);
                this.LoadSearchResults(searchResults);
            }
            else
            {
                var searchResults = await this.objectFactory.FetchAsync(this.NavigationCriteria.SearchQuery);
                this.LoadSearchResults(searchResults);
            }

            this.IsBusy = false;
        }

        private void LoadSearchResults(IPollSearchResults searchResults)
        {
            var viewModel = new ObservableCollection<IPollSearchResultsByCategoryViewModel>();
            foreach (var categoryResult in searchResults.SearchResultsByCategory)
            {
                viewModel.Add(new PollResultsByCategoryViewModel(categoryResult, pollSubmissionFactory));
            }
            this.PollSearchResults = viewModel;
        }

        public ICommand AddPoll
        {
            get
            {
                return new MvxCommand(() => this.ShowViewModel<AddPollPageViewModel>());
            }
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

        public override void Start()
        {
            bool keywordSearch = ((NavigationCriteria != null) && (!string.IsNullOrEmpty(NavigationCriteria.SearchQuery)));
            this.PopulateFilterOptions(keywordSearch);
        }

        public void Init(PollsPageSearchNavigationCriteria criteria)
        {
            this.NavigationCriteria = criteria;
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
            this.IsBusy = true;
            var task = this.SearchPollsAsync();
            var awaiter = task.GetAwaiter();
            awaiter.OnCompleted(() =>
            {
                this.IsBusy = false;
                if (task.Exception != null)
                {
                    System.Diagnostics.Debug.WriteLine(task.Exception.Message);
                    Insights.Report(task.Exception);
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
                this.RaisePropertyChanged(() => this.SelectedSearchOption);

                this.ExecuteSearch();
            }
        }

        private ObservableCollection<IPollSearchResultsByCategoryViewModel> pollSearchResults;
        public ObservableCollection<IPollSearchResultsByCategoryViewModel> PollSearchResults
        {
            get { return this.pollSearchResults; }
            set
            {
                this.pollSearchResults = value;
                this.RaisePropertyChanged(() => this.PollSearchResults);
            }
        }

        private PollsPageSearchNavigationCriteria NavigationCriteria { get; set; }

        public ICommand Logout
        {
            get
            {
                return new MvxCommand(this.LogoutCommand);
            }
        }

        public void LogoutCommand()
        {
            this.DoLogout();
            this.Close(this);
        }

        public void DoLogout()
        {
            this.appSettings.Remove(SettingsKeys.ProfileId);
            Csla.ApplicationContext.User = null;
        }

        public ICommand EditProfile
        {
            get
            {
                return new MvxCommand(this.EditProfileCommand);
            }
        }

        public void EditProfileCommand()
        {
            var profileId = ((IUserIdentity)Csla.ApplicationContext.User.Identity).ProfileID;
            var criteria = new RegistrationPageNavigationCriteria
			{
				ProfileId = profileId,
                 ExistingUser = true
			};

			this.ShowViewModel<RegistrationPageViewModel>(criteria);
        }
    }
}