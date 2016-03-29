using System;
using System.Reflection;
using System.Threading.Tasks;

namespace MyVote.BusinessObjects.Extensions
{
	internal static class TaskExtensions
	{
		internal static T HandleTask<T>(this Task @this)
			where T : class
		{
			if (@this.Exception != null)
			{
				throw @this.Exception;
			}
			else
			{
#if !NETFX_CORE
				return @this.GetType().GetProperty(nameof(Task<int>.Result))
					.GetValue(@this) as T;
#else
				return @this.GetType().GetTypeInfo().GetDeclaredProperty(nameof(Task<int>.Result))
					.GetValue(@this) as T;
#endif
			}
		}
	}
}
