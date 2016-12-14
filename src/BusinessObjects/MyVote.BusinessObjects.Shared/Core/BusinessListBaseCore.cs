using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Csla;
using Csla.Core;
using MyVote.BusinessObjects.Core.Contracts;


#if !NETFX_CORE && !MOBILE
using MyVote.Data.Entities;
#endif

namespace MyVote.BusinessObjects.Core
{
	[Serializable]
	internal abstract class BusinessListBaseCore<T, C>
		: BusinessListBase<T, C>, IBusinessListBaseCore<C>
		where T : BusinessListBaseCore<T, C>
		where C : IEditableBusinessObject
	{
#if !NETFX_CORE && !MOBILE
		[NonSerialized]
		private IEntitiesContext entities;
#endif

		protected BusinessListBaseCore() : base() { }

		protected override void AddNewCore()
		{
			base.AddNewCore();
		}

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

#if !NETFX_CORE && !MOBILE
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
		protected virtual List<string> IgnoredProperties
		{
			get { return new List<string> { nameof(this.Entities) }; }
		}

		public IEntitiesContext Entities
		{
			get { return this.entities; }
			set { this.entities = value; }
		}
#endif
	}
}
