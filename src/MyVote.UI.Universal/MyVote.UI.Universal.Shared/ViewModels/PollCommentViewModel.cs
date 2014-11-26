using Caliburn.Micro;
using MyVote.BusinessObjects.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVote.UI.ViewModels
{
	public sealed class PollCommentViewModel : PropertyChangedBase
	{
		public PollCommentViewModel(int? parentCommentId, IPollComment pollComment, Func<int, string, Task> submitCommentCallback, bool isNested)
		{
			if (pollComment == null)
			{
				throw new ArgumentNullException("pollComment");
			}
			if (submitCommentCallback == null)
			{
				throw new ArgumentNullException("submitCommentCallback");
			}

			this.ParentCommentId = parentCommentId;
			this.PollComment = pollComment;
			this.SubmitCommentCallback = submitCommentCallback;

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

		public void ShowReply()
		{
			this.ShouldShowReply = true;
		}

		public async Task SubmitComment()
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
				this.NotifyOfPropertyChange(() => this.ReplyText);
				this.NotifyOfPropertyChange(() => this.CanSubmit);
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
				this.NotifyOfPropertyChange(() => this.ShouldShowReply);
				this.NotifyOfPropertyChange(() => this.ShouldShowReplyOption);
			}
		}
	}
}
