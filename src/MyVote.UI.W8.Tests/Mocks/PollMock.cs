using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;
using MyVote.UI.W8.Tests.Mocks.Base;
using System;

namespace MyVote.UI.W8.Tests.Mocks
{
	public class PollMock : BusinessBaseCoreMock, IPoll
	{
		public int? PollID { get; set; }

		public int UserID { get; set; }

		public Func<BusinessList<IPollOption>> PollOptionsDelegate { get; set; }
		public BusinessList<IPollOption> PollOptions
		{
			get
			{
				if (PollOptionsDelegate != null)
				{
					return PollOptionsDelegate();
				}
				else
				{
					return null;
				}
			}
		}

		public int? PollCategoryID { get; set; }

		public string PollDescription { get; set; }

		public string PollQuestion { get; set; }

		public string PollImageLink { get; set; }

		public short? PollMaxAnswers { get; set; }

		public short? PollMinAnswers { get; set; }

		public DateTime? PollStartDate { get; set; }

		public DateTime? PollEndDate { get; set; }

		public bool? PollAdminRemovedFlag { get; set; }

		public DateTime? PollDateRemoved { get; set; }

		public bool? PollDeletedFlag { get; set; }

		public DateTime? PollDeletedDate { get; set; }
	}
}
