using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Csla;
using Csla.Core;
using Csla.Rules;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;
using MyVote.BusinessObjects.Rules;
using MyVote.BusinessObjects.Attributes;

#if !MOBILE
using MyVote.Data.Entities;
using System.Data;
using Csla.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
#endif

namespace MyVote.BusinessObjects
{
	[Serializable]
	internal sealed class Poll
		: BusinessBaseCore<Poll>, IPoll
	{
#if !MOBILE
		private void DataPortal_Create(int userId)
		{
			this.UserID = userId;
			this.PollOptions = this.pollOptionsFactory.CreateChild();
			this.BusinessRules.CheckRules();
		}

		private void DataPortal_Fetch(int pollId)
		{
			using (this.BypassPropertyChecks)
			{
				var entity = (from p in this.Entities.Mvpoll
								  where p.PollId == pollId
								  select new
								  {
									  PollAdminRemovedFlag = p.PollAdminRemovedFlag,
									  PollCategoryId = p.PollCategoryId,
									  PollOptions = p.MvpollOption,
									  PollDateRemoved = p.PollDateRemoved,
									  PollDeletedDate = p.PollDeletedDate,
									  PollDeletedFlag = p.PollDeletedFlag,
									  PollDescription = p.PollDescription,
									  PollEndDate = p.PollEndDate,
									  PollId = p.PollId,
									  PollImageLink = p.PollImageLink,
									  PollMaxAnswers = p.PollMaxAnswers,
									  PollMinAnswers = p.PollMinAnswers,
									  PollQuestion = p.PollQuestion,
									  PollStartDate = p.PollStartDate,
									  UserId = p.UserId
								  }).Single();

				this.PollAdminRemovedFlag = entity.PollAdminRemovedFlag;
				this.PollCategoryID = entity.PollCategoryId;
				this.PollDateRemoved = entity.PollDateRemoved;
				this.PollDeletedDate = entity.PollDeletedDate;
				this.PollDeletedFlag = entity.PollDeletedFlag;
				this.PollDescription = entity.PollDescription;
				this.PollEndDate = entity.PollEndDate;
				this.PollID = entity.PollId;
				this.PollImageLink = entity.PollImageLink;
				this.PollMaxAnswers = entity.PollMaxAnswers;
				this.PollMinAnswers = entity.PollMinAnswers;
				this.PollQuestion = entity.PollQuestion;
				this.PollStartDate = entity.PollStartDate;
				this.UserID = entity.UserId;

				this.PollOptions = this.pollOptionsFactory.FetchChild();

				foreach (var option in entity.PollOptions)
				{
					this.PollOptions.Add(this.pollOptionFactory.FetchChild(option));
				}
			}
		}

		private Mvpoll MapTo()
		{
			var entity = new Mvpoll();

			entity.PollAdminRemovedFlag = this.PollAdminRemovedFlag;
			entity.PollCategoryId = this.PollCategoryID.Value;
			entity.PollDateRemoved = this.PollDateRemoved;
			entity.PollDeletedDate = this.PollDeletedDate;
			entity.PollDeletedFlag = this.PollDeletedFlag;
			entity.PollDescription = this.PollDescription;
			entity.PollEndDate = this.PollEndDate.Value;
			entity.PollImageLink = this.PollImageLink;
			entity.PollMaxAnswers = this.PollMaxAnswers;
			entity.PollMinAnswers = this.PollMinAnswers;
			entity.PollQuestion = this.PollQuestion;
			entity.PollStartDate = this.PollStartDate.Value;
			entity.UserId = this.UserID;

			return entity;
		}

		protected override void DataPortal_Insert()
		{
			var entity = this.MapTo();

			this.Entities.Mvpoll.Add(entity);
			this.Entities.SaveChanges();
			this.PollID = entity.PollId;
			this.FieldManager.UpdateChildren(this);
		}

		protected override void DataPortal_Update()
		{
			var entity = this.MapTo();
			entity.PollId = (int)this.PollID;

			this.Entities.Mvpoll.Attach(entity);
			this.Entities.SetState(entity, EntityState.Modified);
			this.Entities.SaveChanges();
			this.FieldManager.UpdateChildren(this);
		}

		protected override void DataPortal_DeleteSelf()
		{
			var entity = this.MapTo();

			entity.PollDeletedFlag = true;
			entity.PollDeletedDate = DateTime.UtcNow;
			this.Entities.Mvpoll.Attach(entity);
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

#if !MOBILE
		protected override List<string> IgnoredProperties
		{
			get
			{
				var properties = base.IgnoredProperties;
				properties.AddRange(new[] { nameof(this.PollOptions),
					nameof(this.PollOptionsFactory), nameof(this.PollOptionFactory) });
				return properties;
			}
		}

		[NonSerialized]
		private IObjectFactory<BusinessList<IPollOption>> pollOptionsFactory;
		[Dependency]
		public IObjectFactory<BusinessList<IPollOption>> PollOptionsFactory
		{
			get { return this.pollOptionsFactory; }
			set { this.pollOptionsFactory = value; }
		}

		[NonSerialized]
		private IObjectFactory<IPollOption> pollOptionFactory;
		[Dependency]
		public IObjectFactory<IPollOption> PollOptionFactory
		{
			get { return this.pollOptionFactory; }
			set { this.pollOptionFactory = value; }
		}
#endif

		public static readonly PropertyInfo<BusinessList<IPollOption>> PollOptionsProperty =
			Poll.RegisterProperty<BusinessList<IPollOption>>(_ => _.PollOptions);
		public BusinessList<IPollOption> PollOptions
		{
			get { return this.GetProperty(Poll.PollOptionsProperty); }
			private set { this.SetProperty(Poll.PollOptionsProperty, value); }
		}

		public static readonly PropertyInfo<int?> PollIDProperty =
			Poll.RegisterProperty<int?>(_ => _.PollID);
		public int? PollID
		{
			get { return this.ReadProperty(Poll.PollIDProperty); }
			private set { this.LoadProperty(Poll.PollIDProperty, value); }
		}

		public static readonly PropertyInfo<int> UserIDProperty =
			Poll.RegisterProperty<int>(_ => _.UserID);
		public int UserID
		{
			get { return this.ReadProperty(Poll.UserIDProperty); }
			private set { this.LoadProperty(Poll.UserIDProperty, value); }
		}

		public static readonly PropertyInfo<int?> PollCategoryIDProperty =
			Poll.RegisterProperty<int?>(_ => _.PollCategoryID);
		[Required(ErrorMessage = "The category is required.")]
		public int? PollCategoryID
		{
			get { return this.GetProperty(Poll.PollCategoryIDProperty); }
			set { this.SetProperty(Poll.PollCategoryIDProperty, value); }
		}

		public static readonly PropertyInfo<string> PollDescriptionProperty =
			Poll.RegisterProperty<string>(_ => _.PollDescription);
		public string PollDescription
		{
			get { return this.GetProperty(Poll.PollDescriptionProperty); }
			set { this.SetProperty(Poll.PollDescriptionProperty, value); }
		}

		public static readonly PropertyInfo<string> PollQuestionProperty =
			Poll.RegisterProperty<string>(_ => _.PollQuestion);
		[Required(ErrorMessage = "The question is required.")]
		[StringLength(1000, ErrorMessage = "The question cannot be over 1000 characters in length.")]
		public string PollQuestion
		{
			get { return this.GetProperty(Poll.PollQuestionProperty); }
			set { this.SetProperty(Poll.PollQuestionProperty, value); }
		}

		public static readonly PropertyInfo<string> PollImageLinkProperty =
			Poll.RegisterProperty<string>(_ => _.PollImageLink);
		public string PollImageLink
		{
			get { return this.GetProperty(Poll.PollImageLinkProperty); }
			set { this.SetProperty(Poll.PollImageLinkProperty, value); }
		}

		public static readonly PropertyInfo<short?> PollMaxAnswersProperty =
			Poll.RegisterProperty<short?>(_ => _.PollMaxAnswers);
		[Required(ErrorMessage = "The maximum answers value is required.")]
		public short? PollMaxAnswers
		{
			get { return this.GetProperty(Poll.PollMaxAnswersProperty); }
			set { this.SetProperty(Poll.PollMaxAnswersProperty, value); }
		}

		public static readonly PropertyInfo<short?> PollMinAnswersProperty =
			Poll.RegisterProperty<short?>(_ => _.PollMinAnswers);
		[Required(ErrorMessage = "The minimum answers value is required.")]
		public short? PollMinAnswers
		{
			get { return this.GetProperty(Poll.PollMinAnswersProperty); }
			set { this.SetProperty(Poll.PollMinAnswersProperty, value); }
		}

		public static readonly PropertyInfo<DateTime?> PollStartDateProperty =
			Poll.RegisterProperty<DateTime?>(_ => _.PollStartDate);
		[Required(ErrorMessage = "The start date value is required.")]
		public DateTime? PollStartDate
		{
			get { return this.GetProperty(Poll.PollStartDateProperty); }
			set { this.SetProperty(Poll.PollStartDateProperty, value); }
		}

		public static readonly PropertyInfo<DateTime?> PollEndDateProperty =
			Poll.RegisterProperty<DateTime?>(_ => _.PollEndDate);
		[Required(ErrorMessage = "The end date value is required.")]
		public DateTime? PollEndDate
		{
			get { return this.GetProperty(Poll.PollEndDateProperty); }
			set { this.SetProperty(Poll.PollEndDateProperty, value); }
		}

		public static readonly PropertyInfo<bool?> PollAdminRemovedFlagProperty =
			Poll.RegisterProperty<bool?>(_ => _.PollAdminRemovedFlag);
		public bool? PollAdminRemovedFlag
		{
			get { return this.GetProperty(Poll.PollAdminRemovedFlagProperty); }
			set { this.SetProperty(Poll.PollAdminRemovedFlagProperty, value); }
		}

		public static readonly PropertyInfo<DateTime?> PollDateRemovedProperty =
			Poll.RegisterProperty<DateTime?>(_ => _.PollDateRemoved);
		public DateTime? PollDateRemoved
		{
			get { return this.GetProperty(Poll.PollDateRemovedProperty); }
			set { this.SetProperty(Poll.PollDateRemovedProperty, value); }
		}

		public static readonly PropertyInfo<bool?> PollDeletedFlagProperty =
			Poll.RegisterProperty<bool?>(_ => _.PollDeletedFlag);
		public bool? PollDeletedFlag
		{
			get { return this.GetProperty(Poll.PollDeletedFlagProperty); }
			set { this.SetProperty(Poll.PollDeletedFlagProperty, value); }
		}

		public static readonly PropertyInfo<DateTime?> PollDeletedDateProperty =
			Poll.RegisterProperty<DateTime?>(_ => _.PollDeletedDate);
		public DateTime? PollDeletedDate
		{
			get { return this.GetProperty(Poll.PollDeletedDateProperty); }
			set { this.SetProperty(Poll.PollDeletedDateProperty, value); }
		}
	}
}