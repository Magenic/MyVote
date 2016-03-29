using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Csla;
using MyVote.BusinessObjects.Attributes;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;

#if !NETFX_CORE && !MOBILE
using MyVote.Data.Entities;
#endif

namespace MyVote.BusinessObjects
{
	[Serializable]
	internal sealed class PollSubmissionCommand
		: CommandBaseCore<PollSubmissionCommand>, IPollSubmissionCommand
	{

		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
		[RunLocal]
		private void DataPortal_Create() { }

#if !NETFX_CORE && !MOBILE
		protected override void DataPortal_Execute()
		{
			var submissionExists = (from s in this.Entities.MVPollSubmissions
											where (s.UserID == this.UserID &&
											s.PollID == this.PollID)
											select s.PollSubmissionID).Any();

			if (!submissionExists)
			{
				this.Submission = this.PollSubmissionFactory.Create(
					new PollSubmissionCriteria(this.PollID, this.UserID));
			}
		}
#endif

		public static readonly PropertyInfo<int> PollIDProperty =
			PollSubmissionCommand.RegisterProperty<int>(_ => _.PollID);
		public int PollID
		{
			get { return this.ReadProperty(PollSubmissionCommand.PollIDProperty); }
			set { this.LoadProperty(PollSubmissionCommand.PollIDProperty, value); }
		}

		public static readonly PropertyInfo<int> UserIDProperty =
			PollSubmissionCommand.RegisterProperty<int>(_ => _.UserID);
		public int UserID
		{
			get { return this.ReadProperty(PollSubmissionCommand.UserIDProperty); }
			set { this.LoadProperty(PollSubmissionCommand.UserIDProperty, value); }
		}

		public static readonly PropertyInfo<IPollSubmission> SubmissionProperty =
			PollSubmissionCommand.RegisterProperty<IPollSubmission>(_ => _.Submission);
		public IPollSubmission Submission
		{
			get { return this.ReadProperty(PollSubmissionCommand.SubmissionProperty); }
			private set { this.LoadProperty(PollSubmissionCommand.SubmissionProperty, value); }
		}

#if !NETFX_CORE && !MOBILE
		[NonSerialized]
		private IObjectFactory<IPollSubmission> pollSubmissionFactory;
		[Dependency]
		public IObjectFactory<IPollSubmission> PollSubmissionFactory
		{
			get { return this.pollSubmissionFactory; }
			set { this.pollSubmissionFactory = value; }
		}
		[NonSerialized]
		private IEntities entities;
		[Dependency]
		public IEntities Entities
		{
			get { return this.entities; }
			set { this.entities = value; }
		}
#endif
	}
}
