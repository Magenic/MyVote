using System.Collections.Generic;

namespace MyVote.Services.AppServer.Models
{
	public sealed class PollResponse
	{
		public int PollID { get; set; }
		public int UserID { get; set; }
		public string Comment { get; set; }
		public List<ResponseItem> ResponseItems { get; set; }
	}
}
