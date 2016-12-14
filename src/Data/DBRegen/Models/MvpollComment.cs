using System;
using System.Collections.Generic;

namespace DBRegen.Models
{
    public partial class MvpollComment
    {
        public int PollCommentId { get; set; }
        public int UserId { get; set; }
        public string CommentText { get; set; }
        public DateTime? CommentDate { get; set; }
        public int? ParentCommentId { get; set; }
        public int PollId { get; set; }
        public bool? PollCommentDeletedFlag { get; set; }
        public DateTime? PollCommentDeletedDate { get; set; }

        public virtual Mvpoll Poll { get; set; }
        public virtual Mvuser User { get; set; }
    }
}
