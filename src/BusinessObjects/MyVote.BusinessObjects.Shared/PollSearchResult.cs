using Csla;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;
using System;

#if !NETFX_CORE && !MOBILE
using Csla.Data;
#endif

namespace MyVote.BusinessObjects
{
	[Serializable]
	internal sealed class PollSearchResult
		: ReadOnlyBaseCore<PollSearchResult>, IPollSearchResult
	{
#if !NETFX_CORE && !MOBILE
		private void Child_Fetch(PollSearchResultsData data)
		{
			this.Id = data.Id;
			this.ImageLink = data.ImageLink;
			this.Question = data.Question;
			this.SubmissionCount = data.SubmissionCount;
		}
#endif

		public static readonly PropertyInfo<int> IdProperty =
			PollSearchResult.RegisterProperty<int>(_ => _.Id);
		public int Id
		{
			get { return this.ReadProperty(PollSearchResult.IdProperty); }
			private set { this.LoadProperty(PollSearchResult.IdProperty, value); }
		}

		public static readonly PropertyInfo<string> ImageLinkProperty =
			PollSearchResult.RegisterProperty<string>(_ => _.ImageLink);
		public string ImageLink
		{
			get { return this.ReadProperty(PollSearchResult.ImageLinkProperty); }
			private set { this.LoadProperty(PollSearchResult.ImageLinkProperty, value); }
		}

		public static readonly PropertyInfo<string> QuestionProperty =
			PollSearchResult.RegisterProperty<string>(_ => _.Question);
		public string Question
		{
			get { return this.ReadProperty(PollSearchResult.QuestionProperty); }
			private set { this.LoadProperty(PollSearchResult.QuestionProperty, value); }
		}

		public static readonly PropertyInfo<int> SubmissionCountProperty =
			PollSearchResult.RegisterProperty<int>(_ => _.SubmissionCount);
		public int SubmissionCount
		{
			get { return this.ReadProperty(PollSearchResult.SubmissionCountProperty); }
			private set { this.LoadProperty(PollSearchResult.SubmissionCountProperty, value); }
		}
	}
}