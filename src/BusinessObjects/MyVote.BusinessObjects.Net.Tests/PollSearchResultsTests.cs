using Autofac;
using Csla;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;
using MyVote.Data.Entities;
using Spackle.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MyVote.BusinessObjects.Net.Tests
{
	[TestClass]
	public sealed class PollSearchResultsTests
	{
		[TestMethod]
		public void FetchMostPopular()
		{
			var now = DateTime.UtcNow;
			var category1 = new MVCategory { CategoryID = 1, CategoryName = "1" };

			var poll1 = EntityCreator.Create<MVPoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollCategoryID = category1.CategoryID;
				_.PollEndDate = now.AddDays(2);
				_.PollStartDate = now.AddDays(-2);
			});
			var poll2 = EntityCreator.Create<MVPoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollCategoryID = category1.CategoryID;
				_.PollEndDate = now.AddDays(2);
				_.PollStartDate = now.AddDays(-2);
			});
			var poll3 = EntityCreator.Create<MVPoll>(_ =>
			{
				_.PollDeletedFlag = true;
				_.PollCategoryID = category1.CategoryID;
				_.PollEndDate = now.AddDays(2);
				_.PollStartDate = now.AddDays(-2);
			});
			var poll4 = EntityCreator.Create<MVPoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollCategoryID = category1.CategoryID;
				_.PollEndDate = now.AddDays(2);
				_.PollStartDate = now.AddDays(-2);
			});

			var submission1 = EntityCreator.Create<MVPollSubmission>(_ =>
			{
				_.PollID = poll2.PollID;
			});
			var submission2 = EntityCreator.Create<MVPollSubmission>(_ =>
			{
				_.PollID = poll2.PollID;
			});
			var submission3 = EntityCreator.Create<MVPollSubmission>(_ =>
			{
				_.PollID = poll1.PollID;
			});
			var submission4 = EntityCreator.Create<MVPollSubmission>(_ =>
			{
				_.PollID = poll2.PollID;
			});

			var entities = new Mock<IEntities>(MockBehavior.Strict);
			entities.Setup(_ => _.MVCategories).Returns(new InMemoryDbSet<MVCategory> { category1 });
			entities.Setup(_ => _.MVPolls).Returns(new InMemoryDbSet<MVPoll> { poll1, poll2, poll3, poll4 });
			entities.Setup(_ => _.MVPollSubmissions).Returns(new InMemoryDbSet<MVPollSubmission> { submission1, submission2, submission3, submission4 });
			entities.Setup(_ => _.Dispose());

			var pollSearchResultsByCategoryFactory = new Mock<IObjectFactory<ReadOnlySwitchList<IPollSearchResultsByCategory>>>(MockBehavior.Strict);
			pollSearchResultsByCategoryFactory.Setup(_ => _.FetchChild()).Returns(DataPortal.FetchChild<ReadOnlySwitchList<IPollSearchResultsByCategory>>());

			var pollSearchResultByCategoryFactory = new Mock<IObjectFactory<IPollSearchResultsByCategory>>(MockBehavior.Strict);
			pollSearchResultByCategoryFactory.Setup(_ => _.FetchChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.FetchChild<PollSearchResultsByCategory>(_[0] as List<PollSearchResultsData>));

			var pollSearchResultsFactory = new Mock<IObjectFactory<ReadOnlySwitchList<IPollSearchResult>>>();
			pollSearchResultsFactory.Setup(_ => _.FetchChild()).Returns(() => DataPortal.FetchChild<ReadOnlySwitchList<IPollSearchResult>>());

			var pollSearchResultFactory = new Mock<IObjectFactory<IPollSearchResult>>();
			pollSearchResultFactory.Setup(_ => _.FetchChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.FetchChild<PollSearchResult>(_[0] as PollSearchResultsData));

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => entities.Object);
			builder.Register<ISearchWhereClause>(_ => Mock.Of<ISearchWhereClause>());
			builder.Register<IObjectFactory<ReadOnlySwitchList<IPollSearchResultsByCategory>>>(_ => pollSearchResultsByCategoryFactory.Object);
			builder.Register<IObjectFactory<IPollSearchResultsByCategory>>(_ => pollSearchResultByCategoryFactory.Object);
			builder.Register<IObjectFactory<ReadOnlySwitchList<IPollSearchResult>>>(_ => pollSearchResultsFactory.Object);
			builder.Register<IObjectFactory<IPollSearchResult>>(_ => pollSearchResultFactory.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var result = DataPortal.Fetch<PollSearchResults>(PollSearchResultsQueryType.MostPopular);
				Assert.AreEqual(1, result.SearchResultsByCategory.Count,
					nameof(result.SearchResultsByCategory));
				Assert.AreEqual(3, result.SearchResultsByCategory[0].SearchResults.Count,
					nameof(IPollSearchResultsByCategory.SearchResults));
				Assert.AreEqual(poll2.PollQuestion, result.SearchResultsByCategory[0].SearchResults[0].Question,
					$"{nameof(IPollSearchResult.Question)} 0");
				Assert.AreEqual(poll1.PollQuestion, result.SearchResultsByCategory[0].SearchResults[1].Question,
					$"{nameof(IPollSearchResult.Question)} 1");
			}

			entities.VerifyAll();
			pollSearchResultsByCategoryFactory.VerifyAll();
			pollSearchResultByCategoryFactory.VerifyAll();
		}

		[TestMethod]
		public void FetchByNewest()
		{
			var now = DateTime.UtcNow;
			var category1 = new MVCategory { CategoryID = 1, CategoryName = "b" };
			var category2 = new MVCategory { CategoryID = 2, CategoryName = "a" };

			var poll1 = EntityCreator.Create<MVPoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollEndDate = now.AddDays(2);
				_.PollStartDate = now.AddDays(-2);
				_.PollCategoryID = 1;
			});
			var poll2 = EntityCreator.Create<MVPoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollEndDate = now.AddDays(2);
				_.PollStartDate = now.AddDays(-2);
				_.PollCategoryID = 2;
			});
			var poll3 = EntityCreator.Create<MVPoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollEndDate = now.AddDays(2);
				_.PollStartDate = now.AddDays(-4);
				_.PollCategoryID = 1;
			});
			var poll4 = EntityCreator.Create<MVPoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollEndDate = now.AddDays(2);
				_.PollStartDate = now.AddDays(-4);
				_.PollCategoryID = 2;
			});
			var poll5 = EntityCreator.Create<MVPoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollEndDate = now.AddDays(2);
				_.PollStartDate = now.AddDays(-1);
				_.PollCategoryID = 1;
			});
			var poll6 = EntityCreator.Create<MVPoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollEndDate = now.AddDays(2);
				_.PollStartDate = now.AddDays(-1);
				_.PollCategoryID = 2;
			});
			var poll7 = EntityCreator.Create<MVPoll>(_ =>
			{
				_.PollDeletedFlag = true;
				_.PollEndDate = now.AddDays(2);
				_.PollStartDate = now.AddDays(-1);
				_.PollCategoryID = 2;
			});

			var entities = new Mock<IEntities>(MockBehavior.Strict);
			entities.Setup(_ => _.MVCategories).Returns(new InMemoryDbSet<MVCategory> { category1, category2 });
			entities.Setup(_ => _.MVPolls).Returns(new InMemoryDbSet<MVPoll> { poll1, poll2, poll3, poll4, poll5, poll6, poll7 });
			entities.Setup(_ => _.MVPollSubmissions).Returns(new InMemoryDbSet<MVPollSubmission>());
			entities.Setup(_ => _.Dispose());

			var pollSearchResultsByCategoryFactory = new Mock<IObjectFactory<ReadOnlySwitchList<IPollSearchResultsByCategory>>>();
			pollSearchResultsByCategoryFactory.Setup(_ => _.FetchChild()).Returns(DataPortal.FetchChild<ReadOnlySwitchList<IPollSearchResultsByCategory>>());

			var pollSearchResultByCategoryFactory = new Mock<IObjectFactory<IPollSearchResultsByCategory>>();
			pollSearchResultByCategoryFactory.Setup(_ => _.FetchChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.FetchChild<PollSearchResultsByCategory>(_[0] as List<PollSearchResultsData>));

			var pollSearchResultsFactory = new Mock<IObjectFactory<ReadOnlySwitchList<IPollSearchResult>>>();
			pollSearchResultsFactory.Setup(_ => _.FetchChild()).Returns(() => DataPortal.FetchChild<ReadOnlySwitchList<IPollSearchResult>>());

			var pollSearchResultFactory = new Mock<IObjectFactory<IPollSearchResult>>();
			pollSearchResultFactory.Setup(_ => _.FetchChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.FetchChild<PollSearchResult>(_[0] as PollSearchResultsData));

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => entities.Object);
			builder.Register<ISearchWhereClause>(_ => Mock.Of<ISearchWhereClause>());
			builder.Register<IObjectFactory<ReadOnlySwitchList<IPollSearchResultsByCategory>>>(_ => pollSearchResultsByCategoryFactory.Object);
			builder.Register<IObjectFactory<IPollSearchResultsByCategory>>(_ => pollSearchResultByCategoryFactory.Object);
			builder.Register<IObjectFactory<ReadOnlySwitchList<IPollSearchResult>>>(_ => pollSearchResultsFactory.Object);
			builder.Register<IObjectFactory<IPollSearchResult>>(_ => pollSearchResultFactory.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var result = DataPortal.Fetch<PollSearchResults>(PollSearchResultsQueryType.Newest);
				Assert.AreEqual(2, result.SearchResultsByCategory.Count,
					nameof(result.SearchResultsByCategory));

				var firstCategory = result.SearchResultsByCategory[0];
				Assert.AreEqual("a", firstCategory.Category,
					$"{nameof(firstCategory.Category)} a");
				Assert.AreEqual(3, firstCategory.SearchResults.Count,
					$"{nameof(firstCategory.SearchResults)} a");
				Assert.AreEqual(poll6.PollQuestion, firstCategory.SearchResults[0].Question,
					$"{nameof(IPollSearchResult.Question)} a 0");
				Assert.AreEqual(poll2.PollQuestion, firstCategory.SearchResults[1].Question,
					$"{nameof(IPollSearchResult.Question)} a 1");
				Assert.AreEqual(poll4.PollQuestion, firstCategory.SearchResults[2].Question,
					$"{nameof(IPollSearchResult.Question)} a 2");

				var secondCategory = result.SearchResultsByCategory[1];
				Assert.AreEqual("b", secondCategory.Category,
					$"{nameof(secondCategory.Category)} b");
				Assert.AreEqual(3, secondCategory.SearchResults.Count,
					$"{nameof(secondCategory.SearchResults)} b");
				Assert.AreEqual(poll5.PollQuestion, secondCategory.SearchResults[0].Question,
					$"{nameof(IPollSearchResult.Question)} b 0");
				Assert.AreEqual(poll1.PollQuestion, secondCategory.SearchResults[1].Question,
					$"{nameof(IPollSearchResult.Question)} b 1");
				Assert.AreEqual(poll3.PollQuestion, secondCategory.SearchResults[2].Question,
					nameof(IPollSearchResult.Question));
			}

			entities.VerifyAll();
		}

		[TestMethod]
		public void FetchByPollQuestion()
		{
			var now = DateTime.UtcNow;
			var category1 = new MVCategory { CategoryID = 1, CategoryName = "1" };

			var poll1 = EntityCreator.Create<MVPoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollCategoryID = category1.CategoryID;
				_.PollQuestion = "12345";
				_.PollEndDate = now.AddDays(2);
				_.PollStartDate = now.AddDays(-2);
			});
			var poll2 = EntityCreator.Create<MVPoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollCategoryID = category1.CategoryID;
				_.PollQuestion = "AbCdE";
				_.PollEndDate = now.AddDays(2);
				_.PollStartDate = now.AddDays(-2);
			});
			var poll3 = EntityCreator.Create<MVPoll>(_ =>
			{
				_.PollDeletedFlag = true;
				_.PollCategoryID = category1.CategoryID;
				_.PollQuestion = "DeFgH";
				_.PollEndDate = now.AddDays(2);
				_.PollStartDate = now.AddDays(-2);
			});

			var submission1 = EntityCreator.Create<MVPollSubmission>(_ =>
			{
				_.PollID = poll2.PollID;
			});
			var submission2 = EntityCreator.Create<MVPollSubmission>(_ =>
			{
				_.PollID = poll2.PollID;
			});
			var submission3 = EntityCreator.Create<MVPollSubmission>(_ =>
			{
				_.PollID = poll1.PollID;
			});
			var submission4 = EntityCreator.Create<MVPollSubmission>(_ =>
			{
				_.PollID = poll2.PollID;
			});

			var entities = new Mock<IEntities>(MockBehavior.Strict);
			entities.Setup(_ => _.MVCategories).Returns(new InMemoryDbSet<MVCategory> { category1 });
			entities.Setup(_ => _.MVPolls).Returns(new InMemoryDbSet<MVPoll> { poll1, poll2, poll3 });
			entities.Setup(_ => _.MVPollSubmissions).Returns(new InMemoryDbSet<MVPollSubmission> { submission1, submission2, submission3, submission4 });
			entities.Setup(_ => _.Dispose());

			var searchWhereClause = new Mock<ISearchWhereClause>(MockBehavior.Strict);
			searchWhereClause.Setup(_ => _.WhereClause(It.IsAny<DateTime>(), "%bcd%")).Returns(_ => _.PollQuestion == "AbCdE");

			var pollSearchResultsByCategoryFactory = new Mock<IObjectFactory<ReadOnlySwitchList<IPollSearchResultsByCategory>>>();
			pollSearchResultsByCategoryFactory.Setup(_ => _.FetchChild()).Returns(DataPortal.FetchChild<ReadOnlySwitchList<IPollSearchResultsByCategory>>());

			var pollSearchResultByCategoryFactory = new Mock<IObjectFactory<IPollSearchResultsByCategory>>();
			pollSearchResultByCategoryFactory.Setup(_ => _.FetchChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.FetchChild<PollSearchResultsByCategory>(_[0] as List<PollSearchResultsData>));

			var pollSearchResultsFactory = new Mock<IObjectFactory<ReadOnlySwitchList<IPollSearchResult>>>();
			pollSearchResultsFactory.Setup(_ => _.FetchChild()).Returns(() => DataPortal.FetchChild<ReadOnlySwitchList<IPollSearchResult>>());

			var pollSearchResultFactory = new Mock<IObjectFactory<IPollSearchResult>>();
			pollSearchResultFactory.Setup(_ => _.FetchChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.FetchChild<PollSearchResult>(_[0] as PollSearchResultsData));

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => entities.Object);
			builder.Register<ISearchWhereClause>(_ => searchWhereClause.Object);
			builder.Register<IObjectFactory<ReadOnlySwitchList<IPollSearchResultsByCategory>>>(_ => pollSearchResultsByCategoryFactory.Object);
			builder.Register<IObjectFactory<IPollSearchResultsByCategory>>(_ => pollSearchResultByCategoryFactory.Object);
			builder.Register<IObjectFactory<ReadOnlySwitchList<IPollSearchResult>>>(_ => pollSearchResultsFactory.Object);
			builder.Register<IObjectFactory<IPollSearchResult>>(_ => pollSearchResultFactory.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var result = DataPortal.Fetch<PollSearchResults>("bCd");
				Assert.AreEqual(1, result.SearchResultsByCategory.Count,
					nameof(result.SearchResultsByCategory));
				Assert.AreEqual(1, result.SearchResultsByCategory[0].SearchResults.Count,
					nameof(ICollection.Count));
				Assert.AreEqual(poll2.PollQuestion, result.SearchResultsByCategory[0].SearchResults[0].Question,
					nameof(IPollSearchResult.Question));
			}

			searchWhereClause.VerifyAll();
			entities.VerifyAll();
		}

		[TestMethod]
		public void FetchByUserForPollsThatAreActive()
		{
			var now = DateTime.UtcNow;

			var category1 = new MVCategory { CategoryID = 1, CategoryName = "1" };
			var category2 = new MVCategory { CategoryID = 2, CategoryName = "2" };

			var poll1 = EntityCreator.Create<MVPoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollStartDate = now.AddDays(-2);
				_.PollEndDate = now.AddDays(2);
				_.PollCategoryID = 1;
				_.UserID = 1;
			});
			var poll2 = EntityCreator.Create<MVPoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollStartDate = now.AddDays(-3);
				_.PollEndDate = DateTime.UtcNow.AddDays(-2);
				_.UserID = 2;
			});
			var poll3 = EntityCreator.Create<MVPoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollStartDate = now.AddDays(-2);
				_.PollEndDate = DateTime.UtcNow.AddDays(2);
				_.PollCategoryID = 2;
				_.UserID = 1;
			});
			var poll4 = EntityCreator.Create<MVPoll>(_ =>
			{
				_.PollDeletedFlag = true;
				_.PollStartDate = now.AddDays(-2);
				_.PollEndDate = DateTime.UtcNow.AddDays(2);
				_.PollCategoryID = 2;
				_.UserID = 1;
			});

			var entities = new Mock<IEntities>(MockBehavior.Strict);
			entities.Setup(_ => _.MVCategories).Returns(new InMemoryDbSet<MVCategory> { category1, category2 });
			entities.Setup(_ => _.MVPolls).Returns(new InMemoryDbSet<MVPoll> { poll1, poll2, poll3, poll4 });
			entities.Setup(_ => _.MVPollSubmissions).Returns(new InMemoryDbSet<MVPollSubmission>());
			entities.Setup(_ => _.Dispose());

			var pollSearchResultsByCategoryFactory = new Mock<IObjectFactory<ReadOnlySwitchList<IPollSearchResultsByCategory>>>();
			pollSearchResultsByCategoryFactory.Setup(_ => _.FetchChild()).Returns(DataPortal.FetchChild<ReadOnlySwitchList<IPollSearchResultsByCategory>>());

			var pollSearchResultByCategoryFactory = new Mock<IObjectFactory<IPollSearchResultsByCategory>>();
			pollSearchResultByCategoryFactory.Setup(_ => _.FetchChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.FetchChild<PollSearchResultsByCategory>(_[0] as List<PollSearchResultsData>));

			var pollSearchResultsFactory = new Mock<IObjectFactory<ReadOnlySwitchList<IPollSearchResult>>>();
			pollSearchResultsFactory.Setup(_ => _.FetchChild()).Returns(() => DataPortal.FetchChild<ReadOnlySwitchList<IPollSearchResult>>());

			var pollSearchResultFactory = new Mock<IObjectFactory<IPollSearchResult>>();
			pollSearchResultFactory.Setup(_ => _.FetchChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.FetchChild<PollSearchResult>(_[0] as PollSearchResultsData));

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => entities.Object);
			builder.Register<ISearchWhereClause>(_ => Mock.Of<ISearchWhereClause>());
			builder.Register<IObjectFactory<ReadOnlySwitchList<IPollSearchResultsByCategory>>>(_ => pollSearchResultsByCategoryFactory.Object);
			builder.Register<IObjectFactory<IPollSearchResultsByCategory>>(_ => pollSearchResultByCategoryFactory.Object);
			builder.Register<IObjectFactory<ReadOnlySwitchList<IPollSearchResult>>>(_ => pollSearchResultsFactory.Object);
			builder.Register<IObjectFactory<IPollSearchResult>>(_ => pollSearchResultFactory.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var result = DataPortal.Fetch<PollSearchResults>(
					new PollSearchResultsByUserCriteria(1, true));
				Assert.AreEqual(2, result.SearchResultsByCategory.Count,
					nameof(result.SearchResultsByCategory));
			}

			entities.VerifyAll();
		}

		[TestMethod]
		public void FetchByUserForPollsThatAreNotActive()
		{
			var now = DateTime.UtcNow;

			var category1 = new MVCategory { CategoryID = 1, CategoryName = "1" };
			var category2 = new MVCategory { CategoryID = 2, CategoryName = "2" };

			var poll1 = EntityCreator.Create<MVPoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollStartDate = now.AddDays(-4);
				_.PollEndDate = DateTime.UtcNow.AddDays(-2);
				_.PollCategoryID = 1;
				_.UserID = 1;
			});
			var poll2 = EntityCreator.Create<MVPoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollStartDate = now.AddDays(-2);
				_.PollEndDate = DateTime.UtcNow.AddDays(2);
				_.UserID = 1;
			});
			var poll3 = EntityCreator.Create<MVPoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollStartDate = now.AddDays(-4);
				_.PollEndDate = DateTime.UtcNow.AddDays(-2);
				_.PollCategoryID = 2;
				_.UserID = 1;
			});
			var poll4 = EntityCreator.Create<MVPoll>(_ =>
			{
				_.PollDeletedFlag = true;
				_.PollStartDate = now.AddDays(-4);
				_.PollEndDate = DateTime.UtcNow.AddDays(-2);
				_.PollCategoryID = 2;
				_.UserID = 1;
			});

			var entities = new Mock<IEntities>(MockBehavior.Strict);
			entities.Setup(_ => _.MVCategories).Returns(new InMemoryDbSet<MVCategory> { category1, category2 });
			entities.Setup(_ => _.MVPolls).Returns(new InMemoryDbSet<MVPoll> { poll1, poll2, poll3, poll4 });
			entities.Setup(_ => _.MVPollSubmissions).Returns(new InMemoryDbSet<MVPollSubmission>());
			entities.Setup(_ => _.Dispose());

			var pollSearchResultsByCategoryFactory = new Mock<IObjectFactory<ReadOnlySwitchList<IPollSearchResultsByCategory>>>();
			pollSearchResultsByCategoryFactory.Setup(_ => _.FetchChild()).Returns(DataPortal.FetchChild<ReadOnlySwitchList<IPollSearchResultsByCategory>>());

			var pollSearchResultByCategoryFactory = new Mock<IObjectFactory<IPollSearchResultsByCategory>>();
			pollSearchResultByCategoryFactory.Setup(_ => _.FetchChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.FetchChild<PollSearchResultsByCategory>(_[0] as List<PollSearchResultsData>));

			var pollSearchResultsFactory = new Mock<IObjectFactory<ReadOnlySwitchList<IPollSearchResult>>>();
			pollSearchResultsFactory.Setup(_ => _.FetchChild()).Returns(() => DataPortal.FetchChild<ReadOnlySwitchList<IPollSearchResult>>());

			var pollSearchResultFactory = new Mock<IObjectFactory<IPollSearchResult>>();
			pollSearchResultFactory.Setup(_ => _.FetchChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.FetchChild<PollSearchResult>(_[0] as PollSearchResultsData));

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => entities.Object);
			builder.Register<ISearchWhereClause>(_ => Mock.Of<ISearchWhereClause>());
			builder.Register<IObjectFactory<ReadOnlySwitchList<IPollSearchResultsByCategory>>>(_ => pollSearchResultsByCategoryFactory.Object);
			builder.Register<IObjectFactory<IPollSearchResultsByCategory>>(_ => pollSearchResultByCategoryFactory.Object);
			builder.Register<IObjectFactory<ReadOnlySwitchList<IPollSearchResult>>>(_ => pollSearchResultsFactory.Object);
			builder.Register<IObjectFactory<IPollSearchResult>>(_ => pollSearchResultFactory.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var result = DataPortal.Fetch<PollSearchResults>(
					new PollSearchResultsByUserCriteria(1, false));
				Assert.AreEqual(2, result.SearchResultsByCategory.Count,
					nameof(result.SearchResultsByCategory));
			}

			entities.VerifyAll();
		}
	}
}
