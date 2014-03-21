using System;
using System.Collections.Generic;

namespace MyVote.AppServer.Models
{
	public class PollResult
	{
		public int PollID { get; set; }
		public bool IsPollOwnedByUser { get; set; }
		public bool IsActive { get; set; }
		public string PollImageLink { get; set; }
		public string Question { get; set; }
		public List<PollResultItem> Results { get; set; }
		public List<PollResultComment> Comments { get; set; }
	}

	public class PollResultItem
	{
		public string OptionText { get; set; }
		public int PollOptionID { get; set; }
		public int ResponseCount { get; set; }
	}
}