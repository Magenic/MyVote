using Csla.Security;
using MyVote.BusinessObjects.Core.Contracts;

namespace MyVote.BusinessObjects.Core
{
    [System.Serializable]
	internal abstract class CslaIdentityCore<T>
		: CslaIdentityBase<T>, ICslaIdentityCore
		where T : CslaIdentityBase<T>
	{
		protected CslaIdentityCore()
			: base() { }
	}
}
