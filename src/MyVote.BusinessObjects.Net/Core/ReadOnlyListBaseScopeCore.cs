using System;
using Autofac;
using MyVote.BusinessObjects.Attributes;
using MyVote.BusinessObjects.Core.Contracts;


#if !NETFX_CORE && !WINDOWS_PHONE
using MyVote.Repository;
#endif

namespace MyVote.BusinessObjects.Core
{
#if (!NETFX_CORE && !WINDOWS_PHONE) || ANDROID
	[System.Serializable]
#else
	[Csla.Serialization.Serializable]
#endif
	internal abstract class ReadOnlyListBaseScopeCore<T, C>
		: ReadOnlyListBaseCore<T, C>, IBusinessScope
		where T : ReadOnlyListBaseScopeCore<T, C>
	{
		protected ReadOnlyListBaseScopeCore()
			: base() { }

#if !NETFX_CORE && !WINDOWS_PHONE
		[NonSerialized]
#endif
		private ILifetimeScope scope;
		[Dependency]
		ILifetimeScope IBusinessScope.Scope
		{
			get { return this.scope; }
			set { this.scope = value; }
		}

#if !NETFX_CORE && !WINDOWS_PHONE
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
