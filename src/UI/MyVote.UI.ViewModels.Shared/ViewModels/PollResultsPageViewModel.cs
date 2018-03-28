using Csla;
using MyVote.BusinessObjects;
using MyVote.BusinessObjects.Contracts;
using MyVote.UI.Helpers;
using MyVote.UI.NavigationCriteria;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MyVote.UI.Contracts;

namespace MyVote.UI.ViewModels
{
	public sealed class PollResultsPageViewModel : NavigatingViewModelBase
	{
		private readonly IObjectFactory<IPollResults> objectFactory;
		private readonly IObjectFactory<IPoll> pollFactory;
		private readonly IObjectFactory<IPollComment> pollCommentFactory;
		private readonly IMessageBox messageBox;
		private readonly ILogger logger;

#if NETFX_CORE
		private readonly IShareManager shareManager;
		private readonly ISecondaryPinner secondaryPinner;
#endif // NETFX_CORE

		public PollResultsPageViewModel(
			 IObjectFactory<IPollResults> objectFactory,
			 IObjectFactory<IPoll> pollFactory,
			 IObjectFactory<IPollComment> pollCommentFactory,
			 IMessageBox messageBox,
		     ILogger logger,
             INavigationService navigationService
#if NETFX_CORE
			, IShareManager shareManager,
		 ISecondaryPinner secondaryPinner
#endif // NETFX_CORE
) : base(navigationService)
		{
			this.objectFactory = objectFactory;
			this.pollFactory = pollFactory;
			this.pollCommentFactory = pollCommentFactory;
			this.messageBox = messageBox;
			this.logger = logger;

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
							this.PollComments.Add(new PollCommentViewModel(null, comment, this.SubmitChildCommentAsync, false));
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
				this.logger.Log(ex);
				hasError = true;
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
		public void ShareRequested(Windows.ApplicationModel.DataTransfer.DataPackage dataPackage)
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

		public ICommand PinPoll
		{
			get
			{
				return new Command<Windows.UI.Xaml.FrameworkElement>(async (param) => await PinPollHandler(param));
			}
		}

		private async Task PinPollHandler(Windows.UI.Xaml.FrameworkElement sender)
		{
			this.IsPollPinned = await this.secondaryPinner.PinPoll(sender, this.PollResults.PollID, this.PollResults.PollDataResults.Question);
		}

		public ICommand UnpinPoll
		{
			get
			{
				return new Command<Windows.UI.Xaml.FrameworkElement>(async (param) => await UnpinPollHandler(param));
			}
		}

		public async Task UnpinPollHandler(Windows.UI.Xaml.FrameworkElement sender)
		{
			this.IsPollPinned = !await this.secondaryPinner.UnpinPoll(sender, this.PollResults.PollID);
		}
#endif // NETFX_CORE

		public ICommand DeletePoll
		{
			get
			{
				return new Command(async () => await DeletePollHandlerAsync());
			}
		}

		private async Task DeletePollHandlerAsync()
		{
#if __MOBILE__
				var result = await this.messageBox.ShowAsync("Are you sure you want to delete this poll?", "Delete Poll?", MessageBoxButtons.OkCancel);
#else
			var result = await this.messageBox.ShowAsync("Are you sure you want to delete this poll?", "Delete Poll?", MessageBoxButtons.YesNo);
#endif // __MOBILE__

			if (result != null && result.Value)
			{
				this.IsBusy = true;

				var poll = await this.pollFactory.FetchAsync(this.PollResults.PollID);
				poll.Delete();
				await poll.SaveAsync();

				this.IsBusy = false;
				this.GoBack.Execute(null);
			}
		}

		public ICommand SubmitComment
		{
			get
			{
				return new Command(async () => await SubmitCommentHandlerAsync());
			}
		}

		private async Task SubmitCommentHandlerAsync()
		{
			var identity = ApplicationContext.User.Identity as IUserIdentity;
			var comment = this.pollCommentFactory.CreateChild(identity.UserID, identity.UserName);
			comment.CommentText = this.RootComment;

			this.PollResults.PollComments.Comments.Add(comment);
			this.PollComments.Add(new PollCommentViewModel(null, comment, this.SubmitChildCommentAsync, false));

			this.IsBusy = true;

			await this.PollResults.SaveAsync();

			this.RootComment = string.Empty;

			this.IsBusy = false;
		}

		public async Task SubmitChildCommentAsync(int pollCommentId, string commentText)
		{
			var identity = Csla.ApplicationContext.User.Identity as IUserIdentity;

			var parentComment = this.PollResults.PollComments.Comments.Single(_ => _.PollCommentID == pollCommentId);
			var comment = this.pollCommentFactory.CreateChild(identity.UserID, identity.UserName);
			comment.CommentText = commentText;

			parentComment.Comments.Add(comment);

			var parentCommentViewModel = this.PollComments.Single(_ => _.PollComment.PollCommentID == pollCommentId);
			parentCommentViewModel.ChildComments.Add(new PollCommentViewModel(parentComment.PollCommentID, comment, this.SubmitChildCommentAsync, true));

			this.IsBusy = true;

			await this.PollResults.SaveAsync();

			this.IsBusy = false;
		}

		public override void Init(object parameters)
		{
			this.NavigationCriteria = (PollResultsPageNavigationCriteria)parameters;
		}

		public async override void Start()
		{
			try
			{
				await this.LoadPollAsync();
			}
			catch (Exception ex)
			{
				this.logger.Log(ex);
			}

#if NETFX_CORE
			this.shareManager.Initialize();
			this.shareManager.OnShareRequested = ShareRequested;
#endif // NETFX_CORE
		}

        //todo: figure out what to do for UWP
//		protected override void SaveStateToBundle(IMvxBundle bundle)
//		{
//			base.SaveStateToBundle(bundle);

//#if NETFX_CORE
//			this.shareManager.Cleanup();
//#endif // NETFX_CORE
		//}

		public ObservableCollection<PollDataResultViewModel> PollDataResults { get; private set; }
		public ObservableCollection<PollCommentViewModel> PollComments { get; private set; }

		private IPollResults pollResults;
		public IPollResults PollResults
		{
			get { return this.pollResults; }
			set
			{
				this.pollResults = value;
                this.RaisePropertyChanged(nameof(PollResults));
                this.RaisePropertyChanged(nameof(TotalResponses));

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
                this.RaisePropertyChanged(nameof(IsPollPinned));
			}
		}

		private string rootComment = string.Empty;
		public string RootComment
		{
			get { return this.rootComment; }
			set
			{
				this.rootComment = value;
                this.RaisePropertyChanged(nameof(RootComment));
                this.RaisePropertyChanged(nameof(CanSubmitComment));
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

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		private PollResultsPageNavigationCriteria NavigationCriteria { get; set; }
	}
}
