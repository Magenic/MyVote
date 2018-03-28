using System;
using MyVote.BusinessObjects.Core;
using MyVote.BusinessObjects.Core.Contracts;

namespace MyVote.BusinessObjects.Contracts
{
	public interface IPoll
		: IBusinessBaseCore
	{
		int? PollID { get; }
		int UserID { get; }
		BusinessList<IPollOption> PollOptions { get; }
		int? PollCategoryID { get; set; }
		string PollDescription { get; set; }
		string PollQuestion { get; set; }
		string PollImageLink { get; set; }
		short? PollMaxAnswers { get; set; }
		short? PollMinAnswers { get; set; }
		DateTime? PollStartDate { get; set; }
		DateTime? PollEndDate { get; set; }
		bool? PollAdminRemovedFlag { get; set; }
		DateTime? PollDateRemoved { get; set; }
		bool? PollDeletedFlag { get; set; }
		DateTime? PollDeletedDate { get; set; }
	}
}
