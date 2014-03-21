using System.Diagnostics.CodeAnalysis;
using MyVote.BusinessObjects.Contracts;
using MyVote.UI.W8.Tests.Mocks.Base;

namespace MyVote.UI.W8.Tests.Mocks
{
	public class PollResultsMock : BusinessBaseCoreMock, IPollResults
	{
		public int PollID { get; set; }

		public IPollDataResults PollDataResults { get; set; }

		public IPollComments PollComments { get; set; }

		public bool IsActive { get; set; }

		public bool IsPollOwnedByUser { get; set; }

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		public string PollImageLink { get; private set; }
	}
}
