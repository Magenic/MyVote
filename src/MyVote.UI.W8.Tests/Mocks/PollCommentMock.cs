using System;
using System.Diagnostics.CodeAnalysis;
using MyVote.BusinessObjects.Contracts;
using MyVote.UI.W8.Tests.Mocks.Base;

namespace MyVote.UI.W8.Tests.Mocks
{
	public class PollCommentMock : BusinessBaseCoreMock, IPollComment
	{
		public DateTime? CommentDate { get; set; }

		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public IPollCommentCollection Comments { get; set; }

		public int? PollCommentID { get; set; }

		public string CommentText { get; set; }

		public int UserID { get; set; }

		public string UserName { get; set; }
	}
}
