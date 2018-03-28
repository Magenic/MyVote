using System;
using System.Diagnostics.CodeAnalysis;
using MyVote.BusinessObjects.Core.Contracts;

namespace MyVote.BusinessObjects.Contracts
{
	public interface IPollSubmission
		: IBusinessBaseCore
	{
		string CategoryName { get; }
		string Comment { get; set; }
		bool IsActive { get; }
		bool IsPollOwnedByUser { get; }
		[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flag")]
		bool? PollDeletedFlag { get; }
		DateTime PollEndDate { get; }
		string PollImageLink { get; }
		short PollMaxAnswers { get; }
		short PollMinAnswers { get; }
		DateTime PollStartDate { get; }
		int? PollSubmissionID { get; }
		int PollID { get; }
		string PollDescription { get; }
		string PollQuestion { get; }
		IPollSubmissionResponseCollection Responses { get; }
		int UserID { get; }
		DateTime? SubmissionDate { get; }
	}
}
