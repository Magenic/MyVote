using System;
using System.Collections.Generic;

namespace Models.WebService
{
	public sealed class PollResultComment
	{
		public int PollID { get; set; }
		public int PollCommentID { get; set; }
		public int? ParentCommentID { get; set; }
		public DateTime? CommentDate { get; set; }
		public string CommentText { get; set; }
		public List<PollResultComment> Comments { get; set; }
		public string UserName { get; set; }
		public int UserID { get; set; }
	}
}