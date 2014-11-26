using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using Csla;
using MyVote.BusinessObjects.Contracts;
using MyVote.UI.Contracts;
using MyVote.UI.Helpers;
using MyVote.UI.NavigationCriteria;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin;

namespace MyVote.UI.ViewModels
{
    public sealed class AddPollPageViewModel : PageViewModelBase
    {
        private readonly IObjectFactory<IPoll> pollObjectFactory;
        private readonly IObjectFactory<IPollOption> pollOptionObjectFactory;
        private readonly IObjectFactory<ICategoryCollection> categoryObjectFactory;
        private readonly IMessageBox messageBox;

        public AddPollPageViewModel(
            IObjectFactory<IPoll> pollObjectFactory,
            IObjectFactory<IPollOption> pollOptionObjectFactory,
            IObjectFactory<ICategoryCollection> categoryObjectFactory,
            IMessageBox messageBox,
            IPollImageViewModel pollImageViewModel)
        {
            this.pollObjectFactory = pollObjectFactory;
            this.pollOptionObjectFactory = pollOptionObjectFactory;
            this.categoryObjectFactory = categoryObjectFactory;
            this.messageBox = messageBox;
            this.PollImageViewModel = pollImageViewModel;
        }

        public ICommand Submit
        {
            get
            {
                return new MvxCommand(async () => await SubmitHandlerAsync());
            }
        }

        public async Task SubmitHandlerAsync()
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
                Insights.Report(ex);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                hasError = true;
                Insights.Report(ex);
            }
            IsBusy = false;

            if (!hasError)
            {
                if (this.Poll != null && this.Poll.PollID != null)
                {
                    this.Close(this);
                    var criteria = new ViewPollPageNavigationCriteria
                                       {
                                           PollId = this.Poll.PollID.Value
                                       };

                    this.ShowViewModel<ViewPollPageViewModel>(criteria);
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

        public ICommand AddImage
        {
            get
            {
                return new MvxCommand(async () => await AddImageHandler());
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
                this.SetupAnswers();
                this.Poll.PollMinAnswers = 1;
                this.Poll.PollMaxAnswers = 1;
                this.SpecifyBeginEndDates = false;

            }
            catch (Exception ex)
            {
#if WINDOWS_PHONE
				this.messageBox.Show("There was an error creating the poll.", "Error");
#else
                this.messageBox.ShowAsync("There was an error creating the poll.", "Error");
#endif
                Insights.Report(ex);
            }

            try
            {
                await this.LoadCategoriesAsync();
            }
            catch (Exception ex)
            {
#if WINDOWS_PHONE
				this.messageBox.Show("There was an error loading the categories.", "Error");
#else
                this.messageBox.ShowAsync("There was an error loading the categories.", "Error");
#endif
                Insights.Report(ex);
            }
        }

        private void SetupAnswers()
        {
            var newAnswers = new ObservableCollection<PollAnswerViewModel>();
            for (int i = 0; i <= 5; i++)
                newAnswers.Add(new PollAnswerViewModel(this.Poll, this.pollOptionObjectFactory, (short)i));
            this.PollAnswers = newAnswers;
        }

        public async Task CreatePollAsync()
        {
            var identity = ApplicationContext.User.Identity as IUserIdentity;
            if (identity != null) this.Poll = await this.pollObjectFactory.CreateAsync(identity.UserID);
        }

        public async Task LoadCategoriesAsync()
        {
            var newCategoryList = new ObservableCollection<SelectOptionViewModel<int>>();
            var categories = await this.categoryObjectFactory.FetchAsync();
            foreach (var category in categories)
            {
                var bindableCategory = new SelectOptionViewModel<int> {Display = category.Name, Value = category.ID};
                newCategoryList.Add(bindableCategory);
            }
            this.Categories = newCategoryList;
            this.RaisePropertyChanged(() => this.Categories);
        }

        private void PollPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.RaisePropertyChanged(() => this.CanSave);
        }

        public ObservableCollection<PollAnswerViewModel> PollAnswers { get; private set; }

        public IPollImageViewModel PollImageViewModel { get; private set; }

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
                this.RaisePropertyChanged(() => this.Poll);
            }
        }

        private ObservableCollection<SelectOptionViewModel<int>> categories;
        public ObservableCollection<SelectOptionViewModel<int>> Categories
        {
            get { return this.categories; }
            private set
            {
                this.categories = value;
                this.RaisePropertyChanged(() => this.Categories);
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
                this.RaisePropertyChanged(() => this.SpecifyBeginEndDates);

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
                this.RaisePropertyChanged(() => this.HasMultiAnswer);
            }
        }

        public bool CanSave
        {
            get { return Poll != null && Poll.IsSavable; }
        }
    }
}