namespace MyVote.Services.AppServer.Models
{
	public sealed class PollOption
	{
		public int? PollOptionID { get; set; }
		public int? PollID { get; set; }
		public short? OptionPosition { get; set; }
		public string OptionText { get; set; }
	}
}
