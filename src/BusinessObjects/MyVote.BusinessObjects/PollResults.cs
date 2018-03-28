using System;
using Csla;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;
using System.Linq;

#if !MOBILE
using MyVote.BusinessObjects.Attributes;
#endif

namespace MyVote.BusinessObjects
{
	[Serializable]
	internal sealed class PollResults
		: BusinessBaseCore<PollResults>, IPollResults
	{
#if !MOBILE
		private void DataPortal_Fetch(PollResultsCriteria criteria)
		{
			using (this.BypassPropertyChecks)
			{
				this.PollID = criteria.PollID;
				this.PollDataResults = this.PollDataResultsFactory.FetchChild(criteria.PollID);
				this.PollComments = this.PollCommentsFactory.FetchChild(criteria.PollID);

				var pollData = (from p in this.Entities.Mvpoll
									 where p.PollId == criteria.PollID
									 select new
									 {
										 p.UserId,
										 IsDeleted = p.PollDeletedFlag != (bool?)false,
										 p.PollStartDate,
										 p.PollEndDate,
										 p.PollImageLink
									 }).Single();

				this.IsActive = !pollData.IsDeleted && pollData.PollStartDate < DateTime.UtcNow && pollData.PollEndDate > DateTime.UtcNow;
				this.PollImageLink = pollData.PollImageLink;

				if (criteria.UserID != null)
				{
					this.IsPollOwnedByUser = pollData.UserId == criteria.UserID.Value;
				}
			}
		}

		protected override void DataPortal_Update()
		{
			this.FieldManager.UpdateChildren();
		}
#endif

		public static readonly PropertyInfo<int> PollIDProperty =
		 PollResults.RegisterProperty<int>(_ => _.PollID);
		public int PollID
		{
			get { return this.ReadProperty(PollResults.PollIDProperty); }
			private set { this.LoadProperty(PollResults.PollIDProperty, value); }
		}

		public static readonly PropertyInfo<bool> IsActiveProperty =
			PollResults.RegisterProperty<bool>(_ => _.IsActive);
		public bool IsActive
		{
			get { return this.ReadProperty(PollResults.IsActiveProperty); }
			private set { this.LoadProperty(PollResults.IsActiveProperty, value); }
		}

		public static readonly PropertyInfo<bool> IsPollOwnedByUserProperty =
			PollResults.RegisterProperty<bool>(_ => _.IsPollOwnedByUser);
		public bool IsPollOwnedByUser
		{
			get { return this.ReadProperty(PollResults.IsPollOwnedByUserProperty); }
			private set { this.LoadProperty(PollResults.IsPollOwnedByUserProperty, value); }
		}

		public static readonly PropertyInfo<IPollDataResults> PollDataResultsProperty =
			PollResults.RegisterProperty<IPollDataResults>(_ => _.PollDataResults);
		public IPollDataResults PollDataResults
		{
			get { return this.ReadProperty(PollResults.PollDataResultsProperty); }
			private set { this.LoadProperty(PollResults.PollDataResultsProperty, value); }
		}

		public static readonly PropertyInfo<string> PollImageLinkProperty =
			PollResults.RegisterProperty<string>(_ => _.PollImageLink);
		public string PollImageLink
		{
			get { return this.ReadProperty(PollResults.PollImageLinkProperty); }
			private set { this.LoadProperty(PollResults.PollImageLinkProperty, value); }
		}

		public static readonly PropertyInfo<IPollComments> PollCommentsProperty =
			PollResults.RegisterProperty<IPollComments>(_ => _.PollComments);
		public IPollComments PollComments
		{
			get { return this.ReadProperty(PollResults.PollCommentsProperty); }
			private set { this.LoadProperty(PollResults.PollCommentsProperty, value); }
		}

#if !MOBILE
		[NonSerialized]
		private IObjectFactory<IPollDataResults> pollDataResultsFactory;
		[Dependency]
		public IObjectFactory<IPollDataResults> PollDataResultsFactory
		{
			get { return this.pollDataResultsFactory; }
			set { this.pollDataResultsFactory = value; }
		}

		[NonSerialized]
		private IObjectFactory<IPollComments> pollCommentsFactory;
		[Dependency]
		public IObjectFactory<IPollComments> PollCommentsFactory
		{
			get { return this.pollCommentsFactory; }
			set { this.pollCommentsFactory = value; }
		}
#endif
	}
}