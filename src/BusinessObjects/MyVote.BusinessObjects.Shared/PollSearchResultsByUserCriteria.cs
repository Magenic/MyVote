using Csla;
using MyVote.BusinessObjects.Core;

namespace MyVote.BusinessObjects
{
	 [System.Serializable]
	public sealed class PollSearchResultsByUserCriteria
		: CriteriaBaseCore<PollSearchResultsByUserCriteria>
	{
		public PollSearchResultsByUserCriteria(int userId, bool arePollsActive)
			: base()
		{
			this.UserID = userId;
			this.ArePollsActive = arePollsActive;
		}

		public static PropertyInfo<bool> ArePollsActiveProperty =
			PollSearchResultsByUserCriteria.RegisterProperty<bool>(_ => _.ArePollsActive);
		public bool ArePollsActive
		{
			get { return this.ReadProperty(PollSearchResultsByUserCriteria.ArePollsActiveProperty); }
			private set { this.LoadProperty(PollSearchResultsByUserCriteria.ArePollsActiveProperty, value); }
		}

		public static PropertyInfo<int> UserIDProperty =
			PollSearchResultsByUserCriteria.RegisterProperty<int>(_ => _.UserID);
		public int UserID
		{
			get { return this.ReadProperty(PollSearchResultsByUserCriteria.UserIDProperty); }
			private set { this.LoadProperty(PollSearchResultsByUserCriteria.UserIDProperty, value); }
		}
	}
}
