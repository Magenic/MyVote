using System;
using System.Collections.Generic;

namespace MyVote.Services.AppServer.Models
{
	public sealed class PollInfo
	{
		public int? PollSubmissionID { get; set; }
		public int PollID { get; set; }
		public string PollDescription { get; set; }
		public string PollQuestion { get; set; }
		public int MaxAnswers { get; set; }
		public int MinAnswers { get; set; }
		public int UserID { get; set; }
		public DateTime? SubmissionDate { get; set; }
		public string Comment { get; set; }
		public List<PollResponseOption> PollOptions { get; set; }
	}
}
