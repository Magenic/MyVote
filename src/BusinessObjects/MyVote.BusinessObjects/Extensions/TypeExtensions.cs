using System;
using System.Reflection;

namespace MyVote.BusinessObjects.Extensions
{
	public static class TypeExtensions
	{
		public static Type GetConcreteType(this Type @this)
		{
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
		}
	}
}
