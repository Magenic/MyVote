using System.Diagnostics.CodeAnalysis;
using Csla;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;

namespace MyVote.BusinessObjects
{
	[SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
#if (!NETFX_CORE && !WINDOWS_PHONE) || ANDROID || IOS
	[System.Serializable]
#else
	[Csla.Serialization.Serializable]
#endif
	internal sealed class PollCommentCollection
		: BusinessListBaseCore<PollCommentCollection, IPollComment>, IPollCommentCollection
	{
		[RunLocal]
		protected override void Child_Create() { }
#if !NETFX_CORE && !WINDOWS_PHONE && !ANDROID && !IOS
		protected override void Child_Fetch() { }
#endif
	}
}
