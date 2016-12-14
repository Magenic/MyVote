using System.Collections.Generic;

namespace Models.WebService
{
	public sealed class PollResult
	{
		public int PollID { get; set; }
		public bool IsPollOwnedByUser { get; set; }
		public bool IsActive { get; set; }
		public string PollImageLink { get; set; }
		public string Question { get; set; }
		public List<PollResultItem> Results { get; set; }
		public List<PollResultComment> Comments { get; set; }
	}
}