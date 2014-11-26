using System;
using Autofac;
using MyVote.BusinessObjects.Core.Contracts;
using MyVote.BusinessObjects.Attributes;

#if !NETFX_CORE && !WINDOWS_PHONE && !ANDROID && !IOS
using MyVote.Repository;
#endif

namespace MyVote.BusinessObjects.Core
{
#if (!NETFX_CORE && !WINDOWS_PHONE) || ANDROID || IOS
	[System.Serializable]
#else
	[Csla.Serialization.Serializable]
#endif
	internal abstract class CslaIdentityScopeCore<T>
		: CslaIdentityCore<T>, IBusinessScope
		where T : CslaIdentityScopeCore<T>
	{
		protected CslaIdentityScopeCore()
			: base() { }

#if !NETFX_CORE && !WINDOWS_PHONE && !ANDROID && !IOS
		[NonSerialized]
#endif
		private ILifetimeScope scope;
		[Dependency]
		ILifetimeScope IBusinessScope.Scope
		{
			get { return this.scope; }
			set { this.scope = value; }
		}

#if !NETFX_CORE && !WINDOWS_PHONE && !ANDROID && !IOS
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
