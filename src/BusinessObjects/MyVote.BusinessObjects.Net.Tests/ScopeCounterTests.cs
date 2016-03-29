using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace MyVote.BusinessObjects.Net.Tests
{
	[TestClass]
	public sealed class ScopeCounterTests
	{
		[TestMethod]
		public void Create()
		{
			var scope = Mock.Of<ILifetimeScope>();
			var counter = new ScopeCounter(scope);

			Assert.AreSame(scope, counter.Scope);
		}

		[TestMethod]
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

			Assert.IsNull(counter.Scope);
			scope.VerifyAll();
		}

		[TestMethod]
		[ExpectedException(typeof(ObjectDisposedException))]
		public void AddAfterDisposed()
		{
			var counter = new ScopeCounter(Mock.Of<ILifetimeScope>());
			counter.Release();
			counter.Add();
		}

		[TestMethod]
		[ExpectedException(typeof(ObjectDisposedException))]
		public void ReleaseAfterDisposed()
		{
			var counter = new ScopeCounter(Mock.Of<ILifetimeScope>());
			counter.Release();
			counter.Release();
		}
	}
}
