using System.ComponentModel.DataAnnotations;
using Csla;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;

#if !NETFX_CORE && !MOBILE
using MyVote.Core.Extensions;
using MyVote.Data.Entities;
using System.Data;
using Csla.Data;
using System.Data.Entity;
#endif

namespace MyVote.BusinessObjects
{
	[System.Serializable]
	internal sealed class PollOption
		: BusinessBaseScopeCore<PollOption>, IPollOption
	{
		[RunLocal]
		protected override void Child_Create()
		{
			this.BusinessRules.CheckRules();
		}

#if !NETFX_CORE && !MOBILE
        private void Child_Fetch(MVPollOption criteria)
		{
			using (this.BypassPropertyChecks)
			{
				DataMapper.Map(criteria, this,
					criteria.GetPropertyName(_ => _.MVPoll),
					criteria.GetPropertyName(_ => _.MVPollResponses));
			}
		}

		private void Child_Insert(IPoll parent)
		{
			this.PollID = parent.PollID;
			var entity = new MVPollOption();
			DataMapper.Map(this, entity, this.IgnoredProperties.ToArray());
			this.Entities.MVPollOptions.Add(entity);
			this.Entities.SaveChanges();
			this.PollOptionID = entity.PollOptionID;
		}

		private void Child_Update(IPoll parent)
		{
			var entity = new MVPollOption();
			DataMapper.Map(this, entity, this.IgnoredProperties.ToArray());
			this.Entities.MVPollOptions.Attach(entity);
			this.Entities.SetState(entity, EntityState.Modified);
			this.Entities.SaveChanges();
		}
#endif

		public static PropertyInfo<int?> PollOptionIDProperty =
			PollOption.RegisterProperty<int?>(_ => _.PollOptionID);
		public int? PollOptionID
		{
			get { return this.ReadProperty(PollOption.PollOptionIDProperty); }
			private set { this.LoadProperty(PollOption.PollOptionIDProperty, value); }
		}

		public static PropertyInfo<int?> PollIDProperty =
			PollOption.RegisterProperty<int?>(_ => _.PollID);
		public int? PollID
		{
			get { return this.ReadProperty(PollOption.PollIDProperty); }
			private set { this.LoadProperty(PollOption.PollIDProperty, value); }
		}

		public static PropertyInfo<short?> OptionPositionProperty =
			PollOption.RegisterProperty<short?>(_ => _.OptionPosition);
		[Required(ErrorMessage = "The option position value is required.")]
		public short? OptionPosition
		{
			get { return this.GetProperty(PollOption.OptionPositionProperty); }
			set { this.SetProperty(PollOption.OptionPositionProperty, value); }
		}

		public static PropertyInfo<string> OptionTextProperty =
			PollOption.RegisterProperty<string>(_ => _.OptionText);
		[Required(ErrorMessage = "The option text value is required.")]
		[StringLength(200, ErrorMessage = "The maximum length of the option text value is 200 characters.")]
		public string OptionText
		{
			get { return this.GetProperty(PollOption.OptionTextProperty); }
			set { this.SetProperty(PollOption.OptionTextProperty, value); }
		}
	}
}
