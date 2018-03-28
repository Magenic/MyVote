using MyVote.BusinessObjects;
using MyVote.BusinessObjects.Contracts;
using MyVote.UI.Helpers;
using MyVote.UI.NavigationCriteria;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Linq;
using Csla.Security;
using MyVote.UI.Contracts;
using System;

namespace MyVote.UI.ViewModels
{
	public sealed class PollsPageViewModel : NavigatingViewModelBase
    {
		private readonly IObjectFactory<IPollSearchResults> objectFactory;
		private readonly IObjectFactory<IPollSubmissionCommand> pollSubmissionFactory;
		private readonly IAppSettings appSettings;
		private readonly ILogger logger;

		public PollsPageViewModel(
			IObjectFactory<IPollSearchResults> objectFactory,
			IObjectFactory<IPollSubmissionCommand> pollSubmissionFactory,
			IAppSettings appSettings,
			ILogger logger,
            INavigationService navigationService) : base(navigationService)
		{
			this.objectFactory = objectFactory;
			this.pollSubmissionFactory = pollSubmissionFactory;
			this.SearchOptions = new ObservableCollection<PollSearchOptionViewModel>();
			this.PollSearchResults = new ObservableCollection<PollResultsByCategoryViewModel>();
			this.appSettings = appSettings;
			this.logger = logger;
		}

		public async Task SearchPollsAsync()
		{
			try
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
			catch (System.Exception ex)
			{
				var blah = 0;
			}
		}

		private void LoadSearchResults(IPollSearchResults searchResults)
		{
#if __MOBILE__
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
#endif
			var viewModel = new ObservableCollection<PollResultsByCategoryViewModel>();
			foreach (var categoryResult in searchResults.SearchResultsByCategory)
			{
                viewModel.Add(new PollResultsByCategoryViewModel(categoryResult, pollSubmissionFactory));
			}
			this.PollSearchResults = viewModel;
            var cats = viewModel.Select(item => item.Category).Distinct();
            var newCats = new ObservableCollection<string>(cats);
            newCats.Insert(0, "All");
            this.Categories = newCats;
#if __MOBILE__
            });
#endif
		}

		public ICommand AddPoll
		{
			get
			{
				return new Command(() => navigationService.ShowViewModel<AddPollPageViewModel>());
			}
		}

		public void SelectSearchOption(PollSearchOptionViewModel searchOption)
		{
			this.SelectedSearchOption = searchOption;
		}

		public override void Start()
		{
			bool keywordSearch = ((NavigationCriteria != null) && (!string.IsNullOrEmpty(NavigationCriteria.SearchQuery)));
			this.PopulateFilterOptions(keywordSearch);
            this.Categories = new ObservableCollection<string>();
            this.Categories.Add("All");
            this.SelectedCategory = "All";
		}

		public override void Init(object parameter)
		{
			this.NavigationCriteria = (PollsPageSearchNavigationCriteria)parameter;
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

			this.SelectedSearchOption = this.SearchOptions.Last();
		}

		public void ExecuteSearch()
		{
			this.IsBusy = true;
			var task = this.SearchPollsAsync();
			var awaiter = task.GetAwaiter();
			awaiter.OnCompleted(() =>
			{
				this.IsBusy = false;
				if (task.Exception != null)
				{
					this.logger.Log(task.Exception);
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
			    if (value != null && value != this.selectedSearchOption)
			    {
                    this.selectedSearchOption = value;
                    this.RaisePropertyChanged(nameof(SelectedSearchOption));

                    this.ExecuteSearch();			        
			    }
			}
		}

        private ObservableCollection<String> categories;
        public ObservableCollection<String> Categories
        {
            get { return this.categories; }
            set
            {
                this.categories = value;
                this.RaisePropertyChanged(nameof(Categories));
            }
        }

        private String selectedCategory;
        public String SelectedCategory
        {
            get { return this.selectedCategory; }
            set
            {
                if (value != null && value != this.selectedCategory)
                {
                    this.selectedCategory = value;
                    this.RaisePropertyChanged(nameof(SelectedCategory));
                    this.RaisePropertyChanged(nameof(FilteredPollSearchResults));
                }
            }
        }

		private ObservableCollection<PollResultsByCategoryViewModel> pollSearchResults;
		public ObservableCollection<PollResultsByCategoryViewModel> PollSearchResults
		{
			get { return this.pollSearchResults; }
			set
			{
				this.pollSearchResults = value;
                this.RaisePropertyChanged(nameof(PollSearchResults));
                this.RaisePropertyChanged(nameof(FilteredPollSearchResults));
			}
		}

        public ObservableCollection<PollResultsByCategoryViewModel> FilteredPollSearchResults
        {
            get 
            {
                if (this.SelectedCategory == "All")
                {
                    return this.PollSearchResults;
                }
                else
                {
                    return new ObservableCollection<PollResultsByCategoryViewModel>(this.PollSearchResults.Where(p => p.Category == this.SelectedCategory));
                }
            }
        }

		private PollsPageSearchNavigationCriteria NavigationCriteria { get; set; }

		public ICommand Logout
		{
			get
			{
				return new Command(this.LogoutCommand);
			}
		}

		public void LogoutCommand()
		{
		    this.IsBusy = true;
		    this.IsEnabled = false;
            this.DoLogout();
            navigationService.Close();
            navigationService.ShowViewModel<LandingPageViewModel>();
#if !MOBILE
            //this.Close(this);
#endif
            this.IsBusy = false;
        }

		public void DoLogout()
		{
			this.appSettings.Remove(SettingsKeys.ProfileId);
            Csla.ApplicationContext.User = new UnauthenticatedPrincipal();
		}

		public ICommand EditProfile
		{
			get
			{
				return new Command(this.EditProfileCommand);
			}
		}

		public void EditProfileCommand()
		{
		    this.IsBusy = true;
            this.IsEnabled = false;
            var profileId = ((IUserIdentity)Csla.ApplicationContext.User.Identity).ProfileID;
			var criteria = new RegistrationPageNavigationCriteria
			{
				ProfileId = profileId,
				ExistingUser = true
			};

			navigationService.ShowViewModel<RegistrationPageViewModel>(criteria);
            this.IsBusy = false;
        }

        public ICommand ViewPoll
        {
            get { return new Command<int>(async (id) => await this.DoViewPollAsync(id)); }
        }

        private async Task DoViewPollAsync(int id)
        {
            this.IsBusy = true;
            this.IsEnabled = false;
            var identity = (IUserIdentity)Csla.ApplicationContext.User.Identity;
            var command = await this.pollSubmissionFactory.CreateAsync();
            command.PollID = id;
            command.UserID = identity.UserID.HasValue ? identity.UserID.Value : 0;
            command = await this.pollSubmissionFactory.ExecuteAsync(command);
            this.IsBusy = false;

            if (command.Submission != null)
            {
                var criteria = new ViewPollPageNavigationCriteria
                {
                    PollId = id
                };

                navigationService.ShowViewModel<ViewPollPageViewModel>(criteria);
            }
            else
            {
                var navigationCriteria = new PollResultsPageNavigationCriteria
                {
                    PollId = id
                };

                navigationService.ShowViewModel<PollResultsPageViewModel>(navigationCriteria);
            }
            this.IsEnabled = true;
        }
    }
}
