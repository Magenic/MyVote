using System;
using System.Linq.Expressions;

namespace MyVote.Data.Entities
{
	public interface ISearchWhereClause
	{
		Expression<Func<Mvpoll, bool>> WhereClause(DateTime now, string stringPattern);
	}
}
