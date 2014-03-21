using MyVote.BusinessObjects.Contracts;
using MyVote.UI.W8.Tests.Mocks.Base;

namespace MyVote.UI.W8.Tests.Mocks
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
	public class PollSubmissionResponseCollectionMock
		: BusinessListBaseCoreMock<IPollSubmissionResponse>, IPollSubmissionResponseCollection
	{
	}
}
