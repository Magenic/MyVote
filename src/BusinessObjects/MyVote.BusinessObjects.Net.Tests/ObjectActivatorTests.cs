using Autofac;
using FluentAssertions;
using Moq;
using MyVote.BusinessObjects.Attributes;
using MyVote.BusinessObjects.Contracts;
using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace MyVote.BusinessObjects.Net.Tests
{
	public sealed class ObjectActivatorTests
	{
		[Fact]
		public void CreateWhenContainerIsNull()
		{
			new Action(() => new ObjectActivator(null, Mock.Of<ICallContext>())).ShouldThrow<ArgumentNullException>();
		}

		[Fact]
		public void CreateWhenContextIsNull()
		{
			new Action(() => new ObjectActivator(Mock.Of<IContainer>(), null)).ShouldThrow<ArgumentNullException>();
		}

		[Fact]
		public void CreateInstanceWhenRequestedTypeIsNull()
		{
			new Action(() => new ObjectActivator(Mock.Of<IContainer>(), Mock.Of<ICallContext>()).CreateInstance(null)).ShouldThrow<ArgumentNullException>();
		}

		[Fact]
		public void CreateInstance()
		{
			(new ObjectActivator(Mock.Of<IContainer>(), Mock.Of<ICallContext>())
				.CreateInstance(typeof(Target)) is Target).Should().BeTrue();
		}

		[Fact]
		public void InitializeInstanceWhenObjIsNull()
		{
			new Action(() => new ObjectActivator(Mock.Of<IContainer>(), Mock.Of<ICallContext>())
				.InitializeInstance(null)).ShouldThrow<ArgumentNullException>();
		}

		[Fact]
		public void InitializeInstanceWhenObjIsNotScoped()
		{
			new Action(() => new ObjectActivator(Mock.Of<IContainer>(), Mock.Of<ICallContext>())
				.InitializeInstance(new Target())).ShouldNotThrow();
		}

		[Fact]
		public void InitializeInstanceWhenObjIsScoped()
		{
			var list = Mock.Of<IList>();
			var builder = new ContainerBuilder();
			builder.RegisterInstance(list).As<IList>();

			var target = new DependentTarget();

			var activator = new ObjectActivator(builder.Build(), new ActivatorCallContext());
			activator.InitializeInstance(target);

			target.List.Should().BeSameAs(list);
		}

		[Fact]
		public void FinalizeInstanceWhenObjIsNull()
		{
			var target = new Target();
			var activator = new ObjectActivator(
				new ContainerBuilder().Build(), new ActivatorCallContext());
			activator.InitializeInstance(target);
			new Action(() => activator.FinalizeInstance(null)).ShouldThrow<ArgumentNullException>();
		}

		[Fact]
		public void FinalizeInstanceWhenObjIsNotScoped()
		{
			new Action(() =>
			{
				var target = new Target();
				var activator = new ObjectActivator(
					new ContainerBuilder().Build(), new ActivatorCallContext());
				activator.InitializeInstance(target);
				activator.FinalizeInstance(target);
			}).ShouldNotThrow();
		}

		[Fact]
		public void FinalizeInstanceWhenObjIsScoped()
		{
			var list = Mock.Of<IList>();

			var builder = new ContainerBuilder();
			builder.RegisterInstance(list).As<IList>();

			var target = new DependentTarget { List = list };

			var activator = new ObjectActivator(builder.Build(), new ActivatorCallContext());
			activator.InitializeInstance(target);
			activator.FinalizeInstance(target);

			target.List.Should().BeNull();
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
