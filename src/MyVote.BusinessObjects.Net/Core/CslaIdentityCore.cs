using System;
using Csla.Security;
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
	internal abstract class CslaIdentityCore<T>
		: CslaIdentityBase<T>, ICslaIdentityCore
		where T : CslaIdentityBase<T>
	{
		protected CslaIdentityCore()
			: base() { }
	}
}
