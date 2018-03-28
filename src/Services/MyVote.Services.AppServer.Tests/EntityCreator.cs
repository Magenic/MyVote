using System;
using System.Linq;
using System.Reflection;
using Spackle;

namespace MyVote.Services.AppServer.Tests
{
	internal static class EntityCreator
	{
		internal static T Create<T>()
			where T : new()
		{
			var generator = new RandomObjectGenerator();

			var entity = new T();

			foreach (var property in
				(from p in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
				 where p.CanWrite
				 select p))
			{
				try
				{
					property.SetValue(entity,
						typeof(RandomObjectGenerator)
							.GetMethod(nameof(RandomObjectGenerator.Generate), Type.EmptyTypes)
							.MakeGenericMethod(new[] { property.PropertyType })
							.Invoke(generator, null));
				}
				catch (TargetInvocationException) { }
			}

			return entity;
		}

		internal static T Create<T>(Action<T> modifier)
			where T : new()
		{
			var entity = EntityCreator.Create<T>();
			modifier(entity);
			return entity;
		}
	}
}
