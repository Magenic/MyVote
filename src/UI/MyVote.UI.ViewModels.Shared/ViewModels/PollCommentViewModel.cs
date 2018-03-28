using MyVote.BusinessObjects.Contracts;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using MyVote.UI.Helpers;

namespace MyVote.UI.ViewModels
{
    public sealed class PollCommentViewModel : ViewModelBase
    {
		public PollCommentViewModel(int? parentCommentId, 
                                    IPollComment pollComment, 
                                    Func<int, string, Task> submitCommentCallback, 
                                    bool isNested)
		{
            this.ParentCommentId = parentCommentId;
			this.PollComment = pollComment ?? throw new ArgumentNullException(nameof(pollComment));
			this.SubmitCommentCallback = submitCommentCallback ?? throw new ArgumentNullException(nameof(submitCommentCallback));

			this.IsNested = isNested;

			this.ChildComments = new ObservableCollection<PollCommentViewModel>();
			if (pollComment.Comments != null)
			{
				foreach (var comment in pollComment.Comments)
				{
					this.ChildComments.Add(new PollCommentViewModel(comment.PollCommentID, comment, submitCommentCallback, true));
				}
			}
		}

		public ICommand ShowReply
		{
			get
			{
				return new Command(() => ShowReplyHandler());
			}
		}

		private void ShowReplyHandler()
		{
			this.ShouldShowReply = true;
		}

		public ICommand SubmitComment
		{
			get
			{
				return new Command(async() => await SubmitCommentHandler());
			}
		}

		public async Task SubmitCommentHandler()
		{
			await this.SubmitCommentCallback(this.PollComment.PollCommentID.Value, this.ReplyText);
			this.ReplyText = string.Empty;
			this.ShouldShowReply = false;
		}

		private int? ParentCommentId { get; set; }
		private bool IsNested { get; set; }
		private Func<int, string, Task> SubmitCommentCallback { get; set; }
		public IPollComment PollComment { get; private set; }
		public ObservableCollection<PollCommentViewModel> ChildComments { get; private set; }

		private string replyText = string.Empty;
		public string ReplyText
		{
			get { return this.replyText; }
			set
			{
				this.replyText = value;
                this.RaisePropertyChanged(nameof(ReplyText));
                this.RaisePropertyChanged(nameof(CanSubmit));
			}
		}

		public bool CanSubmit
		{
			get
			{
				return !string.IsNullOrWhiteSpace(this.ReplyText);
			}
		}

		public bool ShouldShowReplyOption
		{
			get
			{
				return !this.IsNested && !this.ShouldShowReply;
			}
		}

		private bool shouldShowReply;
		public bool ShouldShowReply
		{
			get { return this.shouldShowReply; }
			private set
			{
				this.shouldShowReply = value;
                this.RaisePropertyChanged(nameof(ShouldShowReply));
                this.RaisePropertyChanged(nameof(ShouldShowReplyOption));
			}
		}
    }
}
