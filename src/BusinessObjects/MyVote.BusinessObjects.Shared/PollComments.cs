using System;
using System.Linq;
using Csla;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;

#if !NETFX_CORE && !MOBILE
using MyVote.BusinessObjects.Attributes;
#endif

namespace MyVote.BusinessObjects
{
	[Serializable]
	internal sealed class PollComments
		: BusinessBaseCore<PollComments>, IPollComments
	{
#if !NETFX_CORE && !MOBILE
		private void Child_Fetch(int pollId)
		{
			using (this.BypassPropertyChecks)
			{
				this.PollID = pollId;
				this.Comments = this.pollCommentsFactory.FetchChild();

				var commentsData = (from c in this.Entities.MvpollComment
										  where c.PollId == pollId
										  join u in this.Entities.Mvuser on c.UserId equals u.UserId
										  orderby c.ParentCommentId, c.CommentDate
										  select new PollCommentData { Comment = c, UserName = u.UserName }).ToList();

				foreach (var commentData in
					(from c in commentsData
					 where c.Comment.ParentCommentId == null
					 select c).ToList())
				{
					this.Comments.Add(this.pollCommentFactory.FetchChild(commentData, commentsData));
				}
			}
		}

		protected override void Child_Update()
		{
			this.FieldManager.UpdateChildren(this.PollID, null as int?);
		}
#endif

		public static readonly PropertyInfo<int> PollIdProperty =
			PollComments.RegisterProperty<int>(_ => _.PollID);
		public int PollID
		{
			get { return this.GetProperty(PollComments.PollIdProperty); }
			private set { this.SetProperty(PollComments.PollIdProperty, value); }
		}

		public static readonly PropertyInfo<IPollCommentCollection> CommentsProperty =
			PollComments.RegisterProperty<IPollCommentCollection>(_ => _.Comments);
		public IPollCommentCollection Comments
		{
			get { return this.GetProperty(PollComments.CommentsProperty); }
			private set { this.SetProperty(PollComments.CommentsProperty, value); }
		}

#if !NETFX_CORE && !MOBILE
		[NonSerialized]
		private IObjectFactory<IPollCommentCollection> pollCommentsFactory;
		[Dependency]
		public IObjectFactory<IPollCommentCollection> PollCommentsFactory
		{
			get { return this.pollCommentsFactory; }
			set { this.pollCommentsFactory = value; }
		}

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

