namespace MyVote.Services.AppServer.Models
{
	public sealed class PollResultItem
	{
		public string OptionText { get; set; }
		public int PollOptionID { get; set; }
		public int ResponseCount { get; set; }
	}
}
