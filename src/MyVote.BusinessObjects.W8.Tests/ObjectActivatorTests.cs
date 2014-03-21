using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Autofac;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MyVote.BusinessObjects.Attributes;
using MyVote.BusinessObjects.Core.Contracts;

namespace MyVote.BusinessObjects.W8.Tests
{
	[TestClass]
	public sealed class ObjectActivatorTests
	{
		[TestMethod]
		[SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "MyVote.BusinessObjects.ObjectActivator")]
		public void CreateWhenContainerIsNull()
		{
			Assert.ThrowsException<ArgumentNullException>(() => new ObjectActivator(null));
		}

		[TestMethod]
		public void CreateInstanceWhenRequestedTypeIsNull()
		{
			Assert.ThrowsException<ArgumentNullException>(
				() => new ObjectActivator(new ContainerBuilder().Build()).CreateInstance(null));
		}

		[TestMethod]
		public void CreateInstance()
		{
			Assert.IsTrue(new ObjectActivator(new ContainerBuilder().Build()).CreateInstance(typeof(Target)) is Target);
		}

		[TestMethod]
		public void InitializeInstanceWhenObjIsNull()
		{
			Assert.ThrowsException<ArgumentNullException>(
				() => new ObjectActivator(new ContainerBuilder().Build()).InitializeInstance(null));
		}

		[TestMethod]
		public void InitializeInstanceWhenObjIsNotScoped()
		{
			new ObjectActivator(new ContainerBuilder().Build()).InitializeInstance(new Target());
		}

		[TestMethod]
		public void InitializeInstanceWhenObjIsScoped()
		{
			var dependency = new ObjectDependency();

			var builder = new ContainerBuilder();
			builder.Register<IObjectDependency>(_ => dependency);

			var target = new DependentTarget();

			new ObjectActivator(builder.Build()).InitializeInstance(target);

			Assert.AreSame(dependency, target.ObjectDependency);
			Assert.IsNotNull(target.Scope);
		}

		[TestMethod]
		public void FinalizeInstanceWhenObjIsNull()
		{
			Assert.ThrowsException<ArgumentNullException>(
				() => new ObjectActivator(new ContainerBuilder().Build()).FinalizeInstance(null));
		}

		[TestMethod]
		public void FinalizeInstanceWhenObjIsNotScoped()
		{
			new ObjectActivator(new ContainerBuilder().Build()).FinalizeInstance(new Target());
		}

		[TestMethod]
		public void FinalizeInstanceWhenObjIsScoped()
		{
			var dependency = new ObjectDependency();

			var target = new DependentTarget { ObjectDependency = dependency, Scope = new LifetimeScope() };

			new ObjectActivator(new ContainerBuilder().Build()).FinalizeInstance(target);

			Assert.IsNull(target.ObjectDependency);
			Assert.IsNotNull(target.Scope);
		}
	}

	[SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces")]
	public interface IObjectDependency { }

	public class ObjectDependency : IObjectDependency { }

#pragma warning disable 67
	[SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
	public class LifetimeScope : ILifetimeScope
	{

		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
		public ILifetimeScope BeginLifetimeScope(object tag, Action<ContainerBuilder> configurationAction)
		{
			throw new NotImplementedException();
		}

		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
		public ILifetimeScope BeginLifetimeScope(Action<ContainerBuilder> configurationAction)
		{
			throw new NotImplementedException();
		}

		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
		public ILifetimeScope BeginLifetimeScope(object tag)
		{
			throw new NotImplementedException();
		}

		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
		public ILifetimeScope BeginLifetimeScope()
		{
			throw new NotImplementedException();
		}

		public event EventHandler<Autofac.Core.Lifetime.LifetimeScopeBeginningEventArgs> ChildLifetimeScopeBeginning;

		public event EventHandler<Autofac.Core.Lifetime.LifetimeScopeEndingEventArgs> CurrentScopeEnding;

		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
		public Autofac.Core.IDisposer Disposer
		{
			get { throw new NotImplementedException(); }
		}

		public event EventHandler<Autofac.Core.Resolving.ResolveOperationBeginningEventArgs> ResolveOperationBeginning;

		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
		public object Tag
		{
			get { throw new NotImplementedException(); }
		}

		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
		public Autofac.Core.IComponentRegistry ComponentRegistry
		{
			get { throw new NotImplementedException(); }
		}

		public object ResolveComponent(Autofac.Core.IComponentRegistration registration, IEnumerable<Autofac.Core.Parameter> parameters)
		{
			throw new NotImplementedException();
		}

		[SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
		[SuppressMessage("Microsoft.Usage", "CA1816:CallGCSuppressFinalizeCorrectly")]
		public void Dispose() { }
	}
#pragma warning restore 67

	public class Target { }

	public class DependentTarget
		: IBusinessScope
	{
		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		[Dependency]
		public IObjectDependency ObjectDependency { get; set; }
		public ILifetimeScope Scope { get; set; }
	}
}
