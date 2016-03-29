using System.Linq;
using Csla;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;
using MyVote.BusinessObjects.Core.Contracts;
using System;
using MyVote.BusinessObjects.Attributes;

namespace MyVote.BusinessObjects
{
	[System.Serializable]
	internal sealed class PollDataResults
		: ReadOnlyBaseCore<PollDataResults>, IPollDataResults
	{
#if !NETFX_CORE && !MOBILE
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

			var resultList = this.pollDataResultsFactory.FetchChild();

			resultList.SwitchReadOnlyStatus();

			foreach (var data in datum)
			{
				resultList.Add(this.pollDataResultFactory.FetchChild(data));
			}

			resultList.SwitchReadOnlyStatus();
			this.Results = resultList;
		}

		[NonSerialized]
		private IObjectFactory<ReadOnlySwitchList<IPollDataResult>> pollDataResultsFactory;
		[Dependency]
		public IObjectFactory<ReadOnlySwitchList<IPollDataResult>> PollDataResultsFactory
		{
			get { return this.pollDataResultsFactory; }
			set { this.pollDataResultsFactory = value; }
		}

		[NonSerialized]
		private IObjectFactory<IPollDataResult> pollDataResultFactory;
		[Dependency]
		public IObjectFactory<IPollDataResult> PollDataResultFactory
		{
			get { return this.pollDataResultFactory; }
			set { this.pollDataResultFactory = value; }
		}
#endif

		public static readonly PropertyInfo<int> PollIDProperty =
			PollDataResults.RegisterProperty<int>(_ => _.PollID);
		public int PollID
		{
			get { return this.ReadProperty(PollDataResults.PollIDProperty); }
			private set { this.LoadProperty(PollDataResults.PollIDProperty, value); }
		}

		public static readonly PropertyInfo<string> QuestionProperty =
			PollDataResults.RegisterProperty<string>(_ => _.Question);
		public string Question
		{
			get { return this.ReadProperty(PollDataResults.QuestionProperty); }
			private set { this.LoadProperty(PollDataResults.QuestionProperty, value); }
		}

		public static readonly PropertyInfo<ReadOnlySwitchList<IPollDataResult>> ResultsProperty =
			PollDataResults.RegisterProperty<ReadOnlySwitchList<IPollDataResult>>(_ => _.Results);
		public IReadOnlyListBaseCore<IPollDataResult> Results
		{
			get { return this.ReadProperty(PollDataResults.ResultsProperty); }
			private set { this.LoadProperty(PollDataResults.ResultsProperty, value); }
		}
	}
}
