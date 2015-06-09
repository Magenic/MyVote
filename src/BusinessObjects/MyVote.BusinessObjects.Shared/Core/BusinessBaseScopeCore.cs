using System;
using MyVote.BusinessObjects.Core.Contracts;
using MyVote.BusinessObjects.Attributes;
using Autofac;

#if !NETFX_CORE && !WINDOWS_PHONE && !ANDROID && !IOS
using MyVote.Data.Entities;
using MyVote.Core.Extensions;
using System.Collections.Generic;
#endif

namespace MyVote.BusinessObjects.Core
{
#if (!NETFX_CORE && !WINDOWS_PHONE) || ANDROID || IOS
	[System.Serializable]
#else
	[Csla.Serialization.Serializable]
#endif
	internal abstract class BusinessBaseScopeCore<T>
		: BusinessBaseCore<T>, IBusinessScope
		where T : BusinessBaseScopeCore<T>
	{
		protected BusinessBaseScopeCore()
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
