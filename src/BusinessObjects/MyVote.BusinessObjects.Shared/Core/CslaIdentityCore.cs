using System;
using Csla.Security;
using MyVote.BusinessObjects.Core.Contracts;

namespace MyVote.BusinessObjects.Core
{
#if (!NETFX_CORE && !WINDOWS_PHONE) || ANDROID || IOS
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
