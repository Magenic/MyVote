using System;
using MyVote.BusinessObjects.Core.Contracts;

namespace MyVote.BusinessObjects.Contracts
{
	public interface IPollComment
		: IBusinessBaseCore
	{
		DateTime? CommentDate { get; }
		IPollCommentCollection Comments { get; }
		int? PollCommentID { get; }
		string CommentText { get; set; }
		int UserID { get; }
		string UserName { get; }
	}
}
