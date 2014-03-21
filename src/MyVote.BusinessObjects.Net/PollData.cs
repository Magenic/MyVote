namespace MyVote.BusinessObjects
{
	public sealed class PollData
	{
		public string OptionText { get; set; }
		public int PollOptionID { get; set; }
		public int ResponseCount { get; set; }
	}
}
