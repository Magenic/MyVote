using MyVote.BusinessObjects;

namespace MyVote.UI.ViewModels
{
    public sealed class PollSearchOptionViewModel
    {
		public string Display { get; set; }
		public PollSearchResultsQueryType QueryType { get; set; }
    }
}
