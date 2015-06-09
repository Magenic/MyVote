using System.Linq;
using Csla;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;
using MyVote.BusinessObjects.Core.Contracts;

namespace MyVote.BusinessObjects
{
#if (!NETFX_CORE && !WINDOWS_PHONE) || ANDROID || IOS
	[System.Serializable]
#else
	[Csla.Serialization.Serializable]
#endif
	internal sealed class PollDataResults
		: ReadOnlyBaseScopeCore<PollDataResults>, IPollDataResults
	{
#if !NETFX_CORE && !WINDOWS_PHONE && !ANDROID && !IOS
		private void Child_Fetch(int pollID)
		{
			this.PollID = pollID;
			this.Question = this.Entities.MVPolls.Single(
				_ => _.PollID == pollID && (_.PollDeletedFlag == null || !_.PollDeletedFlag.Value)).PollQuestion;

			var datum = (from option in this.Entities.MVPollOptions
							 join data in
								 (from response in this.Entities.MVPollResponses
								  where response.PollID == pollID
								  group response by response.PollOptionID into responseCount
								  select new
								  {
									  PollOptionID = responseCount.Key,
									  ResponseCount = responseCount.Count(_ => _.OptionSelected)
								  }) on option.PollOptionID equals data.PollOptionID
							 select new PollData
							 {
								 OptionText = option.OptionText,
								 PollOptionID = data.PollOptionID,
								 ResponseCount = data.ResponseCount
							 }).ToList();

			var resultList = DataPortal.FetchChild<ReadOnlySwitchList<IPollDataResult>>();

			resultList.SwitchReadOnlyStatus();

			foreach (var data in datum)
			{
				resultList.Add(DataPortal.FetchChild<PollDataResult>(data));
			}

			resultList.SwitchReadOnlyStatus();
			this.Results = resultList;
		}
#endif

		public static PropertyInfo<int> PollIDProperty =
			PollDataResults.RegisterProperty<int>(_ => _.PollID);
		public int PollID
		{
			get { return this.ReadProperty(PollDataResults.PollIDProperty); }
			private set { this.LoadProperty(PollDataResults.PollIDProperty, value); }
		}

		public static PropertyInfo<string> QuestionProperty =
			PollDataResults.RegisterProperty<string>(_ => _.Question);
		public string Question
		{
			get { return this.ReadProperty(PollDataResults.QuestionProperty); }
			private set { this.LoadProperty(PollDataResults.QuestionProperty, value); }
		}

		public static PropertyInfo<ReadOnlySwitchList<IPollDataResult>> ResultsProperty =
			PollDataResults.RegisterProperty<ReadOnlySwitchList<IPollDataResult>>(_ => _.Results);
		public IReadOnlyListBaseCore<IPollDataResult> Results
		{
			get { return this.ReadProperty(PollDataResults.ResultsProperty); }
			private set { this.LoadProperty(PollDataResults.ResultsProperty, value); }
		}
	}
}
