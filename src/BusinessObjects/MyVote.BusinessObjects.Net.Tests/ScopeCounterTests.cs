using Autofac;
using FluentAssertions;
using Moq;
using System;
using Xunit;

namespace MyVote.BusinessObjects.Net.Tests
{
	public sealed class ScopeCounterTests
	{
		[Fact]
		public void Create()
		{
			var scope = Mock.Of<ILifetimeScope>();
			var counter = new ScopeCounter(scope);

			counter.Scope.Should().BeSameAs(scope);
		}

		[Fact]
		public void AddAndRelease()
		{
			var scope = new Mock<ILifetimeScope>(MockBehavior.Strict);
			scope.Setup(_ => _.Dispose());

			var counter = new ScopeCounter(scope.Object);
			counter.Add();
			counter.Add();
			counter.Release();
			counter.Release();
			counter.Release();

			counter.Scope.Should().BeNull();
			scope.VerifyAll();
		}

		[Fact]
		public void AddAfterDisposed()
		{
			var counter = new ScopeCounter(Mock.Of<ILifetimeScope>());
			counter.Release();
			new Action(() => counter.Add()).ShouldThrow<ObjectDisposedException>();
		}

		[Fact]
		public void ReleaseAfterDisposed()
		{
			var counter = new ScopeCounter(Mock.Of<ILifetimeScope>());
			counter.Release();
			new Action(() => counter.Release()).ShouldThrow<ObjectDisposedException>();
		}
	}
}
