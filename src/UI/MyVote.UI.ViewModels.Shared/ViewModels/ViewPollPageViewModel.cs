﻿
using Csla;
using MyVote.BusinessObjects.Contracts;
using MyVote.UI.Helpers;
using MyVote.UI.NavigationCriteria;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;

namespace MyVote.UI.ViewModels
{
	public sealed class ViewPollPageViewModel : ViewModelBase<ViewPollPageNavigationCriteria>
    {
		private readonly IObjectFactory<IPollSubmissionCommand> objectFactory;
        private readonly IObjectFactory<IPoll> pollFactory;
        private readonly IMessageBox messageBox;
		private readonly ILogger logger;

#if NETFX_CORE
		private readonly IShareManager shareManager;
		private readonly ISecondaryPinner secondaryPinner;
#endif // NETFX_CORE

        public ViewPollPageViewModel(
            IObjectFactory<IPollSubmissionCommand> objectFactory,
            IObjectFactory<IPoll> pollFactory,
            IMessageBox messageBox,
			ILogger logger
#if NETFX_CORE
			, IShareManager shareManager,
			ISecondaryPinner secondaryPinner
#endif // NETFX_CORE
)
        {
            this.objectFactory = objectFactory;
            this.pollFactory = pollFactory;
            this.messageBox = messageBox;
			this.logger = logger;

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
                var command = await this.objectFactory.CreateAsync();
                command.PollID = this.NavigationCriteria.PollId;
                command.UserID = identity.UserID.Value;
                command = await this.objectFactory.ExecuteAsync(command);

                this.PollSubmission = command.Submission;
            }
            catch (DataPortalException ex)
            {
				this.logger.Log(ex);
                hasError = true;
            }
            this.IsBusy = false;

            if (hasError)
            {
                await this.messageBox.ShowAsync("There was an error loading the poll. Please try again.", "Error");
            }
        }

        public async Task Submit()
        {
            this.IsBusy = true;

            var hasError = false;
            try
            {
                this.PollSubmission = await this.PollSubmission.SaveAsync() as IPollSubmission;
            }
            catch (DataPortalException ex)
            {
				this.logger.Log(ex);
                hasError = true;
            }
            this.IsBusy = false;

            if (!hasError)
            {
                this.NavigateToPollResults(this.PollSubmission.PollID);
            }
            else
            {
                await this.messageBox.ShowAsync("There was an error submitting your poll. Please try again.", "Error");
            }
        }

		public ICommand DeletePoll
		{
			get
			{
				return new MvxCommand(async () => await DeletePollHandler());
			}
		}

        private async Task DeletePollHandler()
        {
#if __MOBILE__
            var result = await this.messageBox.ShowAsync("Are you sure you want to delete this poll?", "Delete Poll?", MessageBoxButtons.OkCancel);
#else
            var result = await this.messageBox.ShowAsync("Are you sure you want to delete this poll?", "Delete Poll?", MessageBoxButtons.YesNo);
#endif // __MOBILE__

			if (result != null && result.Value)
            {
                var poll = await this.pollFactory.FetchAsync(this.PollSubmission.PollID);
                poll.Delete();
                await poll.SaveAsync();

                this.GoBack.Execute(null);
            }
        }

#if NETFX_CORE
		public void ShareRequested(Windows.ApplicationModel.DataTransfer.DataPackage dataPackage)
		{
			var sharedPoll = new MyVote.UI.Models.SharedPoll
			{
				PollId = this.PollSubmission.PollID,
				Question = this.PollSubmission.PollQuestion,
				Description = this.PollSubmission.PollDescription
			};
			foreach (var response in this.PollSubmission.Responses)
			{
				sharedPoll.Options.Add(response.OptionText);
			}

			SharedPollPackageBuilder.Build(sharedPoll, dataPackage);
		}

		public ICommand PinPoll
		{
			get
			{
				return new MvxCommand<Windows.UI.Xaml.FrameworkElement>(async (param) => await PinPollHandler(param));
			}
		}

		private async Task PinPollHandler(Windows.UI.Xaml.FrameworkElement sender)
		{
			this.IsPollPinned = await this.secondaryPinner.PinPoll(sender, this.PollSubmission.PollID, this.PollSubmission.PollQuestion);
		}

		public ICommand UnpinPoll
		{
			get
			{
				return new MvxCommand<Windows.UI.Xaml.FrameworkElement>(async (param) => await UnpinPollHandler(param));
			}
		}

		public async Task UnpinPollHandler(Windows.UI.Xaml.FrameworkElement sender)
		{
			this.IsPollPinned = !await this.secondaryPinner.UnpinPoll(sender, this.PollSubmission.PollID);
		}
#endif // NETFX_CORE

        public override void Start()
        {
            var task = this.LoadPollAsync();
            var awaiter = task.GetAwaiter();
            awaiter.OnCompleted(() =>
            {
                if (task.Exception != null)
                {
					this.logger.Log(task.Exception);
                }
            });

#if NETFX_CORE
			this.shareManager.Initialize();
			this.shareManager.OnShareRequested = ShareRequested;
#endif // NETFX_CORE
        }

		protected override void SaveStateToBundle(IMvxBundle bundle)
		{
			base.SaveStateToBundle(bundle);

#if NETFX_CORE
			this.shareManager.Cleanup();
#endif // NETFX_CORE
		}

        public override void RealInit(ViewPollPageNavigationCriteria criteria)
        {
            this.NavigationCriteria = criteria;
        }

        private void NavigateToPollResults(int pollId)
        {
            var navigationCriteria = new PollResultsPageNavigationCriteria
            {
                PollId = pollId
            };

			this.GoBack.Execute(null);
            this.ShowViewModel<PollResultsPageViewModel>(navigationCriteria);
        }

        private void PollSubmission_ChildChanged(object sender, Csla.Core.ChildChangedEventArgs e)
        {
            this.RaisePropertyChanged(() => this.CanSubmit);
        }

        private void PollSubmission_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.RaisePropertyChanged(() => this.CanSubmit);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private ViewPollPageNavigationCriteria NavigationCriteria { get; set; }

        private IPollSubmission pollSubmission;
        public IPollSubmission PollSubmission
        {
            get { return this.pollSubmission; }
            set
            {
                if (this.PollSubmission != null)
                {
                    this.PollSubmission.PropertyChanged -= PollSubmission_PropertyChanged;
                    this.PollSubmission.ChildChanged -= PollSubmission_ChildChanged;
                }

                this.pollSubmission = value;

                if (value != null)
                {
                    value.PropertyChanged += PollSubmission_PropertyChanged;
                    value.ChildChanged += PollSubmission_ChildChanged;

#if NETFX_CORE
					this.IsPollPinned = this.secondaryPinner.IsPollPinned(value.PollID);
#endif // NETFX_CORE
                }

                this.RaisePropertyChanged(() => this.PollSubmission);
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

        public bool CanSubmit
        {
            get
            {
                if (this.PollSubmission != null)
                {
                    return this.PollSubmission.IsActive && this.PollSubmission.IsSavable;
                }
                else
                {
                    return false;
                }
            }
        }

		public ICommand SubmitPoll
		{
			get
			{
				return new MvxCommand(async () => await this.Submit());
			}
		}
    }
}
