using System;
using MyVote.Data.Entities;

namespace MyVote.BusinessObjects
{

	[Serializable]
	internal sealed class PollCommentData
	{
		internal MvpollComment Comment { get; set; }
		internal string UserName { get; set; }
	}
}
