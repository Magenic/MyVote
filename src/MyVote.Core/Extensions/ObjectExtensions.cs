using System;
using System.Linq.Expressions;
using Csla.Reflection;

namespace MyVote.Core.Extensions
{
	public static class ObjectExtensions
	{
		public static string GetPropertyName<T>(this T @this, Expression<Func<T, object>> propertyExpression)
		{
			return Reflect<T>.GetProperty(propertyExpression).Name;
		}
	}
}
