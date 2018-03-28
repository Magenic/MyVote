using MyVote.BusinessObjects;

namespace MyVote.UI.ViewModels
{
    public sealed class PollSearchOptionViewModel : ViewModelBase
    {
		public string Display { get; set; }
		public PollSearchResultsQueryType QueryType { get; set; }
    }
}
