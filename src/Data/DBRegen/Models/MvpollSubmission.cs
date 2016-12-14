using System;
using System.Collections.Generic;

namespace DBRegen.Models
{
    public partial class MvpollSubmission
    {
        public MvpollSubmission()
        {
            MvpollResponse = new HashSet<MvpollResponse>();
        }

        public int PollSubmissionId { get; set; }
        public int PollId { get; set; }
        public int UserId { get; set; }
        public DateTime PollSubmissionDate { get; set; }
        public string PollSubmissionComment { get; set; }

        public virtual ICollection<MvpollResponse> MvpollResponse { get; set; }
        public virtual Mvpoll Poll { get; set; }
        public virtual Mvuser User { get; set; }
    }
}
