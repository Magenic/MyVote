using System;
using Csla.Core;

namespace MyVote.BusinessObjects.Core
{
	[Serializable]
	public sealed class ReadOnlySwitchList<C>
		: ReadOnlyListBaseCore<ReadOnlySwitchList<C>, C>
		where C : IReadOnlyObject
	{
		protected override void Child_Fetch() { }

		internal void SwitchReadOnlyStatus()
		{
			if (this.switchCount > 1)
			{
				throw new NotSupportedException("The SwitchReadOnlyStatus() method can only be called twice.");
			}

			this.switchCount++;
			this.IsReadOnly = !this.IsReadOnly;
		}

		private int switchCount;
	}
}
