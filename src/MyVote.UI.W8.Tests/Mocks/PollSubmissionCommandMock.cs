using MyVote.BusinessObjects.Contracts;
using MyVote.UI.W8.Tests.Mocks.Base;

namespace MyVote.UI.W8.Tests.Mocks
{
	public class PollSubmissionCommandMock : CommandBaseMock, IPollSubmissionCommand
	{
		public int PollID { get; set; }

		public IPollSubmission Submission { get; set; }

		public int UserID { get; set; }
	}
}
