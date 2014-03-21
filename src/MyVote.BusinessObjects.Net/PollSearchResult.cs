using Csla;
using Csla.Data;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;
using MyVote.Core.Extensions;

namespace MyVote.BusinessObjects
{
#if (!NETFX_CORE && !WINDOWS_PHONE) || ANDROID
	[System.Serializable]
#else
	[Csla.Serialization.Serializable]
#endif
	internal sealed class PollSearchResult
		: ReadOnlyBaseCore<PollSearchResult>, IPollSearchResult
	{
#if !NETFX_CORE && !WINDOWS_PHONE
		private void Child_Fetch(PollSearchResultsData data)
		{
			DataMapper.Map(data, this,
				data.GetPropertyName(_ => _.Category));
		}
#endif

		public static PropertyInfo<int> IdProperty =
			PollSearchResult.RegisterProperty<int>(_ => _.Id);
		public int Id
		{
			get { return this.ReadProperty(PollSearchResult.IdProperty); }
			private set { this.LoadProperty(PollSearchResult.IdProperty, value); }
		}

		public static PropertyInfo<string> ImageLinkProperty =
			PollSearchResult.RegisterProperty<string>(_ => _.ImageLink);
		public string ImageLink
		{
			get { return this.ReadProperty(PollSearchResult.ImageLinkProperty); }
			private set { this.LoadProperty(PollSearchResult.ImageLinkProperty, value); }
		}

		public static PropertyInfo<string> QuestionProperty =
			PollSearchResult.RegisterProperty<string>(_ => _.Question);
		public string Question
		{
			get { return this.ReadProperty(PollSearchResult.QuestionProperty); }
			private set { this.LoadProperty(PollSearchResult.QuestionProperty, value); }
		}

		public static PropertyInfo<int> SubmissionCountProperty =
			PollSearchResult.RegisterProperty<int>(_ => _.SubmissionCount);
		public int SubmissionCount
		{
			get { return this.ReadProperty(PollSearchResult.SubmissionCountProperty); }
			private set { this.LoadProperty(PollSearchResult.SubmissionCountProperty, value); }
		}
	}
}
