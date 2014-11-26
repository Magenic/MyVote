using Csla;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;

namespace MyVote.BusinessObjects
{
#if (!NETFX_CORE && !WINDOWS_PHONE) || ANDROID || IOS
	[System.Serializable]
#else
	[Csla.Serialization.Serializable]
#endif
	internal sealed class PollDataResult
		: ReadOnlyBaseCore<PollDataResult>, IPollDataResult
	{
#if !NETFX_CORE && !WINDOWS_PHONE && !ANDROID && !IOS
		private void Child_Fetch(PollData data)
		{
			Csla.Data.DataMapper.Map(data, this);
		}
#endif

		public static PropertyInfo<string> OptionTextProperty =
			PollDataResult.RegisterProperty<string>(_ => _.OptionText);
		public string OptionText
		{
			get { return this.ReadProperty(PollDataResult.OptionTextProperty); }
			private set { this.LoadProperty(PollDataResult.OptionTextProperty, value); }
		}

		public static PropertyInfo<int> PollOptionIDProperty =
			PollDataResult.RegisterProperty<int>(_ => _.PollOptionID);
		public int PollOptionID
		{
			get { return this.ReadProperty(PollDataResult.PollOptionIDProperty); }
			private set { this.LoadProperty(PollDataResult.PollOptionIDProperty, value); }
		}

		public static PropertyInfo<int> ResponseCountProperty =
			PollDataResult.RegisterProperty<int>(_ => _.ResponseCount);
		public int ResponseCount
		{
			get { return this.ReadProperty(PollDataResult.ResponseCountProperty); }
			private set { this.LoadProperty(PollDataResult.ResponseCountProperty, value); }
		}
	}
}
