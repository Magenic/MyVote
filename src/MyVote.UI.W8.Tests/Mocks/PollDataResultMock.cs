using MyVote.BusinessObjects.Contracts;
using MyVote.UI.W8.Tests.Mocks.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVote.UI.W8.Tests.Mocks
{
	public class PollDataResultMock : BusinessBaseCoreMock, IPollDataResult
	{
		public string OptionText { get; set; }

		public int PollOptionID { get; set; }

		public int ResponseCount { get; set; }
	}
}
