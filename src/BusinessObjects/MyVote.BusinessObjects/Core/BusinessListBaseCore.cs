using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Csla;
using Csla.Core;
using MyVote.BusinessObjects.Core.Contracts;


#if !MOBILE
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
#if !MOBILE
		[NonSerialized]
		private IEntitiesContext entities;
#endif

		protected BusinessListBaseCore() : base() { }

		protected override C AddNewCore()
		{
			return base.AddNewCore();
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

		protected virtual void DataPortal_Fetch()
		{
			throw new NotSupportedException(CoreMessages.ErrorMustOverrideDataPortalMethod);
		}

#if !MOBILE
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
