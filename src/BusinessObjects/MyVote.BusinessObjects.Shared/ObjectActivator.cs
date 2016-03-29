using Autofac;
using Csla.Reflection;
using Csla.Server;
using MyVote.BusinessObjects.Attributes;
using MyVote.BusinessObjects.Contracts;
using System;
using System.Linq;
using System.Reflection;

namespace MyVote.BusinessObjects
{
	public sealed class ObjectActivator
		: IDataPortalActivator
	{
		private const string CounterID = "deea6f50-f1e6-41b6-8440-632e0db98394";
		private readonly ICallContext context;
		private readonly IContainer container;

		public ObjectActivator(IContainer container, ICallContext context)
		{
			if (container == null)
			{
				throw new ArgumentNullException(nameof(container));
			}

			if (context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}

			this.container = container;
			this.context = context;
		}

		public object CreateInstance(Type requestedType)
		{
			if (requestedType == null)
			{
				throw new ArgumentNullException(nameof(requestedType));
			}

			return MethodCaller.CreateInstance(requestedType);
		}

		public void InitializeInstance(object obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException(nameof(obj));
			}

			var counter = this.context.GetData<ScopeCounter>(ObjectActivator.CounterID);

			if (counter == null)
			{
				counter = new ScopeCounter(this.container.BeginLifetimeScope());
				this.context.SetData(ObjectActivator.CounterID, counter);
			}
			else
			{
				counter.Add();
			}

#if !NETFX_CORE && !MOBILE
			foreach (var property in
				  (from _ in obj.GetType().GetProperties(
						  BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
					where _.GetCustomAttribute<DependencyAttribute>() != null
					select _))
			{
				property.SetValue(obj, counter.Scope.Resolve(property.PropertyType));
				}
#else
			foreach (var property in
				(from _ in obj.GetType().GetRuntimeProperties()
					where !_.GetMethod.IsStatic &&
					_.GetCustomAttribute<DependencyAttribute>() != null
					select _))
			{
				property.SetValue(obj, counter.Scope.Resolve(property.PropertyType));
			}
#endif
		}

		public void FinalizeInstance(object obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException(nameof(obj));
			}

			var counter = this.context.GetData<ScopeCounter>(ObjectActivator.CounterID);
			counter.Release();

			if (counter.Scope == null)
			{
				this.context.FreeNamedDataSlot(ObjectActivator.CounterID);
#if !NETFX_CORE
				foreach (var property in
					(from _ in obj.GetType().GetProperties(
							BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
					 where _.GetCustomAttribute<DependencyAttribute>() != null
					 select _))
				{
					property.SetValue(obj, null);
				}
#else
				foreach (var property in
					(from _ in obj.GetType().GetRuntimeProperties()
					 where !_.GetMethod.IsStatic &&
					 _.GetCustomAttribute<DependencyAttribute>() != null
					 select _))
				{
					property.SetValue(obj, null);
				}
#endif
			}
		}
	}
}
