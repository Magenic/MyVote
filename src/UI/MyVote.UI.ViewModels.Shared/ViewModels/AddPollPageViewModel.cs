using Csla;
using MyVote.BusinessObjects.Contracts;
using MyVote.UI.Helpers;
using MyVote.UI.NavigationCriteria;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows.Input;
using MyVote.UI.Contracts;

namespace MyVote.UI.ViewModels
{
    public delegate void PollAddedEventHandler(object sender, AddPollEventArgs e);

    public sealed class AddPollEventArgs : EventArgs
    {
        public IPoll Poll { get; set; }
    }

    public sealed class AddPollPageViewModel : NavigatingViewModelBase
    {
        private readonly IObjectFactory<IPoll> pollObjectFactory;
        private readonly IObjectFactory<IPollOption> pollOptionObjectFactory;
        private readonly IObjectFactory<ICategoryCollection> categoryObjectFactory;
        private readonly IMessageBox messageBox;
		private readonly ILogger logger;

        public event PollAddedEventHandler PollAdded;

        public AddPollPageViewModel(
            IObjectFactory<IPoll> pollObjectFactory,
            IObjectFactory<IPollOption> pollOptionObjectFactory,
            IObjectFactory<ICategoryCollection> categoryObjectFactory,
            IMessageBox messageBox,
			ILogger logger,
            PollImageViewModel pollImageViewModel, 
            INavigationService navigationService) :  base(navigationService)
        {
            this.pollObjectFactory = pollObjectFactory;
            this.pollOptionObjectFactory = pollOptionObjectFactory;
            this.categoryObjectFactory = categoryObjectFactory;
            this.messageBox = messageBox;
			this.logger = logger;
            this.PollImageViewModel = pollImageViewModel;
        }

        public ICommand Submit
        {
            get
            {
                return new Command(async () => await SubmitHandlerAsync());
            }
        }

        public async Task SubmitHandlerAsync()
        {
            this.IsBusy = true;
            var hasError = false;
            try
            {
                if (this.PollImageViewModel.HasImage)
                {
					this.Poll.PollImageLink = await this.PollImageViewModel.UploadImage().ConfigureAwait(true);
                }

                this.Poll = ((IPoll)await this.Poll.SaveAsync());

            }
            catch (DataPortalException ex)
            {
				this.logger.Log(ex);
                hasError = true;
            }
            catch (Exception ex)
            {
				this.logger.Log(ex);
                hasError = true;
            }
            this.IsBusy = false;

            if (!hasError)
            {
                if (this.Poll != null && this.Poll.PollID != null)
                {
                    if (PollAdded != null)
                    {
                        PollAdded(this, new AddPollEventArgs { Poll = this.Poll });                        
                    }
#if ANDROID
                    this.navigationService.Close();
#endif
                    var criteria = new ViewPollPageNavigationCriteria
                                       {
                                           PollId = this.Poll.PollID.Value
                                       };

                    navigationService.ShowViewModel<ViewPollPageViewModel>(criteria);
                    var viewModelLoader = Ioc.Resolve<IViewModelLoader>();
                    this.PollImageViewModel = viewModelLoader.LoadViewModel<PollImageViewModel>();
                    Start();
                }
            }
            else
            {
                await this.messageBox.ShowAsync("There was an error saving your poll. Please try again.", "Error");
            }
        }

        public ICommand AddImage
        {
            get
            {
                return new Command(async () => await AddImageHandler());
            }
        }

        public async Task AddImageHandler()
        {
            await PollImageViewModel.AddImage();
        }

        public async override void Start()
        {
            try
            {
                await this.CreatePollAsync();
                this.SetupOptions();
                this.Poll.PollMinAnswers = 1;
                this.Poll.PollMaxAnswers = 1;
                this.SpecifyBeginEndDates = false;
            }
            catch (Exception ex)
            {
                await this.messageBox.ShowAsync("There was an error creating the poll.", "Error");
				this.logger.Log(ex);
            }

            try
            {
                await this.LoadCategoriesAsync();
            }
            catch (Exception ex)
            {
                await this.messageBox.ShowAsync("There was an error loading the categories.", "Error");
				this.logger.Log(ex);
            }
        }

        private void SetupOptions()
        {
            var newOptions = new ObservableCollection<PollOptionViewModel>();
			for (int i = 0; i < 5; i++)
			{
				newOptions.Add(new PollOptionViewModel(this.Poll, this.pollOptionObjectFactory, (short)i));
			}

            this.PollOptions = newOptions;
        }

        public async Task CreatePollAsync()
        {
            var identity = ApplicationContext.User.Identity as IUserIdentity;
            if (identity != null) this.Poll = await this.pollObjectFactory.CreateAsync(identity.UserID);
        }

        public async Task LoadCategoriesAsync()
        {
            var newCategoryList = new ObservableCollection<SelectOptionViewModel<int>>();
            var cats = await this.categoryObjectFactory.FetchAsync();
            foreach (var category in cats)
            {
                var bindableCategory = new SelectOptionViewModel<int> {Display = category.Name, Value = category.ID};
                newCategoryList.Add(bindableCategory);
            }
            this.Categories = newCategoryList;
            this.RaisePropertyChanged(nameof(Categories));
        }

        private void PollPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.RaisePropertyChanged(nameof(CanSave));
        }

		private ObservableCollection<PollOptionViewModel> pollOptions;
        public ObservableCollection<PollOptionViewModel> PollOptions
		{
			get { return this.pollOptions; }
			private set
			{
				this.pollOptions = value;
                RaisePropertyChanged(nameof(PollOptions));
			}
		}

        private PollImageViewModel pollImageViewModel;
        public PollImageViewModel PollImageViewModel 
        { 
            get { return pollImageViewModel; }
            private set
            {
                pollImageViewModel = value;
                RaisePropertyChanged(nameof(PollImageViewModel));
            }
        }

        private IPoll poll;
        public IPoll Poll
        {
            get { return this.poll; }
            private set
            {
                if (this.poll != null)
                {
                    this.poll.PropertyChanged -= PollPropertyChanged;
                }

                this.poll = value;

                if (value != null)
                {
                    value.PropertyChanged += PollPropertyChanged;
                }
                this.RaisePropertyChanged(nameof(Poll));
            }
        }

        private ObservableCollection<SelectOptionViewModel<int>> categories;
        public ObservableCollection<SelectOptionViewModel<int>> Categories
        {
            get { return this.categories; }
            private set
            {
                this.categories = value;
                this.RaisePropertyChanged(nameof(Categories));
            }
        }

        private bool specifyBeginEndDates;
        public bool SpecifyBeginEndDates
        {
            get
            {
                return specifyBeginEndDates;
            }
            set
            {
                specifyBeginEndDates = value;
                this.RaisePropertyChanged(nameof(SpecifyBeginEndDates));

                if (value)
                {
                    Poll.PollStartDate = DateTime.Today;
                    Poll.PollEndDate = DateTime.Today.AddDays(7);
                }
                else
                {
                    Poll.PollStartDate = DateTime.Today;
                    Poll.PollEndDate = DateTime.MaxValue;
                }
            }
        }

        private bool hasMultiAnswer;
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi")]
        public bool HasMultiAnswer
        {
            get { return this.hasMultiAnswer; }
            set
            {
                this.hasMultiAnswer = value;
                this.RaisePropertyChanged(nameof(HasMultiAnswer));
            }
        }

        public bool CanSave
        {
            get { return Poll != null && Poll.IsSavable; }
        }
    }
}
