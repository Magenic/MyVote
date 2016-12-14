namespace MyVote.Services.AppServer.Models
{
	public sealed class ResponseItem
	{
		public int PollOptionID { get; set; }
		public bool IsOptionSelected { get; set; }
	}
}