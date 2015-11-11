using System;
using MyVote.BusinessObjects.Core.Contracts;
using MyVote.BusinessObjects.Attributes;
using Autofac;

#if !NETFX_CORE && !MOBILE
using MyVote.Data.Entities;
using MyVote.Core.Extensions;
using System.Collections.Generic;
#endif

namespace MyVote.BusinessObjects.Core
{
	[System.Serializable]
	internal abstract class BusinessBaseScopeCore<T>
		: BusinessBaseCore<T>, IBusinessScope
		where T : BusinessBaseScopeCore<T>
	{
		protected BusinessBaseScopeCore()
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

		protected override List<string> IgnoredProperties
		{
			get
			{
				var properties = base.IgnoredProperties;
				properties.Add(this.GetPropertyName(_ => _.Entities));
				return properties;
			}
		}
#endif
	}
}
