using Csla;
using MyVote.BusinessObjects.Core;

namespace MyVote.BusinessObjects
{
#if (!NETFX_CORE && !WINDOWS_PHONE) || ANDROID || IOS
	[System.Serializable]
#else
	[Csla.Serialization.Serializable]
#endif
	public sealed class PollSubmissionCriteria
		: CriteriaBaseCore<PollSubmissionCriteria>
	{
		public PollSubmissionCriteria()
			: base() { }

		public PollSubmissionCriteria(int pollId, int userId)
			: base()
		{
			this.PollID = pollId;
			this.UserID = userId;
		}

		public static PropertyInfo<int> PollIDProperty =
			PollSubmissionCriteria.RegisterProperty<int>(_ => _.PollID);
		public int PollID
		{
			get { return this.ReadProperty(PollSubmissionCriteria.PollIDProperty); }
			private set { this.LoadProperty(PollSubmissionCriteria.PollIDProperty, value); }
		}

		public static PropertyInfo<int> UserIDProperty =
			PollSubmissionCriteria.RegisterProperty<int>(_ => _.UserID);
		public int UserID
		{
			get { return this.ReadProperty(PollSubmissionCriteria.UserIDProperty); }
			private set { this.LoadProperty(PollSubmissionCriteria.UserIDProperty, value); }
		}
	}
}
