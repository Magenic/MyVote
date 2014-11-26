using System;
using System.Linq;
using Csla;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;

#if !NETFX_CORE && !WINDOWS_PHONE && !ANDROID && !IOS
using MyVote.BusinessObjects.Attributes;
#endif

namespace MyVote.BusinessObjects
{
#if (!NETFX_CORE && !WINDOWS_PHONE) || ANDROID
	[System.Serializable]
#else
	[Csla.Serialization.Serializable]
#endif
	internal sealed class PollComments
		: BusinessBaseScopeCore<PollComments>, IPollComments
	{
#if !NETFX_CORE && !WINDOWS_PHONE && !ANDROID && !IOS
		private void Child_Fetch(int pollId)
		{
			using (this.BypassPropertyChecks)
			{
				this.PollID = pollId;
				this.Comments = this.pollCommentsFactory.FetchChild();

				var commentsData = (from c in this.Entities.MVPollComments
									where c.PollID == pollId
									join u in this.Entities.MVUsers on c.UserID equals u.UserID
									orderby c.ParentCommentID, c.CommentDate
									select new PollCommentData { Comment = c, UserName = u.UserName }).ToList();

				foreach (var commentData in
					(from c in commentsData
					 where c.Comment.ParentCommentID == null
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

		public static PropertyInfo<int> PollIdProperty =
			PollComments.RegisterProperty<int>(_ => _.PollID);
		public int PollID
		{
			get { return this.GetProperty(PollComments.PollIdProperty); }
			private set { this.SetProperty(PollComments.PollIdProperty, value); }
		}

		public static PropertyInfo<IPollCommentCollection> CommentsProperty =
			PollComments.RegisterProperty<IPollCommentCollection>(_ => _.Comments);
		public IPollCommentCollection Comments
		{
			get { return this.GetProperty(PollComments.CommentsProperty); }
			private set { this.SetProperty(PollComments.CommentsProperty, value); }
		}

#if !NETFX_CORE && !WINDOWS_PHONE && !ANDROID && !IOS
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

