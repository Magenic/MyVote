namespace MyVote.Services.AppServer.Models
{
	public sealed class PollResponseOption
	{
		public int? PollResponseID { get; set; }
		public int PollOptionID { get; set; }
		public bool IsOptionSelected { get; set; }
		public short OptionPosition { get; set; }
		public string OptionText { get; set; }
	}
}
