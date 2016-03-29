using Autofac;
using System;

namespace MyVote.BusinessObjects
{
	internal sealed class ScopeCounter
	{
		private uint count = 1;
		private bool isScopeDisposed;

		public ScopeCounter(ILifetimeScope scope)
		{
			this.Scope = scope;
		}

		public void Add()
		{
			if(this.isScopeDisposed)
			{
				throw new ObjectDisposedException(nameof(this.Scope));
			}

			this.count++;
		}

		public void Release()
		{
			if (this.isScopeDisposed)
			{
				throw new ObjectDisposedException(nameof(this.Scope));
			}

			this.count--;

			if(this.count == 0)
			{
				this.Scope.Dispose();
				this.isScopeDisposed = true;
				this.Scope = null;
			}
		}

		public ILifetimeScope Scope { get; private set; }
	}
}
