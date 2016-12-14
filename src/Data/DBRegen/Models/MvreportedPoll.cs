using System;
using System.Collections.Generic;

namespace DBRegen.Models
{
    public partial class MvreportedPoll
    {
        public MvreportedPoll()
        {
            MvreportedPollStateLog = new HashSet<MvreportedPollStateLog>();
        }

        public int ReportedPollId { get; set; }
        public int PollId { get; set; }
        public int ReportedByUserId { get; set; }
        public int? CurrentStateAdminUserId { get; set; }
        public DateTime DateReported { get; set; }
        public DateTime? DateCurrentStateChanged { get; set; }
        public int ReportedPollStateOptionId { get; set; }
        public string ReportComments { get; set; }

        public virtual ICollection<MvreportedPollStateLog> MvreportedPollStateLog { get; set; }
        public virtual Mvuser CurrentStateAdminUser { get; set; }
        public virtual Mvpoll Poll { get; set; }
        public virtual Mvuser ReportedByUser { get; set; }
        public virtual MvreportedPollStateOption ReportedPollStateOption { get; set; }
    }
}
