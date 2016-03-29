using System;
using Csla;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;

#if !NETFX_CORE && !MOBILE
using MyVote.Data.Entities;
#endif

namespace MyVote.BusinessObjects
{
	[System.Serializable]
	internal sealed class PollSubmissionResponse
		: BusinessBaseCore<PollSubmissionResponse>, IPollSubmissionResponse
	{
#if !NETFX_CORE && !MOBILE
		private void Child_Create(IPollOption option)
		{
			this.PollOptionID = option.PollOptionID.Value;
			this.OptionPosition = option.OptionPosition.Value;
			this.OptionText = option.OptionText;
			this.BusinessRules.CheckRules();
		}

		private void Child_Insert(IPollSubmission parent)
		{
			var entity = new MVPollResponse
			{
				OptionSelected = this.IsOptionSelected,
				PollID = parent.PollID,
				PollSubmissionID = parent.PollSubmissionID.Value,
				PollOptionID = this.PollOptionID,
				ResponseDate = DateTime.UtcNow,
				UserID = parent.UserID
			};

			this.Entities.MVPollResponses.Add(entity);
			this.Entities.SaveChanges();
			this.PollResponseID = entity.PollResponseID;
		}
#endif

		public static readonly PropertyInfo<int?> PollResponseIDProperty =
			PollSubmissionResponse.RegisterProperty<int?>(_ => _.PollResponseID);
		public int? PollResponseID
		{
			get { return this.ReadProperty(PollSubmissionResponse.PollResponseIDProperty); }
			private set { this.LoadProperty(PollSubmissionResponse.PollResponseIDProperty, value); }
		}

		public static readonly PropertyInfo<int> PollOptionIDProperty =
			PollSubmissionResponse.RegisterProperty<int>(_ => _.PollOptionID);
		public int PollOptionID
		{
			get { return this.ReadProperty(PollSubmissionResponse.PollOptionIDProperty); }
			private set { this.LoadProperty(PollSubmissionResponse.PollOptionIDProperty, value); }
		}

		public static readonly PropertyInfo<bool> IsOptionSelectedProperty =
			PollSubmissionResponse.RegisterProperty<bool>(_ => _.IsOptionSelected);
		public bool IsOptionSelected
		{
			get { return this.GetProperty(PollSubmissionResponse.IsOptionSelectedProperty); }
			set { this.SetProperty(PollSubmissionResponse.IsOptionSelectedProperty, value); }
		}

		public static readonly PropertyInfo<short> OptionPositionProperty =
			PollSubmissionResponse.RegisterProperty<short>(_ => _.OptionPosition);
		public short OptionPosition
		{
			get { return this.ReadProperty(PollSubmissionResponse.OptionPositionProperty); }
			private set { this.LoadProperty(PollSubmissionResponse.OptionPositionProperty, value); }
		}

		public static readonly PropertyInfo<string> OptionTextProperty =
			PollSubmissionResponse.RegisterProperty<string>(_ => _.OptionText);
		public string OptionText
		{
			get { return this.ReadProperty(PollSubmissionResponse.OptionTextProperty); }
			private set { this.LoadProperty(PollSubmissionResponse.OptionTextProperty, value); }
		}
	}
}