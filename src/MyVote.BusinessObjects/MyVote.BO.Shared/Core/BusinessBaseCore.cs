using System;
using Csla;
using System.Diagnostics.CodeAnalysis;

#if !NETFX_CORE && !WINDOWS_PHONE && !ANDROID && !IOS
using System.Collections.Generic;
#endif

namespace MyVote.BusinessObjects.Core
{
#if (!NETFX_CORE && !WINDOWS_PHONE) || ANDROID || IOS
	[System.Serializable]
#else
	[Csla.Serialization.Serializable]
#endif
	internal abstract class BusinessBaseCore<T>
		: BusinessBase<T>, IBusinessBase
		where T : BusinessBaseCore<T>
	{
		protected BusinessBaseCore()
			: base() { }

		protected override void Child_Create()
		{
			throw new NotSupportedException(CoreMessages.ErrorMustOverrideDataPortalMethod);
		}

		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
		protected virtual void Child_Fetch()
		{
			throw new NotSupportedException(CoreMessages.ErrorMustOverrideDataPortalMethod);
		}

		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
		protected virtual void Child_Insert()
		{
			throw new NotSupportedException(CoreMessages.ErrorMustOverrideDataPortalMethod);
		}

		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
		protected virtual void Child_Update()
		{
			throw new NotSupportedException(CoreMessages.ErrorMustOverrideDataPortalMethod);
		}

		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
		protected virtual void Child_DeleteSelf()
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

		protected override void DataPortal_Insert()
		{
			throw new NotSupportedException(CoreMessages.ErrorMustOverrideDataPortalMethod);
		}

		protected override void DataPortal_Update()
		{
			throw new NotSupportedException(CoreMessages.ErrorMustOverrideDataPortalMethod);
		}

		protected override void DataPortal_DeleteSelf()
		{
			throw new NotSupportedException(CoreMessages.ErrorMustOverrideDataPortalMethod);
		}

#if !NETFX_CORE && !WINDOWS_PHONE && !ANDROID && !IOS
		protected virtual List<string> IgnoredProperties
		{
			get { return new List<string>(); }
		}
#endif
	}
}
