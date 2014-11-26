using System;
using System.Linq;
using System.Windows.Input;

using Cirrious.MvvmCross.ViewModels;

using Csla;
using MyVote.BusinessObjects.Contracts;
using MyVote.UI.Helpers;
using MyVote.UI.NavigationCriteria;
using System.Threading.Tasks;
#if !__MOBILE__
using Windows.ApplicationModel.DataTransfer;
#endif
using System.Collections.ObjectModel;
using MyVote.BusinessObjects;
using Xamarin;

namespace MyVote.UI.ViewModels
{
    public sealed class PollResultsPageViewModel : PageViewModelBase
    {
        private readonly IObjectFactory<IPollResults> objectFactory;
        private readonly IObjectFactory<IPoll> pollFactory;
        private readonly IObjectFactory<IPollComment> pollCommentFactory;
        private readonly IMessageBox messageBox;

#if NETFX_CORE
		private readonly IShareManager shareManager;
		private readonly ISecondaryPinner secondaryPinner;
#endif // NETFX_CORE

        public PollResultsPageViewModel(
            IObjectFactory<IPollResults> objectFactory,
            IObjectFactory<IPoll> pollFactory,
            IObjectFactory<IPollComment> pollCommentFactory,
            IMessageBox messageBox
#if NETFX_CORE
			, IShareManager shareManager,
			ISecondaryPinner secondaryPinner
#endif // NETFX_CORE
)
        {
            this.objectFactory = objectFactory;
            this.pollFactory = pollFactory;
            this.pollCommentFactory = pollCommentFactory;
            this.messageBox = messageBox;

            this.PollComments = new ObservableCollection<PollCommentViewModel>();
			this.PollDataResults = new ObservableCollection<PollDataResultViewModel>();

#if NETFX_CORE
			this.shareManager = shareManager;
			this.secondaryPinner = secondaryPinner;
#endif // NETFX_CORE
        }

        public async Task LoadPollAsync()
        {
            this.IsBusy = true;

            var hasError = false;
            try
            {
                var identity = Csla.ApplicationContext.User.Identity as IUserIdentity;

                this.PollResults = await this.objectFactory.FetchAsync(new PollResultsCriteria(identity.UserID, this.NavigationCriteria.PollId));

				if (this.PollResults != null)
				{
					if (this.PollResults.PollComments != null)
					{
						foreach (var comment in this.PollResults.PollComments.Comments)
						{
							this.PollComments.Add(new PollCommentViewModel(null, comment, this.SubmitChildComment, false));
						}
					}

					var totalResponses = this.TotalResponses;
					foreach (var pollDataResult in this.PollResults.PollDataResults.Results)
					{
						this.PollDataResults.Add(new PollDataResultViewModel(pollDataResult, totalResponses));
					}
				}
            }
            catch (DataPortalException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                hasError = true;
                Insights.Report(ex);
            }
            this.IsBusy = false;

            if (hasError)
            {
#if WINDOWS_PHONE
				this.messageBox.Show("There was an error loading the poll results. Please try again.", "Error");
#else
                await this.messageBox.ShowAsync("There was an error loading the poll results. Please try again.", "Error");
#endif // WINDOWS_PHONE
            }
        }

#if NETFX_CORE
		public void ShareRequested(DataPackage dataPackage)
		{
			var sharedPoll = new MyVote.UI.Models.SharedPoll
			{
				PollId = this.PollResults.PollID,
				Question = this.PollResults.PollDataResults.Question
			};
			foreach (var response in this.PollResults.PollDataResults.Results)
			{
				sharedPoll.Options.Add(response.OptionText);
			}

			SharedPollPackageBuilder.Build(sharedPoll, dataPackage);
		}

		public async Task PinPoll(Windows.UI.Xaml.FrameworkElement sender)
		{
			this.IsPollPinned = await this.secondaryPinner.PinPoll(sender, this.PollResults.PollID, this.PollResults.PollDataResults.Question);
		}

		public async Task UnpinPoll(Windows.UI.Xaml.FrameworkElement sender)
		{
			this.IsPollPinned = !await this.secondaryPinner.UnpinPoll(sender, this.PollResults.PollID);
		}
#endif // NETFX_CORE

        public ICommand DeletePoll
        {
            get
            {
                return new MvxCommand(async () => await DeletePollHandler());
            }
        }

        private async Task DeletePollHandler()
        {
#if	WINDOWS_PHONE
			var result = this.messageBox.Show("Are you sure you want to delete this poll?", "Delete Poll?", MessageBoxButtons.OkCancel);
#elif __MOBILE__
            var result = await this.messageBox.ShowAsync("Are you sure you want to delete this poll?", "Delete Poll?", MessageBoxButtons.OkCancel);
#else
            var result = await this.messageBox.ShowAsync("Are you sure you want to delete this poll?", "Delete Poll?", MessageBoxButtons.YesNo);
#endif // WINDOWS_PHONE

            if (result != null && result.Value)
            {
                this.IsBusy = true;

                var poll = await this.pollFactory.FetchAsync(this.PollResults.PollID);
                poll.Delete();
                await poll.SaveAsync();

                this.IsBusy = false;
                this.GoBack();
            }            
        }

        public async Task SubmitComment()
        {
            var identity = Csla.ApplicationContext.User.Identity as IUserIdentity;
            var comment = this.pollCommentFactory.CreateChild(identity.UserID, identity.UserName);
            comment.CommentText = this.RootComment;

            this.PollResults.PollComments.Comments.Add(comment);
            this.PollComments.Add(new PollCommentViewModel(null, comment, this.SubmitChildComment, false));

            this.IsBusy = true;

            await this.PollResults.SaveAsync();

            this.RootComment = string.Empty;

            this.IsBusy = false;
        }

        public async Task SubmitChildComment(int pollCommentId, string commentText)
        {
            var identity = Csla.ApplicationContext.User.Identity as IUserIdentity;

            var parentComment = this.PollResults.PollComments.Comments.Single(_ => _.PollCommentID == pollCommentId);
            var comment = this.pollCommentFactory.CreateChild(identity.UserID, identity.UserName);
            comment.CommentText = commentText;

            parentComment.Comments.Add(comment);

            var parentCommentViewModel = this.PollComments.Single(_ => _.PollComment.PollCommentID == pollCommentId);
            parentCommentViewModel.ChildComments.Add(new PollCommentViewModel(parentComment.PollCommentID, comment, this.SubmitChildComment, true));

            this.IsBusy = true;

            await this.PollResults.SaveAsync();

            this.IsBusy = false;
        }

        public void Init(PollResultsPageNavigationCriteria criteria)
        {
            this.NavigationCriteria = criteria;
        }

        public async override void Start()
        {
            try
            {
                await this.LoadPollAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                Insights.Report(ex);
            }

#if NETFX_CORE
			this.shareManager.Initialize();
			this.shareManager.OnShareRequested = ShareRequested;
#endif // NETFX_CORE
        }

#if NETFX_CORE
		protected override void OnDeactivate(bool close)
		{
			this.shareManager.Cleanup();
		}
#endif // NETFX_CORE

		public ObservableCollection<PollDataResultViewModel> PollDataResults { get; private set; }
        public ObservableCollection<PollCommentViewModel> PollComments { get; private set; }

        private IPollResults pollResults;
        public IPollResults PollResults
        {
            get { return this.pollResults; }
            set
            {
                this.pollResults = value;
                this.RaisePropertyChanged(() => this.PollResults);
                this.RaisePropertyChanged(() => this.TotalResponses);

#if NETFX_CORE
				if (value != null)
				{
					this.IsPollPinned = this.secondaryPinner.IsPollPinned(value.PollID);
				}
#endif // NETFX_CORE
            }
        }

        private bool isPollPinned;
        public bool IsPollPinned
        {
            get { return this.isPollPinned; }
            set
            {
                this.isPollPinned = value;
                this.RaisePropertyChanged(() => this.IsPollPinned);
            }
        }

        private string rootComment = string.Empty;
        public string RootComment
        {
            get { return this.rootComment; }
            set
            {
                this.rootComment = value;
                this.RaisePropertyChanged(() => this.RootComment);
                this.RaisePropertyChanged(() => this.CanSubmitComment);
            }
        }

        public bool CanSubmitComment
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.RootComment);
            }
        }

        public int TotalResponses
        {
            get
            {
                if (this.PollResults != null)
                {
                    return this.PollResults.PollDataResults.Results.Sum(r => r.ResponseCount);
                }
                else
                {
                    return 0;
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private PollResultsPageNavigationCriteria NavigationCriteria { get; set; }
    }
}
