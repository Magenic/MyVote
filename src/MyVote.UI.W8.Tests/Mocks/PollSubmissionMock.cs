using System;
using System.Diagnostics.CodeAnalysis;
using MyVote.BusinessObjects.Contracts;
using MyVote.UI.W8.Tests.Mocks.Base;

namespace MyVote.UI.W8.Tests.Mocks
{
	public class PollSubmissionMock : BusinessBaseCoreMock, IPollSubmission
	{
		public PollSubmissionMock()
		{
			Responses = new PollSubmissionResponseCollectionMock();
		}

		public string Comment { get; set; }

		public int? PollSubmissionID { get; set; }

		public int PollID { get; set; }

		public string PollDescription { get; set; }

		public string CategoryName { get; set; }

		public string PollQuestion { get; set; }

		public PollSubmissionResponseCollectionMock Responses { get; private set; }
		IPollSubmissionResponseCollection IPollSubmission.Responses
		{
			get
			{
				return this.Responses;
			}
		}

		public int UserID { get; set; }

		public DateTime? SubmissionDate { get; set; }

		public short PollMaxAnswers { get; set; }

		public short PollMinAnswers { get; set; }


		public bool IsActive { get; set; }

		public bool IsPollOwnedByUser { get; set; }

		public bool? PollDeletedFlag { get; set; }

		public DateTime PollEndDate { get; set; }

		public DateTime PollStartDate { get; set; }

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		public string PollImageLink { get; private set; }
	}
}
