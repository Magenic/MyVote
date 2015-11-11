using System;
using System.Diagnostics.CodeAnalysis;
using Csla.Core;

namespace MyVote.BusinessObjects.Core
{
	[SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
	[System.Serializable]
	internal sealed class ReadOnlySwitchList<C>
		: ReadOnlyListBaseCore<ReadOnlySwitchList<C>, C>
		where C : IReadOnlyObject
	{
		protected override void Child_Fetch() { }

		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "SwitchReadOnlyStatus")]
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
