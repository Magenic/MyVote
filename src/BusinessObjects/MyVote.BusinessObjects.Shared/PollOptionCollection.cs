using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;

namespace MyVote.BusinessObjects
{
	[System.Serializable]
	internal sealed class PollOptionCollection
	  : BusinessListBaseCore<PollOptionCollection, IPollOption>, IPollOptionCollection
	{ }
}
