using Autofac;
using Csla;
using FluentAssertions;
using Moq;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;
using MyVote.Data.Entities;
using Spackle.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace MyVote.BusinessObjects.Tests
{
	public sealed class PollSearchResultsTests
	{
		[Fact]
		public void FetchMostPopular()
		{
			var now = DateTime.UtcNow;
			var category1 = new Mvcategory { CategoryId = 1, CategoryName = "1" };

			var poll1 = EntityCreator.Create<Mvpoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollCategoryId = category1.CategoryId;
				_.PollEndDate = now.AddDays(2);
				_.PollStartDate = now.AddDays(-2);
			});
			var poll2 = EntityCreator.Create<Mvpoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollCategoryId = category1.CategoryId;
				_.PollEndDate = now.AddDays(2);
				_.PollStartDate = now.AddDays(-2);
			});
			var poll3 = EntityCreator.Create<Mvpoll>(_ =>
			{
				_.PollDeletedFlag = true;
				_.PollCategoryId = category1.CategoryId;
				_.PollEndDate = now.AddDays(2);
				_.PollStartDate = now.AddDays(-2);
			});
			var poll4 = EntityCreator.Create<Mvpoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollCategoryId = category1.CategoryId;
				_.PollEndDate = now.AddDays(2);
				_.PollStartDate = now.AddDays(-2);
			});

			var submission1 = EntityCreator.Create<MvpollSubmission>(_ =>
			{
				_.PollId = poll2.PollId;
			});
			var submission2 = EntityCreator.Create<MvpollSubmission>(_ =>
			{
				_.PollId = poll2.PollId;
			});
			var submission3 = EntityCreator.Create<MvpollSubmission>(_ =>
			{
				_.PollId = poll1.PollId;
			});
			var submission4 = EntityCreator.Create<MvpollSubmission>(_ =>
			{
				_.PollId = poll2.PollId;
			});

			var entities = new Mock<IEntitiesContext>(MockBehavior.Strict);
			entities.Setup(_ => _.Mvcategory).Returns(new InMemoryDbSet<Mvcategory> { category1 });
			entities.Setup(_ => _.Mvpoll).Returns(new InMemoryDbSet<Mvpoll> { poll1, poll2, poll3, poll4 });
			entities.Setup(_ => _.MvpollSubmission).Returns(new InMemoryDbSet<MvpollSubmission> { submission1, submission2, submission3, submission4 });
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
			builder.Register<IEntitiesContext>(_ => entities.Object);
			builder.Register<ISearchWhereClause>(_ => Mock.Of<ISearchWhereClause>());
			builder.Register<IObjectFactory<ReadOnlySwitchList<IPollSearchResultsByCategory>>>(_ => pollSearchResultsByCategoryFactory.Object);
			builder.Register<IObjectFactory<IPollSearchResultsByCategory>>(_ => pollSearchResultByCategoryFactory.Object);
			builder.Register<IObjectFactory<ReadOnlySwitchList<IPollSearchResult>>>(_ => pollSearchResultsFactory.Object);
			builder.Register<IObjectFactory<IPollSearchResult>>(_ => pollSearchResultFactory.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var result = DataPortal.Fetch<PollSearchResults>(PollSearchResultsQueryType.MostPopular);

				result.SearchResultsByCategory.Count.Should().Be(1);
				result.SearchResultsByCategory[0].SearchResults.Count.Should().Be(3);
				result.SearchResultsByCategory[0].SearchResults[0].Question.Should().Be(poll2.PollQuestion);
				result.SearchResultsByCategory[0].SearchResults[1].Question.Should().Be(poll1.PollQuestion);
			}

			entities.VerifyAll();
			pollSearchResultsByCategoryFactory.VerifyAll();
			pollSearchResultByCategoryFactory.VerifyAll();
		}

		[Fact]
		public void FetchByNewest()
		{
			var now = DateTime.UtcNow;
			var category1 = new Mvcategory { CategoryId = 1, CategoryName = "b" };
			var category2 = new Mvcategory { CategoryId = 2, CategoryName = "a" };

			var poll1 = EntityCreator.Create<Mvpoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollEndDate = now.AddDays(2);
				_.PollStartDate = now.AddDays(-2);
				_.PollCategoryId = 1;
			});
			var poll2 = EntityCreator.Create<Mvpoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollEndDate = now.AddDays(2);
				_.PollStartDate = now.AddDays(-2);
				_.PollCategoryId = 2;
			});
			var poll3 = EntityCreator.Create<Mvpoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollEndDate = now.AddDays(2);
				_.PollStartDate = now.AddDays(-4);
				_.PollCategoryId = 1;
			});
			var poll4 = EntityCreator.Create<Mvpoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollEndDate = now.AddDays(2);
				_.PollStartDate = now.AddDays(-4);
				_.PollCategoryId = 2;
			});
			var poll5 = EntityCreator.Create<Mvpoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollEndDate = now.AddDays(2);
				_.PollStartDate = now.AddDays(-1);
				_.PollCategoryId = 1;
			});
			var poll6 = EntityCreator.Create<Mvpoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollEndDate = now.AddDays(2);
				_.PollStartDate = now.AddDays(-1);
				_.PollCategoryId = 2;
			});
			var poll7 = EntityCreator.Create<Mvpoll>(_ =>
			{
				_.PollDeletedFlag = true;
				_.PollEndDate = now.AddDays(2);
				_.PollStartDate = now.AddDays(-1);
				_.PollCategoryId = 2;
			});

			var entities = new Mock<IEntitiesContext>(MockBehavior.Strict);
			entities.Setup(_ => _.Mvcategory).Returns(new InMemoryDbSet<Mvcategory> { category1, category2 });
			entities.Setup(_ => _.Mvpoll).Returns(new InMemoryDbSet<Mvpoll> { poll1, poll2, poll3, poll4, poll5, poll6, poll7 });
			entities.Setup(_ => _.MvpollSubmission).Returns(new InMemoryDbSet<MvpollSubmission>());
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
			builder.Register<IEntitiesContext>(_ => entities.Object);
			builder.Register<ISearchWhereClause>(_ => Mock.Of<ISearchWhereClause>());
			builder.Register<IObjectFactory<ReadOnlySwitchList<IPollSearchResultsByCategory>>>(_ => pollSearchResultsByCategoryFactory.Object);
			builder.Register<IObjectFactory<IPollSearchResultsByCategory>>(_ => pollSearchResultByCategoryFactory.Object);
			builder.Register<IObjectFactory<ReadOnlySwitchList<IPollSearchResult>>>(_ => pollSearchResultsFactory.Object);
			builder.Register<IObjectFactory<IPollSearchResult>>(_ => pollSearchResultFactory.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var result = DataPortal.Fetch<PollSearchResults>(PollSearchResultsQueryType.Newest);

				result.SearchResultsByCategory.Count.Should().Be(2);

				var firstCategory = result.SearchResultsByCategory[0];
				firstCategory.Category.Should().Be("a");
				firstCategory.SearchResults.Count.Should().Be(3);
				firstCategory.SearchResults[0].Question.Should().Be(poll6.PollQuestion);
				firstCategory.SearchResults[1].Question.Should().Be(poll2.PollQuestion);
				firstCategory.SearchResults[2].Question.Should().Be(poll4.PollQuestion);

				var secondCategory = result.SearchResultsByCategory[1];
				secondCategory.Category.Should().Be("b");
				secondCategory.SearchResults.Count.Should().Be(3);
				secondCategory.SearchResults[0].Question.Should().Be(poll5.PollQuestion);
				secondCategory.SearchResults[1].Question.Should().Be(poll1.PollQuestion);
				secondCategory.SearchResults[2].Question.Should().Be(poll3.PollQuestion);
			}

			entities.VerifyAll();
		}

		[Fact]
		public void FetchByPollQuestion()
		{
			var now = DateTime.UtcNow;
			var category1 = new Mvcategory { CategoryId = 1, CategoryName = "1" };

			var poll1 = EntityCreator.Create<Mvpoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollCategoryId = category1.CategoryId;
				_.PollQuestion = "12345";
				_.PollEndDate = now.AddDays(2);
				_.PollStartDate = now.AddDays(-2);
			});
			var poll2 = EntityCreator.Create<Mvpoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollCategoryId = category1.CategoryId;
				_.PollQuestion = "AbCdE";
				_.PollEndDate = now.AddDays(2);
				_.PollStartDate = now.AddDays(-2);
			});
			var poll3 = EntityCreator.Create<Mvpoll>(_ =>
			{
				_.PollDeletedFlag = true;
				_.PollCategoryId = category1.CategoryId;
				_.PollQuestion = "DeFgH";
				_.PollEndDate = now.AddDays(2);
				_.PollStartDate = now.AddDays(-2);
			});

			var submission1 = EntityCreator.Create<MvpollSubmission>(_ =>
			{
				_.PollId = poll2.PollId;
			});
			var submission2 = EntityCreator.Create<MvpollSubmission>(_ =>
			{
				_.PollId = poll2.PollId;
			});
			var submission3 = EntityCreator.Create<MvpollSubmission>(_ =>
			{
				_.PollId = poll1.PollId;
			});
			var submission4 = EntityCreator.Create<MvpollSubmission>(_ =>
			{
				_.PollId = poll2.PollId;
			});

			var entities = new Mock<IEntitiesContext>(MockBehavior.Strict);
			entities.Setup(_ => _.Mvcategory).Returns(new InMemoryDbSet<Mvcategory> { category1 });
			entities.Setup(_ => _.Mvpoll).Returns(new InMemoryDbSet<Mvpoll> { poll1, poll2, poll3 });
			entities.Setup(_ => _.MvpollSubmission).Returns(new InMemoryDbSet<MvpollSubmission> { submission1, submission2, submission3, submission4 });
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
			builder.Register<IEntitiesContext>(_ => entities.Object);
			builder.Register<ISearchWhereClause>(_ => searchWhereClause.Object);
			builder.Register<IObjectFactory<ReadOnlySwitchList<IPollSearchResultsByCategory>>>(_ => pollSearchResultsByCategoryFactory.Object);
			builder.Register<IObjectFactory<IPollSearchResultsByCategory>>(_ => pollSearchResultByCategoryFactory.Object);
			builder.Register<IObjectFactory<ReadOnlySwitchList<IPollSearchResult>>>(_ => pollSearchResultsFactory.Object);
			builder.Register<IObjectFactory<IPollSearchResult>>(_ => pollSearchResultFactory.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var result = DataPortal.Fetch<PollSearchResults>("bCd");
				result.SearchResultsByCategory.Count.Should().Be(1);
				result.SearchResultsByCategory[0].SearchResults.Count.Should().Be(1);
				result.SearchResultsByCategory[0].SearchResults[0].Question.Should().Be(poll2.PollQuestion);
			}

			searchWhereClause.VerifyAll();
			entities.VerifyAll();
		}

		[Fact]
		public void FetchByUserForPollsThatAreActive()
		{
			var now = DateTime.UtcNow;

			var category1 = new Mvcategory { CategoryId = 1, CategoryName = "1" };
			var category2 = new Mvcategory { CategoryId = 2, CategoryName = "2" };

			var poll1 = EntityCreator.Create<Mvpoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollStartDate = now.AddDays(-2);
				_.PollEndDate = now.AddDays(2);
				_.PollCategoryId = 1;
				_.UserId = 1;
			});
			var poll2 = EntityCreator.Create<Mvpoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollStartDate = now.AddDays(-3);
				_.PollEndDate = DateTime.UtcNow.AddDays(-2);
				_.UserId = 2;
			});
			var poll3 = EntityCreator.Create<Mvpoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollStartDate = now.AddDays(-2);
				_.PollEndDate = DateTime.UtcNow.AddDays(2);
				_.PollCategoryId = 2;
				_.UserId = 1;
			});
			var poll4 = EntityCreator.Create<Mvpoll>(_ =>
			{
				_.PollDeletedFlag = true;
				_.PollStartDate = now.AddDays(-2);
				_.PollEndDate = DateTime.UtcNow.AddDays(2);
				_.PollCategoryId = 2;
				_.UserId = 1;
			});

			var entities = new Mock<IEntitiesContext>(MockBehavior.Strict);
			entities.Setup(_ => _.Mvcategory).Returns(new InMemoryDbSet<Mvcategory> { category1, category2 });
			entities.Setup(_ => _.Mvpoll).Returns(new InMemoryDbSet<Mvpoll> { poll1, poll2, poll3, poll4 });
			entities.Setup(_ => _.MvpollSubmission).Returns(new InMemoryDbSet<MvpollSubmission>());
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
			builder.Register<IEntitiesContext>(_ => entities.Object);
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
				result.SearchResultsByCategory.Count.Should().Be(2);
			}

			entities.VerifyAll();
		}

		[Fact]
		public void FetchByUserForPollsThatAreNotActive()
		{
			var now = DateTime.UtcNow;

			var category1 = new Mvcategory { CategoryId = 1, CategoryName = "1" };
			var category2 = new Mvcategory { CategoryId = 2, CategoryName = "2" };

			var poll1 = EntityCreator.Create<Mvpoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollStartDate = now.AddDays(-4);
				_.PollEndDate = DateTime.UtcNow.AddDays(-2);
				_.PollCategoryId = 1;
				_.UserId = 1;
			});
			var poll2 = EntityCreator.Create<Mvpoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollStartDate = now.AddDays(-2);
				_.PollEndDate = DateTime.UtcNow.AddDays(2);
				_.UserId = 1;
			});
			var poll3 = EntityCreator.Create<Mvpoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollStartDate = now.AddDays(-4);
				_.PollEndDate = DateTime.UtcNow.AddDays(-2);
				_.PollCategoryId = 2;
				_.UserId = 1;
			});
			var poll4 = EntityCreator.Create<Mvpoll>(_ =>
			{
				_.PollDeletedFlag = true;
				_.PollStartDate = now.AddDays(-4);
				_.PollEndDate = DateTime.UtcNow.AddDays(-2);
				_.PollCategoryId = 2;
				_.UserId = 1;
			});

			var entities = new Mock<IEntitiesContext>(MockBehavior.Strict);
			entities.Setup(_ => _.Mvcategory).Returns(new InMemoryDbSet<Mvcategory> { category1, category2 });
			entities.Setup(_ => _.Mvpoll).Returns(new InMemoryDbSet<Mvpoll> { poll1, poll2, poll3, poll4 });
			entities.Setup(_ => _.MvpollSubmission).Returns(new InMemoryDbSet<MvpollSubmission>());
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
			builder.Register<IEntitiesContext>(_ => entities.Object);
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
				result.SearchResultsByCategory.Count.Should().Be(2);
			}

			entities.VerifyAll();
		}
	}
}
