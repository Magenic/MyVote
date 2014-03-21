using System;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core.Contracts;
using MyVote.UI.W8.Tests.Mocks.Base;

namespace MyVote.UI.W8.Tests.Mocks
{
	public class PollDataResultsMock : BusinessBaseCoreMock, IPollDataResults
	{
		public int PollID { get; set; }

		public string Question { get; set; }

		public Func<IReadOnlyListBaseCore<IPollDataResult>> ResultsDelegate { get; set; }
		public IReadOnlyListBaseCore<IPollDataResult> Results
		{
			get
			{
				if (ResultsDelegate != null)
				{
					return ResultsDelegate();
				}
				else
				{
					return null;
				}
			}
		}
	}
}
