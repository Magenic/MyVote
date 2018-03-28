using System;
using System.Collections.Generic;

namespace MyVote.Data.Entities
{
    public partial class MvpollResponse
    {
        public int PollResponseId { get; set; }
        public int PollId { get; set; }
        public int PollSubmissionId { get; set; }
        public int UserId { get; set; }
        public int PollOptionId { get; set; }
        public bool OptionSelected { get; set; }
        public DateTime ResponseDate { get; set; }

        public virtual Mvpoll Poll { get; set; }
        public virtual MvpollOption PollOption { get; set; }
        public virtual MvpollSubmission PollSubmission { get; set; }
        public virtual Mvuser User { get; set; }
    }
}
