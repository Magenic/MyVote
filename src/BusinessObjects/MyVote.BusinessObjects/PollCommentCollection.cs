using System.Diagnostics.CodeAnalysis;
using Csla;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;

namespace MyVote.BusinessObjects
{
	[SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
	[System.Serializable]
	internal sealed class PollCommentCollection
		: BusinessListBaseCore<PollCommentCollection, IPollComment>, IPollCommentCollection
	{
		[RunLocal]
		protected override void Child_Create() { }
#if !MOBILE
		protected override void Child_Fetch() { }
#endif
	}
}
