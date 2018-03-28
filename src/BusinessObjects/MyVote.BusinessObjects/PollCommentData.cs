using System;
#if !MOBILE
using MyVote.Data.Entities;
#endif

namespace MyVote.BusinessObjects
{

    [Serializable]
    internal sealed class PollCommentData
    {
#if !MOBILE
        internal MvpollComment Comment { get; set; }
#endif
        internal string UserName { get; set; }
	}
}
