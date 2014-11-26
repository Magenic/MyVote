using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Csla;
using Csla.Core;
using Csla.Rules;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;
using MyVote.BusinessObjects.Rules;

#if !NETFX_CORE && !WINDOWS_PHONE && !ANDROID && !IOS
using MyVote.Core.Extensions;
using MyVote.Repository;
using System.Data;
using System.Data.Entity;
using Csla.Data;
using System.Collections.Generic;
#endif

namespace MyVote.BusinessObjects
{
#if (!NETFX_CORE && !WINDOWS_PHONE) || ANDROID||IOS
	[System.Serializable]
#else
	[Csla.Serialization.Serializable]
#endif
	internal sealed class Poll
		: BusinessBaseScopeCore<Poll>, IPoll
	{
#if !NETFX_CORE && !WINDOWS_PHONE && !ANDROID && !IOS
		private void DataPortal_Create(int userId)
		{
			this.UserID = userId;
			this.PollOptions = DataPortal.CreateChild<BusinessList<IPollOption>>();
			this.BusinessRules.CheckRules();
		}

		private void DataPortal_Fetch(int pollId)
		{
			using (this.BypassPropertyChecks)
			{
				var entity = (from p in this.Entities.MVPolls.Include(_ => _.MVPollOptions)
								  where p.PollID == pollId
								  select p).Single();
				DataMapper.Map(entity, this,
					entity.GetPropertyName(_ => _.AuditDateCreated),
					entity.GetPropertyName(_ => _.AuditDateModified),
					entity.GetPropertyName(_ => _.MVPollOptions),
					entity.GetPropertyName(_ => _.MVPollResponses),
					entity.GetPropertyName(_ => _.MVPollSubmissions),
					entity.GetPropertyName(_ => _.MVUser),
					entity.GetPropertyName(_ => _.MVCategory),
					entity.GetPropertyName(_ => _.MVReportedPolls),
					entity.GetPropertyName(_ => _.MVReportedPollStateLogs),
					entity.GetPropertyName(_ => _.MVPollComments));

				this.PollOptions = DataPortal.FetchChild<BusinessList<IPollOption>>();

				foreach (var option in entity.MVPollOptions)
				{
					this.PollOptions.Add(DataPortal.FetchChild<PollOption>(option));
				}
			}
		}

		protected override void DataPortal_Insert()
		{
			var entity = new MVPoll();
			DataMapper.Map(this, entity, this.IgnoredProperties.ToArray());
			this.Entities.MVPolls.Add(entity);
			this.Entities.SaveChanges();
			this.PollID = entity.PollID;
			this.FieldManager.UpdateChildren(this);
		}

		protected override void DataPortal_Update()
		{
			var entity = new MVPoll();
			DataMapper.Map(this, entity, this.IgnoredProperties.ToArray());
			this.Entities.MVPolls.Attach(entity);
			this.Entities.SetState(entity, EntityState.Modified);
			this.Entities.SaveChanges();
			this.FieldManager.UpdateChildren(this);
		}

		protected override void DataPortal_DeleteSelf()
		{
			var entity = new MVPoll();
			DataMapper.Map(this, entity, this.IgnoredProperties.ToArray());
			entity.PollDeletedFlag = true;
			entity.PollDeletedDate = DateTime.UtcNow;
			this.Entities.MVPolls.Attach(entity);
			this.Entities.SetState(entity, EntityState.Modified);
			this.Entities.SaveChanges();
		}
#endif
		public static void AddObjectAuthorizationRules()
		{
			BusinessRules.AddRule(typeof(Poll), new CanDeletePollRule());
		}

		protected override void AddBusinessRules()
		{
			base.AddBusinessRules();
			this.BusinessRules.AddRule(new PollOptionsRule(Poll.PollOptionsProperty));
			this.BusinessRules.AddRule(new PollMinAnswersRule(Poll.PollMinAnswersProperty, Poll.PollOptionsProperty));
			this.BusinessRules.AddRule(new PollMaxAnswersRule(Poll.PollMaxAnswersProperty, Poll.PollOptionsProperty));
			this.BusinessRules.AddRule(new MinimumAndMaximumPollOptionRule(Poll.PollMinAnswersProperty,
				Poll.PollMinAnswersProperty, Poll.PollMaxAnswersProperty));
			this.BusinessRules.AddRule(new MinimumAndMaximumPollOptionRule(Poll.PollMaxAnswersProperty,
				Poll.PollMinAnswersProperty, Poll.PollMaxAnswersProperty));
			this.BusinessRules.AddRule(new UtcDateRule(Poll.PollStartDateProperty));
			this.BusinessRules.AddRule(new UtcDateRule(Poll.PollEndDateProperty));
			this.BusinessRules.AddRule(new StartAndEndDateRule(Poll.PollStartDateProperty,
				Poll.PollStartDateProperty, Poll.PollEndDateProperty));
			this.BusinessRules.AddRule(new StartAndEndDateRule(Poll.PollEndDateProperty,
				Poll.PollStartDateProperty, Poll.PollEndDateProperty));
		}

		protected override void OnChildChanged(ChildChangedEventArgs e)
		{
			base.OnChildChanged(e);

			if (e.ChildObject == this.PollOptions ||
				(e.ChildObject as IParent).Parent == this.PollOptions)
			{
				this.BusinessRules.CheckRules(Poll.PollOptionsProperty);
			}
		}

#if !NETFX_CORE && !WINDOWS_PHONE && !ANDROID && !IOS
		protected override List<string> IgnoredProperties
		{
			get
			{
				var properties = base.IgnoredProperties;
				properties.Add(this.GetPropertyName(_ => _.PollOptions));
				return properties;
			}
		}
#endif

		public static PropertyInfo<BusinessList<IPollOption>> PollOptionsProperty =
			Poll.RegisterProperty<BusinessList<IPollOption>>(_ => _.PollOptions);
		public BusinessList<IPollOption> PollOptions
		{
			get { return this.GetProperty(Poll.PollOptionsProperty); }
			private set { this.SetProperty(Poll.PollOptionsProperty, value); }
		}

		public static PropertyInfo<int?> PollIDProperty =
			Poll.RegisterProperty<int?>(_ => _.PollID);
		public int? PollID
		{
			get { return this.ReadProperty(Poll.PollIDProperty); }
			private set { this.LoadProperty(Poll.PollIDProperty, value); }
		}

		public static PropertyInfo<int> UserIDProperty =
			Poll.RegisterProperty<int>(_ => _.UserID);
		public int UserID
		{
			get { return this.ReadProperty(Poll.UserIDProperty); }
			private set { this.LoadProperty(Poll.UserIDProperty, value); }
		}

		public static PropertyInfo<int?> PollCategoryIDProperty =
			Poll.RegisterProperty<int?>(_ => _.PollCategoryID);
		[Required(ErrorMessage = "The category is required.")]
		public int? PollCategoryID
		{
			get { return this.GetProperty(Poll.PollCategoryIDProperty); }
			set { this.SetProperty(Poll.PollCategoryIDProperty, value); }
		}

		public static PropertyInfo<string> PollDescriptionProperty =
			Poll.RegisterProperty<string>(_ => _.PollDescription);
		public string PollDescription
		{
			get { return this.GetProperty(Poll.PollDescriptionProperty); }
			set { this.SetProperty(Poll.PollDescriptionProperty, value); }
		}

		public static PropertyInfo<string> PollQuestionProperty =
			Poll.RegisterProperty<string>(_ => _.PollQuestion);
		[Required(ErrorMessage = "The question is required.")]
		[StringLength(1000, ErrorMessage = "The question cannot be over 1000 characters in length.")]
		public string PollQuestion
		{
			get { return this.GetProperty(Poll.PollQuestionProperty); }
			set { this.SetProperty(Poll.PollQuestionProperty, value); }
		}

		public static PropertyInfo<string> PollImageLinkProperty =
			Poll.RegisterProperty<string>(_ => _.PollImageLink);
		public string PollImageLink
		{
			get { return this.GetProperty(Poll.PollImageLinkProperty); }
			set { this.SetProperty(Poll.PollImageLinkProperty, value); }
		}

		public static PropertyInfo<short?> PollMaxAnswersProperty =
			Poll.RegisterProperty<short?>(_ => _.PollMaxAnswers);
		[Required(ErrorMessage = "The maximum answers value is required.")]
		public short? PollMaxAnswers
		{
			get { return this.GetProperty(Poll.PollMaxAnswersProperty); }
			set { this.SetProperty(Poll.PollMaxAnswersProperty, value); }
		}

		public static PropertyInfo<short?> PollMinAnswersProperty =
			Poll.RegisterProperty<short?>(_ => _.PollMinAnswers);
		[Required(ErrorMessage = "The minimum answers value is required.")]
		public short? PollMinAnswers
		{
			get { return this.GetProperty(Poll.PollMinAnswersProperty); }
			set { this.SetProperty(Poll.PollMinAnswersProperty, value); }
		}

		public static PropertyInfo<DateTime?> PollStartDateProperty =
			Poll.RegisterProperty<DateTime?>(_ => _.PollStartDate);
		[Required(ErrorMessage = "The start date value is required.")]
		public DateTime? PollStartDate
		{
			get { return this.GetProperty(Poll.PollStartDateProperty); }
			set { this.SetProperty(Poll.PollStartDateProperty, value); }
		}

		public static PropertyInfo<DateTime?> PollEndDateProperty =
			Poll.RegisterProperty<DateTime?>(_ => _.PollEndDate);
		[Required(ErrorMessage = "The end date value is required.")]
		public DateTime? PollEndDate
		{
			get { return this.GetProperty(Poll.PollEndDateProperty); }
			set { this.SetProperty(Poll.PollEndDateProperty, value); }
		}

		public static PropertyInfo<bool?> PollAdminRemovedFlagProperty =
			Poll.RegisterProperty<bool?>(_ => _.PollAdminRemovedFlag);
		public bool? PollAdminRemovedFlag
		{
			get { return this.GetProperty(Poll.PollAdminRemovedFlagProperty); }
			set { this.SetProperty(Poll.PollAdminRemovedFlagProperty, value); }
		}

		public static PropertyInfo<DateTime?> PollDateRemovedProperty =
			Poll.RegisterProperty<DateTime?>(_ => _.PollDateRemoved);
		public DateTime? PollDateRemoved
		{
			get { return this.GetProperty(Poll.PollDateRemovedProperty); }
			set { this.SetProperty(Poll.PollDateRemovedProperty, value); }
		}

		public static PropertyInfo<bool?> PollDeletedFlagProperty =
			Poll.RegisterProperty<bool?>(_ => _.PollDeletedFlag);
		public bool? PollDeletedFlag
		{
			get { return this.GetProperty(Poll.PollDeletedFlagProperty); }
			set { this.SetProperty(Poll.PollDeletedFlagProperty, value); }
		}

		public static PropertyInfo<DateTime?> PollDeletedDateProperty =
			Poll.RegisterProperty<DateTime?>(_ => _.PollDeletedDate);
		public DateTime? PollDeletedDate
		{
			get { return this.GetProperty(Poll.PollDeletedDateProperty); }
			set { this.SetProperty(Poll.PollDeletedDateProperty, value); }
		}
	}
}
