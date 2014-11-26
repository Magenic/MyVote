using System.Security.Principal;
using Csla.Security;
using MyVote.BusinessObjects.Core.Contracts;

namespace MyVote.BusinessObjects.Core
{
#if (!NETFX_CORE && !WINDOWS_PHONE) || ANDROID || IOS
	 [System.Serializable]
#else
	[Csla.Serialization.Serializable]
#endif
	public class CslaPrincipalCore
		: CslaPrincipal, ICslaPrincipalCore
	{
		public CslaPrincipalCore()
			: base() { }

		public CslaPrincipalCore(IIdentity identity)
			: base(identity) { }
	}
}
