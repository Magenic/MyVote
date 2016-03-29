using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyVote.BusinessObjects.Attributes;
using MyVote.BusinessObjects.Contracts;
using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace MyVote.BusinessObjects.Net.Tests
{
	[TestClass]
	public sealed class ObjectActivatorTests
	{
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		[SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "MyVote.BusinessObjects.ObjectActivator")]
		public void CreateWhenContainerIsNull()
		{
			new ObjectActivator(null, Mock.Of<ICallContext>());
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void CreateWhenContextIsNull()
		{
			new ObjectActivator(Mock.Of<IContainer>(), null);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void CreateInstanceWhenRequestedTypeIsNull()
		{
			new ObjectActivator(Mock.Of<IContainer>(), Mock.Of<ICallContext>()).CreateInstance(null);
		}

		[TestMethod]
		public void CreateInstance()
		{
			Assert.IsTrue(new ObjectActivator(Mock.Of<IContainer>(), Mock.Of<ICallContext>())
				.CreateInstance(typeof(Target)) is Target);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void InitializeInstanceWhenObjIsNull()
		{
			new ObjectActivator(Mock.Of<IContainer>(), Mock.Of<ICallContext>())
				.InitializeInstance(null);
		}

		[TestMethod]
		public void InitializeInstanceWhenObjIsNotScoped()
		{
			new ObjectActivator(Mock.Of<IContainer>(), Mock.Of<ICallContext>())
				.InitializeInstance(new Target());
		}

		[TestMethod]
		public void InitializeInstanceWhenObjIsScoped()
		{
			var list = Mock.Of<IList>();
			var builder = new ContainerBuilder();
			builder.RegisterInstance(list).As<IList>();

			var target = new DependentTarget();

			var activator = new ObjectActivator(builder.Build(), new ActivatorCallContext());
			activator.InitializeInstance(target);

			Assert.AreSame(list, target.List);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void FinalizeInstanceWhenObjIsNull()
		{
			var target = new Target();
			var activator = new ObjectActivator(
				new ContainerBuilder().Build(), new ActivatorCallContext());
			activator.InitializeInstance(target);
			activator.FinalizeInstance(null);
		}

		[TestMethod]
		public void FinalizeInstanceWhenObjIsNotScoped()
		{
			var target = new Target();
			var activator = new ObjectActivator(
				new ContainerBuilder().Build(), new ActivatorCallContext());
			activator.InitializeInstance(target);
			activator.FinalizeInstance(target);
		}

		[TestMethod]
		public void FinalizeInstanceWhenObjIsScoped()
		{
			var list = Mock.Of<IList>();

			var builder = new ContainerBuilder();
			builder.RegisterInstance(list).As<IList>();

			var target = new DependentTarget { List = list };

			var activator = new ObjectActivator(builder.Build(), new ActivatorCallContext());
			activator.InitializeInstance(target);
			activator.FinalizeInstance(target);

			Assert.IsNull(target.List);
		}
	}

	public class Target { }

	public class DependentTarget
	{
		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		[Dependency]
		public IList List { get; set; }
	}
}
