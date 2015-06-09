using System;
using System.Linq.Expressions;
using MyVote.Data.Entities;

namespace MyVote.Data.Entities
{
	public interface ISearchWhereClause
	{
		Expression<Func<MVPoll, bool>> WhereClause(DateTime now, string stringPattern);
	}
}
