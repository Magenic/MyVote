using System.Security.Principal;
using Csla.Security;
using MyVote.BusinessObjects.Core.Contracts;

namespace MyVote.BusinessObjects.Core
{
	 [System.Serializable]
	public class CslaPrincipalCore
		: CslaPrincipal, ICslaPrincipalCore
	{
		public CslaPrincipalCore()
			: base() { }

		public CslaPrincipalCore(IIdentity identity)
			: base(identity) { }
	}
}
