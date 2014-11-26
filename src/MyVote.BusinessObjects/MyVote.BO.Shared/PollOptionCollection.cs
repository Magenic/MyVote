using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;

namespace MyVote.BusinessObjects
{
#if (!NETFX_CORE && !WINDOWS_PHONE) || ANDROID || IOS
    [System.Serializable]
#else
	[Csla.Serialization.Serializable]
#endif
	internal sealed class PollOptionCollection
		: BusinessListBaseCore<PollOptionCollection, IPollOption>, IPollOptionCollection { }
}
