using MyVote.BusinessObjects;
using MyVote.UI.ViewModels;

namespace MyVote.UI.ViewModels
{
	public sealed class PollSearchOptionViewModel
	{
		public string Display { get; set; }
		public PollSearchResultsQueryType QueryType { get; set; }
	}
}
