using Csla;

namespace MyVote.BusinessObjects.Core
{
#if (!NETFX_CORE && !WINDOWS_PHONE) || ANDROID || IOS
	[System.Serializable]
#else
	[Csla.Serialization.Serializable]
#endif
	public abstract class CriteriaBaseCore<T>
		: CriteriaBase<T>
		where T : CriteriaBaseCore<T>
	{
		protected CriteriaBaseCore()
			: base() { }
	}
}
