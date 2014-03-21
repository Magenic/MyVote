using Csla.Core;
using MyVote.BusinessObjects.Contracts;
using MyVote.UI.W8.Tests.Mocks.Base;

namespace MyVote.UI.W8.Tests.Mocks
{
	public class PollOptionMock : BusinessBaseCoreMock, IPollOption
	{
		public int? PollOptionID { get; set; }

		public int? PollID { get; set; }

		public short? OptionPosition { get; set; }

		public string OptionText { get; set; }

		public override void SetParent(IParent parent) { }
	}
}
