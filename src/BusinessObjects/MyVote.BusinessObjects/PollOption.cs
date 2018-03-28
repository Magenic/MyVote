using System.ComponentModel.DataAnnotations;
using Csla;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;
using System;

#if !MOBILE
using MyVote.Data.Entities;
using System.Data;
using Csla.Data;
using Microsoft.EntityFrameworkCore;
#endif

namespace MyVote.BusinessObjects
{
	[Serializable]
	internal sealed class PollOption
		: BusinessBaseCore<PollOption>, IPollOption
	{
		[RunLocal]
		protected override void Child_Create()
		{
			this.BusinessRules.CheckRules();
		}

#if !MOBILE
		private void Child_Fetch(MvpollOption criteria)
		{
			using (this.BypassPropertyChecks)
			{
				this.OptionPosition = criteria.OptionPosition;
				this.OptionText = criteria.OptionText;
				this.PollID = criteria.PollId;
				this.PollOptionID = criteria.PollOptionId;
			}
		}

		private MvpollOption MapTo()
		{
			var entity = new MvpollOption();

			entity.OptionPosition = this.OptionPosition.Value;
			entity.OptionText = this.OptionText;
			entity.PollId = this.PollID.Value;
			entity.PollOptionId = this.PollOptionID != null ? 
				this.PollOptionID.Value : entity.PollOptionId;

			return entity;
		}
		private void Child_Insert(IPoll parent)
		{
			this.PollID = parent.PollID;
			var entity = this.MapTo();
			this.Entities.MvpollOption.Add(entity);
			this.Entities.SaveChanges();
			this.PollOptionID = entity.PollOptionId;
		}

		private void Child_Update(IPoll parent)
		{
			var entity = this.MapTo();
			this.Entities.MvpollOption.Attach(entity);
			this.Entities.SetState(entity, EntityState.Modified);
			this.Entities.SaveChanges();
		}
#endif

		public static readonly PropertyInfo<int?> PollOptionIDProperty =
			PollOption.RegisterProperty<int?>(_ => _.PollOptionID);
		public int? PollOptionID
		{
			get { return this.ReadProperty(PollOption.PollOptionIDProperty); }
			private set { this.LoadProperty(PollOption.PollOptionIDProperty, value); }
		}

		public static readonly PropertyInfo<int?> PollIDProperty =
			PollOption.RegisterProperty<int?>(_ => _.PollID);
		public int? PollID
		{
			get { return this.ReadProperty(PollOption.PollIDProperty); }
			private set { this.LoadProperty(PollOption.PollIDProperty, value); }
		}

		public static readonly PropertyInfo<short?> OptionPositionProperty =
			PollOption.RegisterProperty<short?>(_ => _.OptionPosition);
		[Required(ErrorMessage = "The option position value is required.")]
		public short? OptionPosition
		{
			get { return this.GetProperty(PollOption.OptionPositionProperty); }
			set { this.SetProperty(PollOption.OptionPositionProperty, value); }
		}

		public static readonly PropertyInfo<string> OptionTextProperty =
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
