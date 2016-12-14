using System;
using System.Collections.Generic;

namespace DBRegen.Models
{
    public partial class MvreportedPollStateLog
    {
        public int ReportedPollStateLogId { get; set; }
        public int ReportedPollId { get; set; }
        public int PollId { get; set; }
        public int StateAdminUserId { get; set; }
        public DateTime DateStateChanged { get; set; }
        public string StateChangeComments { get; set; }

        public virtual Mvpoll Poll { get; set; }
        public virtual MvreportedPoll ReportedPoll { get; set; }
        public virtual Mvuser StateAdminUser { get; set; }
    }
}
