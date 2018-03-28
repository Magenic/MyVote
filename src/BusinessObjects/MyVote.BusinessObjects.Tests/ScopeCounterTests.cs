using Autofac;
using FluentAssertions;
using Rocks;
using System;
using Xunit;

namespace MyVote.BusinessObjects.Tests
{
	public sealed class ScopeCounterTests
	{
		[Fact]
		public void Create()
		{
			var scope = Rock.Make<ILifetimeScope>();
			var counter = new ScopeCounter(scope);

			counter.Scope.Should().BeSameAs(scope);
		}

		[Fact]
		public void AddAndRelease()
		{
			var scope = Rock.Create<ILifetimeScope>();
			scope.Handle(_ => _.Dispose());
			var scopeChunk = scope.Make();

			var counter = new ScopeCounter(scopeChunk);
			counter.Add();
			counter.Add();
			counter.Release();
			counter.Release();
			counter.Release();

			counter.Scope.Should().BeNull();
			scope.Verify();
		}

		[Fact]
		public void AddAfterDisposed()
		{
			var counter = new ScopeCounter(Rock.Make<ILifetimeScope>());
			counter.Release();
			new Action(() => counter.Add()).ShouldThrow<ObjectDisposedException>();
		}

		[Fact]
		public void ReleaseAfterDisposed()
		{
			var counter = new ScopeCounter(Rock.Make<ILifetimeScope>());
			counter.Release();
			new Action(() => counter.Release()).ShouldThrow<ObjectDisposedException>();
		}
	}
}
