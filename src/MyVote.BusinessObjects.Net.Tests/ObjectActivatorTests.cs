using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Autofac;
using Autofac.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyVote.BusinessObjects.Attributes;
using MyVote.BusinessObjects.Core.Contracts;

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
			new ObjectActivator(null);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void CreateInstanceWhenRequestedTypeIsNull()
		{
			new ObjectActivator(Mock.Of<IContainer>()).CreateInstance(null);
		}

		[TestMethod]
		public void CreateInstance()
		{
			Assert.IsTrue(new ObjectActivator(Mock.Of<IContainer>()).CreateInstance(typeof(Target)) is Target);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void InitializeInstanceWhenObjIsNull()
		{
			new ObjectActivator(Mock.Of<IContainer>()).InitializeInstance(null);
		}

		[TestMethod]
		public void InitializeInstanceWhenObjIsNotScoped()
		{
			new ObjectActivator(Mock.Of<IContainer>()).InitializeInstance(new Target());
		}

		[TestMethod]
		public void InitializeInstanceWhenObjIsScoped()
		{
			var list = Mock.Of<IList>();

			IComponentRegistration registration = null;

			var registry = new Mock<IComponentRegistry>(MockBehavior.Strict);
			registry.Setup(_ => _.TryGetRegistration(It.IsAny<Service>(), out registration)).Returns(true);

			var scope = new Mock<ILifetimeScope>(MockBehavior.Strict);
			scope.SetupGet(_ => _.ComponentRegistry).Returns(registry.Object);
			scope.Setup(_ => _.ResolveComponent(registration, It.IsAny<IEnumerable<Parameter>>())).Returns(list);

			var container = new Mock<IContainer>(MockBehavior.Strict);
			container.Setup(_ => _.BeginLifetimeScope()).Returns(scope.Object);

			var target = new DependentTarget();

			new ObjectActivator(container.Object).InitializeInstance(target);

			Assert.AreSame(list, target.List);
			Assert.AreSame(scope.Object, target.Scope);

			container.VerifyAll();
			scope.VerifyAll();
			registry.VerifyAll();
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void FinalizeInstanceWhenObjIsNull()
		{
			new ObjectActivator(Mock.Of<IContainer>()).FinalizeInstance(null);
		}

		[TestMethod]
		public void FinalizeInstanceWhenObjIsNotScoped()
		{
			new ObjectActivator(Mock.Of<IContainer>()).FinalizeInstance(new Target());
		}

		[TestMethod]
		public void FinalizeInstanceWhenObjIsScoped()
		{
			var list = Mock.Of<IList>();
			var scope = new Mock<ILifetimeScope>(MockBehavior.Strict);
			scope.Setup(_ => _.Dispose());

			var target = new DependentTarget { List = list, Scope = scope.Object };

			new ObjectActivator(Mock.Of<IContainer>()).FinalizeInstance(target);

			Assert.IsNull(target.List);
			Assert.IsNotNull(target.Scope);

			scope.VerifyAll();
		}
	}

	public class Target { }

	public class DependentTarget
		: IBusinessScope
	{
		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		[Dependency]
		public IList List { get; set; }
		public ILifetimeScope Scope { get; set; }
	}
}
