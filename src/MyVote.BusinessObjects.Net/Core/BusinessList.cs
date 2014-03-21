using Csla;
using Csla.Core;

namespace MyVote.BusinessObjects.Core
{
#if (!NETFX_CORE && !WINDOWS_PHONE) || ANDROID
	[System.Serializable]
#else
	[Csla.Serialization.Serializable]
#endif
	public sealed class BusinessList<C>
		: BusinessListBase<BusinessList<C>, C>
		where C : IEditableBusinessObject
	{
		private void Child_Fetch() { }
	}
}
