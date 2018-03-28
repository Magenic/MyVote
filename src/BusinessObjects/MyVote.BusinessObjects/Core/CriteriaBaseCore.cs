using Csla;

namespace MyVote.BusinessObjects.Core
{
	[System.Serializable]
	public abstract class CriteriaBaseCore<T>
		: CriteriaBase<T>
		where T : CriteriaBaseCore<T>
	{
		protected CriteriaBaseCore()
			: base() { }
	}
}
