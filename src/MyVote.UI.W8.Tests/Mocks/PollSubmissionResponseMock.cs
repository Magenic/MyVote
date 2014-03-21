using MyVote.BusinessObjects.Contracts;
using MyVote.UI.W8.Tests.Mocks.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVote.UI.W8.Tests.Mocks
{
	public class PollSubmissionResponseMock : BusinessBaseCoreMock, IPollSubmissionResponse
	{
		public int? PollResponseID { get; set; }

		public int PollOptionID { get; set; }

		public bool IsOptionSelected { get; set; }

		public short OptionPosition { get; set; }

		public string OptionText { get; set; }
	}
}
