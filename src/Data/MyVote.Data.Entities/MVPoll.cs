using System;
using System.Collections.Generic;

namespace MyVote.Data.Entities
{
    public partial class Mvpoll
    {
        public Mvpoll()
        {
            MvpollComment = new HashSet<MvpollComment>();
            MvpollOption = new HashSet<MvpollOption>();
            MvpollResponse = new HashSet<MvpollResponse>();
            MvpollSubmission = new HashSet<MvpollSubmission>();
            MvreportedPoll = new HashSet<MvreportedPoll>();
            MvreportedPollStateLog = new HashSet<MvreportedPollStateLog>();
        }

        public int PollId { get; set; }
        public int UserId { get; set; }
        public int PollCategoryId { get; set; }
        public string PollQuestion { get; set; }
        public string PollDescription { get; set; }
        public string PollImageLink { get; set; }
        public short? PollMaxAnswers { get; set; }
        public short? PollMinAnswers { get; set; }
        public DateTime PollStartDate { get; set; }
        public DateTime PollEndDate { get; set; }
        public bool? PollAdminRemovedFlag { get; set; }
        public DateTime? PollDateRemoved { get; set; }
        public bool? PollDeletedFlag { get; set; }
        public DateTime? PollDeletedDate { get; set; }
        public DateTime? AuditDateCreated { get; set; }
        public DateTime? AuditDateModified { get; set; }

        public virtual ICollection<MvpollComment> MvpollComment { get; set; }
        public virtual ICollection<MvpollOption> MvpollOption { get; set; }
        public virtual ICollection<MvpollResponse> MvpollResponse { get; set; }
        public virtual ICollection<MvpollSubmission> MvpollSubmission { get; set; }
        public virtual ICollection<MvreportedPoll> MvreportedPoll { get; set; }
        public virtual ICollection<MvreportedPollStateLog> MvreportedPollStateLog { get; set; }
        public virtual Mvcategory PollCategory { get; set; }
        public virtual Mvuser User { get; set; }
    }
}
