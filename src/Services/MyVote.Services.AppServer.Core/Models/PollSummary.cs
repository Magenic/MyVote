namespace MyVote.Services.AppServer.Models
{
	public sealed class PollSummary
	{
		public string Category { get; set; }
		public int Id { get; set; }
		public string ImageLink { get; set; }
		public string Question { get; set; }
		public int SubmissionCount { get; set; }
	}
}
