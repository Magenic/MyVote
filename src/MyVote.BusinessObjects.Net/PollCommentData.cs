using System;
using MyVote.Repository;

namespace MyVote.BusinessObjects
{

	[Serializable]
	internal sealed class PollCommentData
	{
		internal MVPollComment Comment { get; set; }
		internal string UserName { get; set; }
	}
}
