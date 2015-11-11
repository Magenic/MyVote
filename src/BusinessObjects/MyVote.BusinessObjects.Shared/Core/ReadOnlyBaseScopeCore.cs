using System;
using System.Collections.Generic;
using MyVote.BusinessObjects.Core.Contracts;
using MyVote.Core.Extensions;
using Autofac;
using MyVote.BusinessObjects.Attributes;

#if!NETFX_CORE && !MOBILE
using MyVote.Data.Entities;
#endif

namespace MyVote.BusinessObjects.Core
{
	[System.Serializable]
	internal abstract class ReadOnlyBaseScopeCore<T>
		: ReadOnlyBaseCore<T>, IBusinessScope
		where T : ReadOnlyBaseScopeCore<T>
	{
		protected ReadOnlyBaseScopeCore()
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
        protected override List<string> IgnoredProperties
		{
			get
			{
				var properties = base.IgnoredProperties;
				properties.Add(this.GetPropertyName(_ => _.Entities));
				return properties;
			}
		}

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
