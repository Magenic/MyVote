using System;
using System.Collections.Generic;

namespace MyVote.Data.Entities
{
    public partial class MvpollOption
    {
        public MvpollOption()
        {
            MvpollResponse = new HashSet<MvpollResponse>();
        }

        public int PollOptionId { get; set; }
        public int PollId { get; set; }
        public short OptionPosition { get; set; }
        public string OptionText { get; set; }

        public virtual ICollection<MvpollResponse> MvpollResponse { get; set; }
        public virtual Mvpoll Poll { get; set; }
    }
}
