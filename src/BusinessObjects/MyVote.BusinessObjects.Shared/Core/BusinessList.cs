using Csla;
using Csla.Core;

namespace MyVote.BusinessObjects.Core
{
	[System.Serializable]
	public sealed class BusinessList<C>
		: BusinessListBase<BusinessList<C>, C>
		where C : IEditableBusinessObject
	{
		private void Child_Fetch() { }
	}
}
