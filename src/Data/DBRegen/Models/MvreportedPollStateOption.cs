using System;
using System.Collections.Generic;

namespace DBRegen.Models
{
    public partial class MvreportedPollStateOption
    {
        public MvreportedPollStateOption()
        {
            MvreportedPoll = new HashSet<MvreportedPoll>();
        }

        public int ReportedPollStateOptionId { get; set; }
        public string ReportedPollStateName { get; set; }
        public string ReportedPollStateComments { get; set; }

        public virtual ICollection<MvreportedPoll> MvreportedPoll { get; set; }
    }
}
