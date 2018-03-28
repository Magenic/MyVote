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
				return @this.GetType().GetTypeInfo()
					.GetDeclaredProperty(nameof(Task<int>.Result))
					.GetValue(@this) as T;
			}
		}
	}
}
