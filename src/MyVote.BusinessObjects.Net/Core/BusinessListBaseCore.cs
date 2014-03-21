using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Csla;
using Csla.Core;
using MyVote.BusinessObjects.Core.Contracts;


#if !NETFX_CORE && !WINDOWS_PHONE
using MyVote.Repository;
using MyVote.Core.Extensions;
#endif

namespace MyVote.BusinessObjects.Core
{
#if (!NETFX_CORE && !WINDOWS_PHONE) || ANDROID
	[System.Serializable]
#else
	[Csla.Serialization.Serializable]
#endif
	internal abstract class BusinessListBaseCore<T, C>
		: BusinessListBase<T, C>, IBusinessListBaseCore<C>
		where T : BusinessListBaseCore<T, C>
		where C : IEditableBusinessObject
	{
#if !NETFX_CORE && !WINDOWS_PHONE
		[NonSerialized]
		private IEntities entities;
#endif

		protected BusinessListBaseCore() : base() { }

#if !NETFX_CORE && !WINDOWS_PHONE
		protected override C AddNewCore()
		{
			return base.AddNewCore();
		}
#else
		protected override void AddNewCore()
		{
			base.AddNewCore();
		}
#endif

		protected override void Child_Create()
		{
			throw new NotSupportedException(CoreMessages.ErrorMustOverrideDataPortalMethod);
		}

		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
		protected virtual void Child_Fetch()
		{
			throw new NotSupportedException(CoreMessages.ErrorMustOverrideDataPortalMethod);
		}

		protected override void DataPortal_Create()
		{
			throw new NotSupportedException(CoreMessages.ErrorMustOverrideDataPortalMethod);
		}

		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
		protected virtual void DataPortal_Fetch()
		{
			throw new NotSupportedException(CoreMessages.ErrorMustOverrideDataPortalMethod);
		}

#if !NETFX_CORE && !WINDOWS_PHONE
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
		protected virtual List<string> IgnoredProperties
		{
			get { return new List<string> { this.GetPropertyName(_ => _.Entities) }; }
		}

		public IEntities Entities
		{
			get { return this.entities; }
			set { this.entities = value; }
		}
#endif
	}
}
