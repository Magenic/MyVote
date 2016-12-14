using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Csla;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;
using MyVote.BusinessObjects.Core.Contracts;
using MyVote.BusinessObjects.Attributes;

#if !NETFX_CORE && !MOBILE
using MyVote.Data.Entities;
#endif

namespace MyVote.BusinessObjects
{
	[System.Serializable]
	internal sealed class PollSearchResults
		: ReadOnlyBaseCore<PollSearchResults>, IPollSearchResults
	{
		private const int MaximumResultCount = 50;

#if !NETFX_CORE && !MOBILE
		private void DataPortal_Fetch(string pollQuestion)
		{
			var now = DateTime.UtcNow;
			var stringPattern = $"%{pollQuestion.ToLower()}%";

			var polls = this.Entities.Mvpoll
				.Where(this.SearchWhereClause.WhereClause(now, stringPattern));

			var results = (from poll in polls
								join category in this.Entities.Mvcategory on poll.PollCategoryId equals category.CategoryId
								orderby category.CategoryName ascending
								join popular in
									(from submission in this.Entities.MvpollSubmission
									 group submission by submission.PollId into submissionCount
									 select new
									 {
										 PollId = submissionCount.Key,
										 Count = submissionCount.Count()
									 }) on poll.PollId equals popular.PollId into pollCounts
								from pollCount in pollCounts.DefaultIfEmpty(new { PollId = poll.PollId, Count = 0 })
								orderby pollCount.Count descending
								select new PollSearchResultsData
								{
									Category = category.CategoryName,
									Id = poll.PollId,
									ImageLink = poll.PollImageLink,
									Question = poll.PollQuestion,
									SubmissionCount = pollCount.Count
								}).Take(PollSearchResults.MaximumResultCount).ToList();

			this.ProcessQueryResults(results);
		}

		private void DataPortal_Fetch(PollSearchResultsQueryType criteria)
		{
			var now = DateTime.UtcNow;

			if (criteria == PollSearchResultsQueryType.MostPopular)
			{
				this.ProcessQueryResults(
					(from result in
						 ((from poll in this.Entities.Mvpoll
							where (poll.PollStartDate < now && poll.PollEndDate > now &&
								(poll.PollDeletedFlag != (bool?)true))
							join category in this.Entities.Mvcategory on poll.PollCategoryId equals category.CategoryId
							orderby category.CategoryName ascending
							join popular in
								(from submission in this.Entities.MvpollSubmission
								 group submission by submission.PollId into submissionCount
								 select new
								 {
									 PollId = submissionCount.Key,
									 Count = submissionCount.Count()
								 }) on poll.PollId equals popular.PollId into pollCounts
							from pollCount in pollCounts.DefaultIfEmpty(new { PollId = poll.PollId, Count = 0 })
							orderby pollCount.Count descending
							select new
							{
								Category = category.CategoryName,
								Id = poll.PollId,
								ImageLink = poll.PollImageLink,
								Question = poll.PollQuestion,
								Count = pollCount
							}).Take(PollSearchResults.MaximumResultCount).ToList())
					 select new PollSearchResultsData
					 {
						 Category = result.Category,
						 Id = result.Id,
						 ImageLink = result.ImageLink,
						 Question = result.Question,
						 SubmissionCount = result.Count.Count
					 }).ToList());
			}
			else if (criteria == PollSearchResultsQueryType.Newest)
			{
				this.ProcessQueryResults(
					(from result in
						 ((from poll in this.Entities.Mvpoll
							where (poll.PollStartDate < now && poll.PollEndDate > now &&
								(poll.PollDeletedFlag != (bool?)true))
							join category in this.Entities.Mvcategory on poll.PollCategoryId equals category.CategoryId
							orderby category.CategoryName ascending
							join counts in
								(from submission in this.Entities.MvpollSubmission
								 group submission by submission.PollId into submissionCount
								 select new
								 {
									 PollId = submissionCount.Key,
									 Count = submissionCount.Count()
								 }) on poll.PollId equals counts.PollId into pollCounts
							from pollCount in pollCounts.DefaultIfEmpty(new { PollId = poll.PollId, Count = 0 })
							orderby poll.PollStartDate descending
							select new
							{
								Category = category.CategoryName,
								Id = poll.PollId,
								ImageLink = poll.PollImageLink,
								Question = poll.PollQuestion,
								Count = pollCount
							}).Take(PollSearchResults.MaximumResultCount).ToList())
					 select new PollSearchResultsData
					 {
						 Category = result.Category,
						 Id = result.Id,
						 ImageLink = result.ImageLink,
						 Question = result.Question,
						 SubmissionCount = result.Count.Count
					 }).ToList());
			}
			else
			{
				throw new ArgumentException(nameof(criteria));
			}
		}

		[SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode")]
		private void DataPortal_Fetch(PollSearchResultsByUserCriteria criteria)
		{
			var now = DateTime.UtcNow;

			if (criteria.ArePollsActive)
			{
				this.ProcessQueryResults(
					(from result in
						 ((from poll in this.Entities.Mvpoll
							where (poll.PollStartDate < now && poll.PollEndDate > now &&
								poll.UserId == criteria.UserID &&
								(poll.PollDeletedFlag != (bool?)true))
							join category in this.Entities.Mvcategory on poll.PollCategoryId equals category.CategoryId
							orderby category.CategoryName ascending
							join counts in
								(from submission in this.Entities.MvpollSubmission
								 group submission by submission.PollId into submissionCount
								 select new
								 {
									 PollId = submissionCount.Key,
									 Count = submissionCount.Count()
								 }) on poll.PollId equals counts.PollId into pollCounts
							from pollCount in pollCounts.DefaultIfEmpty(new { PollId = poll.PollId, Count = 0 })
							select new
							{
								Category = category.CategoryName,
								Id = poll.PollId,
								ImageLink = poll.PollImageLink,
								Question = poll.PollQuestion,
								Count = pollCount
							}).Take(PollSearchResults.MaximumResultCount).ToList())
					 select new PollSearchResultsData
					 {
						 Category = result.Category,
						 Id = result.Id,
						 ImageLink = result.ImageLink,
						 Question = result.Question,
						 SubmissionCount = result.Count.Count
					 }).ToList());
			}
			else
			{
				this.ProcessQueryResults(
					(from result in
						 ((from poll in this.Entities.Mvpoll
							where (poll.UserId == criteria.UserID &&
								(poll.PollDeletedFlag != (bool?)true))
							where poll.PollEndDate < now
							join category in this.Entities.Mvcategory on poll.PollCategoryId equals category.CategoryId
							orderby category.CategoryName ascending
							join counts in
								(from submission in this.Entities.MvpollSubmission
								 group submission by submission.PollId into submissionCount
								 select new
								 {
									 PollId = submissionCount.Key,
									 Count = submissionCount.Count()
								 }) on poll.PollId equals counts.PollId into pollCounts
							from pollCount in pollCounts.DefaultIfEmpty(new { PollId = poll.PollId, Count = 0 })
							select new
							{
								Category = category.CategoryName,
								Id = poll.PollId,
								ImageLink = poll.PollImageLink,
								Question = poll.PollQuestion,
								Count = pollCount
							}).Take(PollSearchResults.MaximumResultCount).ToList())
					 select new PollSearchResultsData
					 {
						 Category = result.Category,
						 Id = result.Id,
						 ImageLink = result.ImageLink,
						 Question = result.Question,
						 SubmissionCount = result.Count.Count
					 }).ToList());
			}
		}

		private void ProcessQueryResults(List<PollSearchResultsData> results)
		{
			var resultList = this.pollSearchResultsByCategoryFactory.FetchChild();
			resultList.SwitchReadOnlyStatus();

			var pollsByCategories = new Dictionary<string, List<PollSearchResultsData>>();

			foreach (var result in results)
			{
				List<PollSearchResultsData> pollsByCategory = null;

				if (pollsByCategories.ContainsKey(result.Category))
				{
					pollsByCategory = pollsByCategories[result.Category];
				}
				else
				{
					pollsByCategory = new List<PollSearchResultsData>();
					pollsByCategories.Add(result.Category, pollsByCategory);
				}

				pollsByCategory.Add(result);
			}

			foreach (var pollDataPair in pollsByCategories)
			{
				resultList.Add(this.pollSearchResultByCategoryFactory.FetchChild(pollDataPair.Value));
			}

			resultList.SwitchReadOnlyStatus();
			this.SearchResultsByCategory = resultList;
		}

		[NonSerialized]
		private IObjectFactory<ReadOnlySwitchList<IPollSearchResultsByCategory>> pollSearchResultsByCategoryFactory;
		[Dependency]
		public IObjectFactory<ReadOnlySwitchList<IPollSearchResultsByCategory>> PollSearchResultsByCategoryFactory
		{
			get { return this.pollSearchResultsByCategoryFactory; }
			set { this.pollSearchResultsByCategoryFactory = value; }
		}

		[NonSerialized]
		private IObjectFactory<IPollSearchResultsByCategory> pollSearchResultByCategoryFactory;
		[Dependency]
		public IObjectFactory<IPollSearchResultsByCategory> PollSearchResultByCategoryFactory
		{
			get { return this.pollSearchResultByCategoryFactory; }
			set { this.pollSearchResultByCategoryFactory = value; }
		}
#endif

		public static readonly PropertyInfo<ReadOnlySwitchList<IPollSearchResultsByCategory>> SearchResultsByCategoryProperty =
			PollSearchResults.RegisterProperty<ReadOnlySwitchList<IPollSearchResultsByCategory>>(_ => _.SearchResultsByCategory);
		public IReadOnlyListBaseCore<IPollSearchResultsByCategory> SearchResultsByCategory
		{
			get { return this.ReadProperty(PollSearchResults.SearchResultsByCategoryProperty); }
			private set { this.LoadProperty(PollSearchResults.SearchResultsByCategoryProperty, value); }
		}

#if !NETFX_CORE && !MOBILE
		[NonSerialized]
		private ISearchWhereClause searchWhereClause;
		[Dependency]
		public ISearchWhereClause SearchWhereClause
		{
			get { return this.searchWhereClause; }
			set { this.searchWhereClause = value; }
		}
#endif
	}
}