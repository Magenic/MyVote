using System;
#if NETFX_CORE
using System.Reflection;
#endif

namespace MyVote.BusinessObjects.Extensions
{
	public static class TypeExtensions
	{
		public static Type GetConcreteType(this Type @this)
		{
#if !NETFX_CORE
			if (@this == null || !@this.IsInterface || string.IsNullOrWhiteSpace(@this.Namespace))
			{
				return @this;
			}

			var @namespace = @this.Namespace.Replace(".Contracts", string.Empty);

			return Type.GetType($"{@namespace}.{@this.Name.Substring(1)}, {@this.Assembly.FullName}");
#else
			if (@this == null)
			{
				return null;
			}
			else
			{
				var thisTypeInfo = @this.GetTypeInfo();
				if (!thisTypeInfo.IsInterface || string.IsNullOrWhiteSpace(@this.Namespace))
				{
					return @this;
				}

				var @namespace = @this.Namespace.Replace(".Contracts", string.Empty);

				return Type.GetType($"{@namespace}.{@this.Name.Substring(1)}, {thisTypeInfo.Assembly.FullName}");
			}
#endif
		}
	}
}
