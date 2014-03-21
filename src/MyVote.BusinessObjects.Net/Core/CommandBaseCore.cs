using Csla;
using MyVote.BusinessObjects.Core.Contracts;


#if !NETFX_CORE && !WINDOWS_PHONE

#endif

namespace MyVote.BusinessObjects.Core
{
#if (!NETFX_CORE && !WINDOWS_PHONE) || ANDROID
	[System.Serializable]
#else
	[Csla.Serialization.Serializable]
#endif
	internal abstract class CommandBaseCore<T>
		: CommandBase<T>, ICommandBaseCore
		where T : CommandBaseCore<T>
	{
		protected CommandBaseCore()
			: base() { }
	}
}
