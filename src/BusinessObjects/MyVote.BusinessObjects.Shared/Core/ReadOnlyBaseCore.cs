using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Csla;
using MyVote.BusinessObjects.Core.Contracts;

namespace MyVote.BusinessObjects.Core
{
#if (!NETFX_CORE && !WINDOWS_PHONE) || ANDROID || IOS
	[System.Serializable]
#else
	[Csla.Serialization.Serializable]
#endif
	internal abstract class ReadOnlyBaseCore<T>
		: ReadOnlyBase<T>, IReadOnlyBaseCore
		where T : ReadOnlyBaseCore<T>
	{
		protected ReadOnlyBaseCore()
			: base() { }

		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
		protected virtual void Child_Create()
		{
			throw new NotSupportedException(CoreMessages.ErrorMustOverrideDataPortalMethod);
		}

		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
		protected virtual void Child_Fetch()
		{
			throw new NotSupportedException(CoreMessages.ErrorMustOverrideDataPortalMethod);
		}

		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
		protected virtual void DataPortal_Create()
		{
			throw new NotSupportedException(CoreMessages.ErrorMustOverrideDataPortalMethod);
		}

		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
		protected virtual void DataPortal_Fetch()
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
