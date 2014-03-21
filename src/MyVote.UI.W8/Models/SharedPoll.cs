using System.Collections.Generic;

namespace MyVote.UI.Models
{
	public sealed class SharedPoll
	{
		public SharedPoll()
		{
			this.Options = new List<string>();
		}

		public int PollId { get; set; }
		public string Question { get; set; }
		public string Description { get; set; }
		public List<string> Options { get; private set; }
	}
}
