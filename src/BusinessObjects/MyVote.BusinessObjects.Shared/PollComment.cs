using System;
using Csla;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;
using MyVote.BusinessObjects.Attributes;

#if !NETFX_CORE && !MOBILE
using System.Collections.Generic;
using System.Linq;
using MyVote.Data.Entities;
#endif

namespace MyVote.BusinessObjects
{
	[System.Serializable]
	internal sealed class PollComment
		: BusinessBaseCore<PollComment>, IPollComment
	{
		[RunLocal]
		private void Child_Create(int userId, string userName)
		{
			this.UserID = userId;
			this.UserName = userName;
			this.CommentDate = DateTime.UtcNow;
			this.Comments = this.pollCommentsFactory.CreateChild();
		}

#if !NETFX_CORE && !MOBILE
		private void Child_Fetch(PollCommentData commentData, List<PollCommentData> commentsData)
		{
			using (this.BypassPropertyChecks)
			{
				var comment = commentData.Comment;
				this.CommentDate = comment.CommentDate;
				this.CommentText = comment.CommentText;
				this.PollCommentID = comment.PollCommentID;
				this.UserID = comment.UserID;
				this.UserName = commentData.UserName;
				this.Comments = this.pollCommentsFactory.FetchChild();

				foreach (var childComment in
					(from c in commentsData
					 where c.Comment.ParentCommentID == comment.PollCommentID
					 select c).ToList())
				{
					this.Comments.Add(this.pollCommentFactory.FetchChild(childComment, commentsData));
				}
			}
		}

		private void Child_Insert(int pollId, int? parentCommentId)
		{
			var entity = new MVPollComment();
			entity.CommentDate = this.CommentDate;
			entity.CommentText = this.CommentText;
			entity.ParentCommentID = parentCommentId;
			entity.PollID = pollId;
			entity.UserID = this.UserID;

			this.Entities.MVPollComments.Add(entity);
			this.Entities.SaveChanges();
			this.PollCommentID = entity.PollCommentID;

			this.FieldManager.UpdateChildren(pollId, this.PollCommentID);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "parentCommentId")]
		private void Child_Update(int pollId, int? parentCommentId)
		{
			this.FieldManager.UpdateChildren(pollId, this.PollCommentID);
		}
#endif

		public static readonly PropertyInfo<DateTime?> CommentDateProperty =
			PollComment.RegisterProperty<DateTime?>(_ => _.CommentDate);
		public DateTime? CommentDate
		{
			get { return this.ReadProperty(PollComment.CommentDateProperty); }
			private set { this.LoadProperty(PollComment.CommentDateProperty, value); }
		}

		public static readonly PropertyInfo<IPollCommentCollection> CommentsProperty =
			PollComment.RegisterProperty<IPollCommentCollection>(_ => _.Comments);
		public IPollCommentCollection Comments
		{
			get { return this.ReadProperty(PollComment.CommentsProperty); }
			private set { this.LoadProperty(PollComment.CommentsProperty, value); }
		}

		public static readonly PropertyInfo<string> CommentTextProperty =
			PollComment.RegisterProperty<string>(_ => _.CommentText);
		public string CommentText
		{
			get { return this.GetProperty(PollComment.CommentTextProperty); }
			set { this.SetProperty(PollComment.CommentTextProperty, value); }
		}

		public static readonly PropertyInfo<int?> PollCommentIDProperty =
			PollComment.RegisterProperty<int?>(_ => _.PollCommentID);
		public int? PollCommentID
		{
			get { return this.ReadProperty(PollComment.PollCommentIDProperty); }
			private set { this.LoadProperty(PollComment.PollCommentIDProperty, value); }
		}

		public static readonly PropertyInfo<int> UserIDProperty =
			PollComment.RegisterProperty<int>(_ => _.UserID);
		public int UserID
		{
			get { return this.ReadProperty(PollComment.UserIDProperty); }
			private set { this.LoadProperty(PollComment.UserIDProperty, value); }
		}

		public static readonly PropertyInfo<string> UserNameProperty =
			PollComment.RegisterProperty<string>(_ => _.UserName);
		public string UserName
		{
			get { return this.ReadProperty(PollComment.UserNameProperty); }
			private set { this.LoadProperty(PollComment.UserNameProperty, value); }
		}

#if !NETFX_CORE && !MOBILE
		[NonSerialized]
#endif
		private IObjectFactory<IPollCommentCollection> pollCommentsFactory;
		[Dependency]
		public IObjectFactory<IPollCommentCollection> PollCommentsFactory
		{
			get { return this.pollCommentsFactory; }
			set { this.pollCommentsFactory = value; }
		}

#if !NETFX_CORE && !MOBILE
		[NonSerialized]
		private IObjectFactory<IPollComment> pollCommentFactory;
		[Dependency]
		public IObjectFactory<IPollComment> PollCommentFactory
		{
			get { return this.pollCommentFactory; }
			set { this.pollCommentFactory = value; }
		}
#endif
	}
}
