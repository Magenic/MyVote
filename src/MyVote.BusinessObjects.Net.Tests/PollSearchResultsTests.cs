using System;
using Autofac;
using Csla;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyVote.Core.Extensions;
using MyVote.Repository;
using Spackle.Extensions;

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

			var entities = new Mock<IEntities>();
			entities.Setup(_ => _.MVCategories).Returns(new InMemoryDbSet<MVCategory> { category1 });
			entities.Setup(_ => _.MVPolls).Returns(new InMemoryDbSet<MVPoll> { poll1, poll2, poll3, poll4 });
			entities.Setup(_ => _.MVPollSubmissions).Returns(new InMemoryDbSet<MVPollSubmission> { submission1, submission2, submission3, submission4 });

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => entities.Object);

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
			{
				var result = DataPortal.Fetch<PollSearchResults>(PollSearchResultsQueryType.MostPopular);
				Assert.AreEqual(1, result.SearchResultsByCategory.Count,
					result.GetPropertyName(_ => _.SearchResultsByCategory));
				Assert.AreEqual(3, result.SearchResultsByCategory[0].SearchResults.Count,
					result.SearchResultsByCategory[0].GetPropertyName(_ => _.SearchResults));
				Assert.AreEqual(poll2.PollQuestion, result.SearchResultsByCategory[0].SearchResults[0].Question,
					result.GetPropertyName(_ => _.SearchResultsByCategory[0].SearchResults[0].Question) + " 0");
				Assert.AreEqual(poll1.PollQuestion, result.SearchResultsByCategory[0].SearchResults[1].Question,
					result.GetPropertyName(_ => _.SearchResultsByCategory[0].SearchResults[1].Question) + " 1");
			}
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

			var entities = new Mock<IEntities>();
			entities.Setup(_ => _.MVCategories).Returns(new InMemoryDbSet<MVCategory> { category1, category2 });
			entities.Setup(_ => _.MVPolls).Returns(new InMemoryDbSet<MVPoll> { poll1, poll2, poll3, poll4, poll5, poll6, poll7 });
			entities.Setup(_ => _.MVPollSubmissions).Returns(new InMemoryDbSet<MVPollSubmission>());

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => entities.Object);

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
			{
				var result = DataPortal.Fetch<PollSearchResults>(PollSearchResultsQueryType.Newest);
				Assert.AreEqual(2, result.SearchResultsByCategory.Count,
					result.GetPropertyName(_ => _.SearchResultsByCategory));

				var firstCategory = result.SearchResultsByCategory[0];
				Assert.AreEqual("a", firstCategory.Category,
					firstCategory.GetPropertyName(_ => _.Category) + " a");
				Assert.AreEqual(3, firstCategory.SearchResults.Count,
					firstCategory.GetPropertyName(_ => _.SearchResults) + " a");
				Assert.AreEqual(poll6.PollQuestion, firstCategory.SearchResults[0].Question,
					firstCategory.GetPropertyName(_ => _.SearchResults[0].Question) + " a 0");
				Assert.AreEqual(poll2.PollQuestion, firstCategory.SearchResults[1].Question,
					firstCategory.GetPropertyName(_ => _.SearchResults[1].Question) + " a 1");
				Assert.AreEqual(poll4.PollQuestion, firstCategory.SearchResults[2].Question,
					firstCategory.GetPropertyName(_ => _.SearchResults[2].Question) + " a 2");

				var secondCategory = result.SearchResultsByCategory[1];
				Assert.AreEqual("b", secondCategory.Category,
					secondCategory.GetPropertyName(_ => _.Category) + " b");
				Assert.AreEqual(3, secondCategory.SearchResults.Count,
					secondCategory.GetPropertyName(_ => _.SearchResults) + " b");
				Assert.AreEqual(poll5.PollQuestion, secondCategory.SearchResults[0].Question,
					secondCategory.GetPropertyName(_ => _.SearchResults[0].Question) + " b 0");
				Assert.AreEqual(poll1.PollQuestion, secondCategory.SearchResults[1].Question,
					secondCategory.GetPropertyName(_ => _.SearchResults[1].Question) + " b 1");
				Assert.AreEqual(poll3.PollQuestion, secondCategory.SearchResults[2].Question,
					secondCategory.GetPropertyName(_ => _.SearchResults[2].Question) + " b 2");
			}
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
				_.PollQuestion = "abcde";
				_.PollEndDate = now.AddDays(2);
				_.PollStartDate = now.AddDays(-2);
			});
			var poll3 = EntityCreator.Create<MVPoll>(_ =>
			{
				_.PollDeletedFlag = true;
				_.PollCategoryID = category1.CategoryID;
				_.PollQuestion = "abcde";
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

			var entities = new Mock<IEntities>();
			entities.Setup(_ => _.MVCategories).Returns(new InMemoryDbSet<MVCategory> { category1 });
			entities.Setup(_ => _.MVPolls).Returns(new InMemoryDbSet<MVPoll> { poll1, poll2, poll3 });
			entities.Setup(_ => _.MVPollSubmissions).Returns(new InMemoryDbSet<MVPollSubmission> { submission1, submission2, submission3, submission4 });

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => entities.Object);

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
			{
				var result = DataPortal.Fetch<PollSearchResults>("abc");
				Assert.AreEqual(1, result.SearchResultsByCategory.Count,
					result.GetPropertyName(_ => _.SearchResultsByCategory));
				Assert.AreEqual(1, result.SearchResultsByCategory[0].SearchResults.Count,
					result.GetPropertyName(_ => _.SearchResultsByCategory[0].SearchResults.Count));
				Assert.AreEqual(poll2.PollQuestion, result.SearchResultsByCategory[0].SearchResults[0].Question,
					result.GetPropertyName(_ => _.SearchResultsByCategory[0].SearchResults[0].Question));
			}
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

			var entities = new Mock<IEntities>();
			entities.Setup(_ => _.MVCategories).Returns(new InMemoryDbSet<MVCategory> { category1, category2 });
			entities.Setup(_ => _.MVPolls).Returns(new InMemoryDbSet<MVPoll> { poll1, poll2, poll3, poll4 });
			entities.Setup(_ => _.MVPollSubmissions).Returns(new InMemoryDbSet<MVPollSubmission>());

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => entities.Object);

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
			{
				var result = DataPortal.Fetch<PollSearchResults>(
					new PollSearchResultsByUserCriteria(1, true));
				Assert.AreEqual(2, result.SearchResultsByCategory.Count,
					result.GetPropertyName(_ => _.SearchResultsByCategory));
			}
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

			var entities = new Mock<IEntities>();
			entities.Setup(_ => _.MVCategories).Returns(new InMemoryDbSet<MVCategory> { category1, category2 });
			entities.Setup(_ => _.MVPolls).Returns(new InMemoryDbSet<MVPoll> { poll1, poll2, poll3, poll4 });
			entities.Setup(_ => _.MVPollSubmissions).Returns(new InMemoryDbSet<MVPollSubmission>());

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => entities.Object);

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
			{
				var result = DataPortal.Fetch<PollSearchResults>(
					new PollSearchResultsByUserCriteria(1, false));
				Assert.AreEqual(2, result.SearchResultsByCategory.Count,
					result.GetPropertyName(_ => _.SearchResultsByCategory));
			}
		}
	}
}
