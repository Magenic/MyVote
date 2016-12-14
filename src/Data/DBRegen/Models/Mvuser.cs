using System;
using System.Collections.Generic;

namespace DBRegen.Models
{
    public partial class Mvuser
    {
        public Mvuser()
        {
            Mvpoll = new HashSet<Mvpoll>();
            MvpollComment = new HashSet<MvpollComment>();
            MvpollResponse = new HashSet<MvpollResponse>();
            MvpollSubmission = new HashSet<MvpollSubmission>();
            MvreportedPollCurrentStateAdminUser = new HashSet<MvreportedPoll>();
            MvreportedPollReportedByUser = new HashSet<MvreportedPoll>();
            MvreportedPollStateLog = new HashSet<MvreportedPollStateLog>();
        }

        public int UserId { get; set; }
        public string UserName { get; set; }
        public string ProfileId { get; set; }
        public string ProfileAuthToken { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string PostalCode { get; set; }
        public DateTime? BirthDate { get; set; }
        public int? UserRoleId { get; set; }
        public DateTime? AuditCreateDate { get; set; }
        public DateTime? AuditModifyDate { get; set; }

        public virtual ICollection<Mvpoll> Mvpoll { get; set; }
        public virtual ICollection<MvpollComment> MvpollComment { get; set; }
        public virtual ICollection<MvpollResponse> MvpollResponse { get; set; }
        public virtual ICollection<MvpollSubmission> MvpollSubmission { get; set; }
        public virtual ICollection<MvreportedPoll> MvreportedPollCurrentStateAdminUser { get; set; }
        public virtual ICollection<MvreportedPoll> MvreportedPollReportedByUser { get; set; }
        public virtual ICollection<MvreportedPollStateLog> MvreportedPollStateLog { get; set; }
        public virtual MvuserRole UserRole { get; set; }
    }
}
