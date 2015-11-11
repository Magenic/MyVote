using Csla;
using MyVote.BusinessObjects.Core.Contracts;

namespace MyVote.BusinessObjects.Core
{
	[System.Serializable]
	internal abstract class CommandBaseCore<T>
		: CommandBase<T>, ICommandBaseCore
		where T : CommandBaseCore<T>
	{
		protected CommandBaseCore()
			: base() { }
	}
}
