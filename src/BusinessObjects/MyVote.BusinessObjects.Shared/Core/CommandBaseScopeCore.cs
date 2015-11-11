using System;
using MyVote.BusinessObjects.Core.Contracts;
using Autofac;
using MyVote.BusinessObjects.Attributes;

#if !NETFX_CORE && !MOBILE
using MyVote.Data.Entities;
#endif

namespace MyVote.BusinessObjects.Core
{
	[System.Serializable]
	internal abstract class CommandBaseScopeCore<T>
		: CommandBaseCore<T>, IBusinessScope
		where T : CommandBaseScopeCore<T>
	{
		protected CommandBaseScopeCore()
			: base() { }

#if !NETFX_CORE && !MOBILE
        [NonSerialized]
#endif
		private ILifetimeScope scope;
		[Dependency]
		ILifetimeScope IBusinessScope.Scope
		{
			get { return this.scope; }
			set { this.scope = value; }
		}

#if !NETFX_CORE && !MOBILE
        [NonSerialized]
		private IEntities entities;
		[Dependency]
		public IEntities Entities
		{
			get { return this.entities; }
			set { this.entities = value; }
		}
#endif
	}
}
