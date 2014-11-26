using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Autofac;
using Csla.Server;
using MyVote.BusinessObjects.Attributes;
using MyVote.BusinessObjects.Core.Contracts;
using MyVote.BusinessObjects.Extensions;

namespace MyVote.BusinessObjects
{
	public sealed class ObjectActivator
		: IDataPortalActivator
	{
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		private IContainer container;

		public ObjectActivator(IContainer container)
		{
			if (container == null)
			{
				throw new ArgumentNullException("container");
			}

			this.container = container;
		}

		public object CreateInstance(Type requestedType)
		{
			if (requestedType == null)
			{
				throw new ArgumentNullException("requestedType");
			}

			return Activator.CreateInstance(requestedType.GetConcreteType());
		}

		public void InitializeInstance(object obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}

			var scopedObject = obj as IBusinessScope;

			if (scopedObject != null)
			{
				var scope = this.container.BeginLifetimeScope();
				scopedObject.Scope = scope;

#if !NETFX_CORE && !WINDOWS_PHONE && !ANDROID && !IOS
				foreach (var property in
					(from _ in scopedObject.GetType().GetProperties(
							BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
					 where _.GetCustomAttribute<DependencyAttribute>() != null
					 select _))
				{
					property.SetValue(scopedObject, scope.Resolve(property.PropertyType));
				}
#else
				foreach (var property in
					(from _ in scopedObject.GetType().GetRuntimeProperties()
					 where !_.GetMethod.IsStatic &&
					 _.GetCustomAttribute<DependencyAttribute>() != null
					 select _))
				{
					property.SetValue(scopedObject, scope.Resolve(property.PropertyType));
				}
#endif
			}
		}

		public void FinalizeInstance(object obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}

			var scopedObject = obj as IBusinessScope;

			if (scopedObject != null)
			{
				scopedObject.Scope.Dispose();

#if !NETFX_CORE && !WINDOWS_PHONE
				foreach (var property in
					(from _ in scopedObject.GetType().GetProperties(
							BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
					 where _.GetCustomAttribute<DependencyAttribute>() != null
					 select _))
				{
					property.SetValue(scopedObject, null);
				}
#else
				foreach (var property in
					(from _ in scopedObject.GetType().GetRuntimeProperties()
					 where !_.GetMethod.IsStatic &&
					 _.GetCustomAttribute<DependencyAttribute>() != null
					 select _))
				{
					property.SetValue(scopedObject, null);
				}
#endif
			}
		}
	}
}
