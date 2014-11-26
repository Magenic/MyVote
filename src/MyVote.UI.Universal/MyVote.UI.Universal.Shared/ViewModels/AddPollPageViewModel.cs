using Caliburn.Micro;
using Csla;
using MyVote.BusinessObjects.Contracts;
using MyVote.UI.Helpers;
using MyVote.UI.NavigationCriteria;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace MyVote.UI.ViewModels
{
    public sealed class AddPollPageViewModel : PageViewModelBase
    {
        private readonly IObjectFactory<IPoll> pollObjectFactory;
        private readonly IObjectFactory<IPollOption> pollOptionObjectFactory;
        private readonly IObjectFactory<ICategoryCollection> categoryObjectFactory;
        private readonly IMessageBox messageBox;

        public AddPollPageViewModel(
            INavigation navigation,
            IObjectFactory<IPoll> pollObjectFactory,
            IObjectFactory<IPollOption> pollOptionObjectFactory,
            IObjectFactory<ICategoryCollection> categoryObjectFactory,
            IMessageBox messageBox,
			PollImageViewModel pollImageViewModel)
            : base(navigation)
        {
            this.pollObjectFactory = pollObjectFactory;
            this.pollOptionObjectFactory = pollOptionObjectFactory;
            this.categoryObjectFactory = categoryObjectFactory;
            this.messageBox = messageBox;
			this.PollImageViewModel = pollImageViewModel;
            this.PollAnswers = new ObservableCollection<PollAnswerViewModel>();
        }

        public async Task Submit()
        {
			IsBusy = true;
            var hasError = false;
			try
			{
				if (this.PollImageViewModel.HasImage)
				{
					this.Poll.PollImageLink = await this.PollImageViewModel.UploadImage();
				}

				this.Poll = await this.Poll.SaveAsync() as IPoll;

			}
			catch (DataPortalException ex)
			{
				System.Diagnostics.Debug.WriteLine(ex);
				hasError = true;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex);
				hasError = true;
			}
			IsBusy = false;

            if (!hasError)
            {
                if (this.Poll != null && this.Poll.PollID != null)
                {
                    var criteria = new ViewPollPageNavigationCriteria() {PollId = this.Poll.PollID.Value};

					this.Navigation.NavigateToViewModel<ViewPollPageViewModel>(criteria);
                }
            }
            else
            {
#if WINDOWS_PHONE
				this.messageBox.Show("There was an error saving your poll. Please try again.", "Error");
#else
                await this.messageBox.ShowAsync("There was an error saving your poll. Please try again.", "Error");
#endif 
            }
        }

        public async Task AddImage()
        {
			await PollImageViewModel.AddImage();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "result")]
        protected override void OnInitialize()
        {
            // OnInitialize isn't async, so we have to go old school.
            var task = this.CreatePollAsync();

            var awaiter = task.GetAwaiter();
            awaiter.OnCompleted(() =>
            {
                this.SetupAnswers();
                this.Poll.PollMinAnswers = 1;
                this.Poll.PollMaxAnswers = 1;
                this.SpecifyBeginEndDates = false;

                if (task.Exception != null)
                {
#if WINDOWS_PHONE
					this.messageBox.Show("There was an error creating the poll.", "Error");
#else
                    this.messageBox.ShowAsync("There was an error creating the poll.", "Error");
#endif
                }
            });

            var categoryTask = this.LoadCategoriesAsync();

            var categoryAwaiter = categoryTask.GetAwaiter();
            categoryAwaiter.OnCompleted(() =>
                {
                    if (categoryTask.Exception != null)
                    {
#if WINDOWS_PHONE
						this.messageBox.Show("There was an error loading the categories.", "Error");
#else
                        this.messageBox.ShowAsync("There was an error loading the categories.", "Error");
#endif
                    }
                });
        }

        private void SetupAnswers()
        {
            for (int i = 0; i <= 5; i++)
                PollAnswers.Add(new PollAnswerViewModel(this.Poll, this.pollOptionObjectFactory, (short)i));
        }

        public async Task CreatePollAsync()
        {
            var identity = Csla.ApplicationContext.User.Identity as IUserIdentity;
            if (identity != null) this.Poll = await this.pollObjectFactory.CreateAsync(identity.UserID);
        }

        public async Task LoadCategoriesAsync()
        {
            this.Categories = await this.categoryObjectFactory.FetchAsync();
        }

        private void PollPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyOfPropertyChange(() => this.CanSave);
        }

        public ObservableCollection<PollAnswerViewModel> PollAnswers { get; private set; }

		public PollImageViewModel PollImageViewModel { get; private set; }

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
                NotifyOfPropertyChange(() => this.Poll);
            }
        }

        private ICategoryCollection categories;
        public ICategoryCollection Categories
        {
            get { return this.categories; }
            private set
            {
                this.categories = value;
                NotifyOfPropertyChange(() => this.Categories);
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
                NotifyOfPropertyChange(() => this.SpecifyBeginEndDates);

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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi")]
        public bool HasMultiAnswer
        {
            get { return this.hasMultiAnswer; }
            set
            {
                this.hasMultiAnswer = value;
                NotifyOfPropertyChange(() => this.HasMultiAnswer);
            }
        }

        public bool CanSave
        {
            get { return Poll != null && Poll.IsSavable; }
        }
    }
}
