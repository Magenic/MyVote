using System;
using System.Collections.Generic;

namespace Models.WebService
{
	public sealed class Poll
	{
        public Poll(int? pollID, int userID, int pollCategoryID, string pollQuestion, string pollImageLink, short pollMaxAnswers, short pollMinAnswers, DateTime? pollStartDate, DateTime? pollEndDate, bool pollAdminRemovedFlag, DateTime? pollDateRemoved, bool pollDeletedFlag, DateTime? pollDeletedDate, string pollDescription, List<PollOption> pollOptions)
        {
            this.PollID = pollID;
            this.UserID = userID;
            this.PollCategoryID = pollCategoryID;
            this.PollQuestion = pollQuestion;
            this.PollImageLink = pollImageLink;
            this.PollMaxAnswers = pollMaxAnswers;
            this.PollMinAnswers = pollMinAnswers;
            this.PollStartDate = pollStartDate;
            this.PollEndDate = pollEndDate;
            this.PollAdminRemovedFlag = pollAdminRemovedFlag;
            this.PollDateRemoved = pollDateRemoved;
            this.PollDeletedFlag = pollDeletedFlag;
            this.PollDeletedDate = pollDeletedDate;
            this.PollDescription = pollDescription;
            this.PollOptions = pollOptions;
        }

        public int? PollID { get; set; }
		public int UserID { get; set; }
		public int PollCategoryID { get; set; }
		public string PollQuestion { get; set; }
		public string PollImageLink { get; set; }
		public short PollMaxAnswers { get; set; }
		public short PollMinAnswers { get; set; }
		public DateTime? PollStartDate { get; set; }
		public DateTime? PollEndDate { get; set; }
		public bool PollAdminRemovedFlag { get; set; }
		public DateTime? PollDateRemoved { get; set; }
		public bool PollDeletedFlag { get; set; }
		public DateTime? PollDeletedDate { get; set; }
		public string PollDescription { get; set; }
		public List<PollOption> PollOptions { get; set; }
	}
}