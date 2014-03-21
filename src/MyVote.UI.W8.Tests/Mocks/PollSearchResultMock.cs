using MyVote.BusinessObjects.Contracts;
using MyVote.UI.W8.Tests.Mocks.Base;

namespace MyVote.UI.W8.Tests.Mocks
{
	public class PollSearchResultMock : BusinessBaseCoreMock, IPollSearchResult
	{
		public int Id { get; set; }

		public string ImageLink { get; set; }

		public string Question { get; set; }

		public int SubmissionCount { get; set; }
	}
}
