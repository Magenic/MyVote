using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Csla;
using Csla.Core;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;
using MyVote.BusinessObjects.Rules;

#if !NETFX_CORE && !WINDOWS_PHONE && !ANDROID && !IOS
using MyVote.Data.Entities;
#endif

namespace MyVote.BusinessObjects
{
#if (!NETFX_CORE && !WINDOWS_PHONE) || ANDROID || IOS
	[System.Serializable]
#else
	[Csla.Serialization.Serializable]
#endif
	internal sealed class PollSubmission
		: BusinessBaseScopeCore<PollSubmission>, IPollSubmission
	{
		protected override void AddBusinessRules()
		{
			base.AddBusinessRules();
			this.BusinessRules.AddRule(new MinimumAndMaximumPollSubmissionResponsesRule(
				PollSubmission.ResponsesProperty, PollSubmission.PollMinAnswersProperty, PollSubmission.PollMaxAnswersProperty));
			this.BusinessRules.AddRule(new PollSubmissionPollDeletedFlagRule(
				PollSubmission.PollDeletedFlagProperty, PollSubmission.IsActiveProperty));
			this.BusinessRules.AddRule(new PollSubmissionPollStartDateRule(
				PollSubmission.PollStartDateProperty, PollSubmission.IsActiveProperty));
			this.BusinessRules.AddRule(new PollSubmissionPollEndDateRule(
				PollSubmission.PollEndDateProperty, PollSubmission.IsActiveProperty));
		}

		protected override void OnChildChanged(ChildChangedEventArgs e)
		{
			base.OnChildChanged(e);

			if (e.ChildObject == this.Responses ||
				(e.ChildObject as IParent).Parent == this.Responses)
			{
				this.BusinessRules.CheckRules(PollSubmission.ResponsesProperty);
			}
		}

#if !NETFX_CORE && !WINDOWS_PHONE && !ANDROID && !IOS
		private void DataPortal_Fetch(PollSubmissionCriteria criteria)
		{
			using (this.BypassPropertyChecks)
			{
				this.FillFromPoll(criteria);

				var submission = GetSubmissions(criteria).FirstOrDefault();

				if (submission == null)
				{
					return;
				}

				this.SubmissionDate = submission.PollSubmissionDate;

				foreach (var response in submission.MVPollResponses)
				{
					this.Responses.Single(r => r.PollOptionID == response.PollOptionID)
						.IsOptionSelected = response.OptionSelected;
				}
			}
		}

		private void DataPortal_Create(PollSubmissionCriteria criteria)
		{
			var existingSubmission = this.GetSubmissions(criteria).Any();

			if (existingSubmission)
			{
				throw new NotSupportedException("A user cannot vote on a poll more than once.");
			}

			this.FillFromPoll(criteria);
			this.BusinessRules.CheckRules();
		}

		private IQueryable<MVPollSubmission> GetSubmissions(PollSubmissionCriteria criteria)
		{
			return from submission in this.Entities.MVPollSubmissions
					 where (submission.PollID == criteria.PollID &&
							 submission.UserID == criteria.UserID)
					 select submission;
		}

		private void FillFromPoll(PollSubmissionCriteria criteria)
		{
			var poll = DataPortal.Fetch<Poll>(criteria.PollID);
			var now = DateTime.UtcNow;

			// The rules that will kick for the next three properties will set the active flag correctly;
			this.IsActive = true;
			this.IsPollOwnedByUser = poll.UserID == criteria.UserID;
			this.PollDeletedFlag = poll.PollDeletedFlag;
			this.PollStartDate = poll.PollStartDate.Value;
			this.PollEndDate = poll.PollEndDate.Value;
			this.CategoryName = (from category in this.Entities.MVCategories
										where category.CategoryID == poll.PollCategoryID
										select category.CategoryName).Single();
			this.PollImageLink = poll.PollImageLink;
			this.PollID = criteria.PollID;
			this.UserID = criteria.UserID;
			this.PollQuestion = poll.PollQuestion;
			this.PollDescription = poll.PollDescription;
			this.PollMinAnswers = poll.PollMinAnswers.Value;
			this.PollMaxAnswers = poll.PollMaxAnswers.Value;
			this.Responses = DataPortal.CreateChild<PollSubmissionResponseCollection>(poll.PollOptions);
		}

		protected override void DataPortal_Insert()
		{
			var entity = new MVPollSubmission
			{
				PollID = this.PollID,
				PollSubmissionComment = this.Comment,
				PollSubmissionDate = DateTime.UtcNow,
				UserID = this.UserID
			};

			this.Entities.MVPollSubmissions.Add(entity);
			this.Entities.SaveChanges();
			this.PollSubmissionID = entity.PollSubmissionID;
			this.FieldManager.UpdateChildren(this);
		}
#endif

		public static PropertyInfo<bool> IsActiveProperty =
			PollSubmission.RegisterProperty<bool>(_ => _.IsActive);
		public bool IsActive
		{
			get { return this.ReadProperty(PollSubmission.IsActiveProperty); }
			private set { this.LoadProperty(PollSubmission.IsActiveProperty, value); }
		}

		public static PropertyInfo<bool> IsPollOwnedByUserProperty =
			PollSubmission.RegisterProperty<bool>(_ => _.IsPollOwnedByUser);
		public bool IsPollOwnedByUser
		{
			get { return this.ReadProperty(PollSubmission.IsPollOwnedByUserProperty); }
			private set { this.LoadProperty(PollSubmission.IsPollOwnedByUserProperty, value); }
		}

		public static PropertyInfo<string> CategoryNameProperty =
			PollSubmission.RegisterProperty<string>(_ => _.CategoryName);
		public string CategoryName
		{
			get { return this.ReadProperty(PollSubmission.CategoryNameProperty); }
			private set { this.LoadProperty(PollSubmission.CategoryNameProperty, value); }
		}

		public static PropertyInfo<string> CommentProperty =
			PollSubmission.RegisterProperty<string>(_ => _.Comment);
		[StringLength(1000, ErrorMessage = "The comment cannot be over 1000 characters in length.")]
		public string Comment
		{
			get { return this.GetProperty(PollSubmission.CommentProperty); }
			set { this.SetProperty(PollSubmission.CommentProperty, value); }
		}

		public static PropertyInfo<int> PollIDProperty =
			PollSubmission.RegisterProperty<int>(_ => _.PollID);
		public int PollID
		{
			get { return this.ReadProperty(PollSubmission.PollIDProperty); }
			private set { this.LoadProperty(PollSubmission.PollIDProperty, value); }
		}

		public static PropertyInfo<bool?> PollDeletedFlagProperty =
			PollSubmission.RegisterProperty<bool?>(_ => _.PollDeletedFlag);
		public bool? PollDeletedFlag
		{
			get { return this.GetProperty(PollSubmission.PollDeletedFlagProperty); }
			private set { this.SetProperty(PollSubmission.PollDeletedFlagProperty, value); }
		}

		public static PropertyInfo<DateTime> PollStartDateProperty =
			PollSubmission.RegisterProperty<DateTime>(_ => _.PollStartDate);
		public DateTime PollStartDate
		{
			get { return this.GetProperty(PollSubmission.PollStartDateProperty); }
			private set { this.SetProperty(PollSubmission.PollStartDateProperty, value); }
		}

		public static PropertyInfo<DateTime> PollEndDateProperty =
			PollSubmission.RegisterProperty<DateTime>(_ => _.PollEndDate);
		public DateTime PollEndDate
		{
			get { return this.GetProperty(PollSubmission.PollEndDateProperty); }
			private set { this.SetProperty(PollSubmission.PollEndDateProperty, value); }
		}

		public static PropertyInfo<string> PollImageLinkProperty =
			PollSubmission.RegisterProperty<string>(_ => _.PollImageLink);
		public string PollImageLink
		{
			get { return this.ReadProperty(PollSubmission.PollImageLinkProperty); }
			private set { this.LoadProperty(PollSubmission.PollImageLinkProperty, value); }
		}

		public static PropertyInfo<short> PollMaxAnswersProperty =
			PollSubmission.RegisterProperty<short>(_ => _.PollMaxAnswers);
		public short PollMaxAnswers
		{
			get { return this.ReadProperty(PollSubmission.PollMaxAnswersProperty); }
			private set { this.LoadProperty(PollSubmission.PollMaxAnswersProperty, value); }
		}

		public static PropertyInfo<short> PollMinAnswersProperty =
			PollSubmission.RegisterProperty<short>(_ => _.PollMinAnswers);
		public short PollMinAnswers
		{
			get { return this.ReadProperty(PollSubmission.PollMinAnswersProperty); }
			private set { this.LoadProperty(PollSubmission.PollMinAnswersProperty, value); }
		}

		public static PropertyInfo<int?> PollSubmissionIDProperty =
			PollSubmission.RegisterProperty<int?>(_ => _.PollSubmissionID);
		public int? PollSubmissionID
		{
			get { return this.ReadProperty(PollSubmission.PollSubmissionIDProperty); }
			private set { this.LoadProperty(PollSubmission.PollSubmissionIDProperty, value); }
		}

		public static PropertyInfo<IPollSubmissionResponseCollection> ResponsesProperty =
			PollSubmission.RegisterProperty<IPollSubmissionResponseCollection>(_ => _.Responses);
		public IPollSubmissionResponseCollection Responses
		{
			get { return this.ReadProperty(PollSubmission.ResponsesProperty); }
			private set { this.LoadProperty(PollSubmission.ResponsesProperty, value); }
		}

		public static PropertyInfo<int> UserIDProperty =
			PollSubmission.RegisterProperty<int>(_ => _.UserID);
		public int UserID
		{
			get { return this.ReadProperty(PollSubmission.UserIDProperty); }
			private set { this.LoadProperty(PollSubmission.UserIDProperty, value); }
		}

		public static PropertyInfo<DateTime?> SubmissionDateProperty =
			PollSubmission.RegisterProperty<DateTime?>(_ => _.SubmissionDate);
		public DateTime? SubmissionDate
		{
			get { return this.ReadProperty(PollSubmission.SubmissionDateProperty); }
			private set { this.LoadProperty(PollSubmission.SubmissionDateProperty, value); }
		}

		public static PropertyInfo<string> PollDescriptionProperty =
			PollSubmission.RegisterProperty<string>(_ => _.PollDescription);
		public string PollDescription
		{
			get { return this.ReadProperty(PollSubmission.PollDescriptionProperty); }
			private set { this.LoadProperty(PollSubmission.PollDescriptionProperty, value); }
		}

		public static PropertyInfo<string> PollQuestionProperty =
			PollSubmission.RegisterProperty<string>(_ => _.PollQuestion);
		public string PollQuestion
		{
			get { return this.ReadProperty(PollSubmission.PollQuestionProperty); }
			private set { this.LoadProperty(PollSubmission.PollQuestionProperty, value); }
		}
	}
}
