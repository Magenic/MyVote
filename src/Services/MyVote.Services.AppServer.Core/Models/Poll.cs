using System;
using System.Collections.Generic;

namespace MyVote.Services.AppServer.Models
{
	public sealed class Poll
	{
		public int PollID { get; set; }
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