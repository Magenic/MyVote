using System;
using System.Data.Entity.SqlServer;
using System.Linq.Expressions;

namespace MyVote.Data.Entities
{
    public sealed class SearchWhereClause
		 : ISearchWhereClause
    {
		 public Expression<Func<MVPoll, bool>> WhereClause(DateTime now, string stringPattern)
		 {
			 return poll => (poll.PollStartDate < now && poll.PollEndDate > now &&
					 SqlFunctions.PatIndex(stringPattern, poll.PollQuestion.ToLower()) > 0 &&
					 (poll.PollDeletedFlag == null || !poll.PollDeletedFlag.Value));
		 }
	 }
}
