using System;
using System.Collections.Generic;
using Autofac;
using Csla;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyVote.BusinessObjects.Net.Tests.Extensions;
using MyVote.BusinessObjects.Rules;
using MyVote.Core.Extensions;
using MyVote.Repository;
using Spackle;
using Spackle.Extensions;

namespace MyVote.BusinessObjects.Net.Tests
{
	[TestClass]
	public sealed class PollSubmissionLifecycleTests
	{
		[TestMethod]
		public void Create()
		{
			var now = DateTime.UtcNow;
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();

			var poll = EntityCreator.Create<MVPoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollStartDate = now.AddDays(-2);
				_.PollEndDate = now.AddDays(2);
				_.PollMinAnswers = 2;
				_.PollMaxAnswers = 3;
			});

			poll.MVPollOptions = new List<MVPollOption> 
			{ 
				EntityCreator.Create<MVPollOption>(),
				EntityCreator.Create<MVPollOption>(),
				EntityCreator.Create<MVPollOption>(),
				EntityCreator.Create<MVPollOption>()
			};

			var category = EntityCreator.Create<MVCategory>(_ => _.CategoryID = poll.PollCategoryID);

			var entities = new Mock<IEntities>();
			entities.Setup(_ => _.MVPolls)
				.Returns(new InMemoryDbSet<MVPoll> { poll });
			entities.Setup(_ => _.MVPollSubmissions)
				.Returns(new InMemoryDbSet<MVPollSubmission>());
			entities.Setup(_ => _.MVCategories)
				.Returns(new InMemoryDbSet<MVCategory> { category });

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => entities.Object);

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
			{
				var criteria = new PollSubmissionCriteria(poll.PollID, userId);
				var submission = DataPortal.Create<PollSubmission>(criteria);

				Assert.AreEqual(poll.PollID, submission.PollID, submission.GetPropertyName(_ => _.PollID));
				Assert.AreEqual(poll.PollImageLink, submission.PollImageLink, submission.GetPropertyName(_ => _.PollImageLink));
				Assert.AreEqual(userId, submission.UserID, submission.GetPropertyName(_ => _.UserID));
				Assert.AreEqual(poll.PollQuestion, submission.PollQuestion, submission.GetPropertyName(_ => _.PollQuestion));
				Assert.AreEqual(category.CategoryName, submission.CategoryName, submission.GetPropertyName(_ => _.CategoryName));
				Assert.AreEqual(poll.PollDescription, submission.PollDescription, submission.GetPropertyName(_ => _.PollDescription));
				Assert.AreEqual(poll.PollMaxAnswers.Value, submission.PollMaxAnswers, submission.GetPropertyName(_ => _.PollMaxAnswers));
				Assert.AreEqual(poll.PollMinAnswers.Value, submission.PollMinAnswers, submission.GetPropertyName(_ => _.PollMinAnswers));
				Assert.AreEqual(4, submission.Responses.Count, submission.GetPropertyName(_ => _.Responses));

				submission.BrokenRulesCollection.AssertRuleCount(1);
				submission.BrokenRulesCollection.AssertRuleCount(PollSubmission.ResponsesProperty, 1);
				submission.BrokenRulesCollection.AssertBusinessRuleExists<MinimumAndMaximumPollSubmissionResponsesRule>(
					PollSubmission.ResponsesProperty, true);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(DataPortalException))]
		public void CreateWhenUserHasSubmittedAnswersBefore()
		{
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();
			var pollId = generator.Generate<int>();

			var entity = EntityCreator.Create<MVPollSubmission>(_ =>
			{
				_.UserID = userId;
				_.PollID = pollId;
			});

			var entities = new Mock<IEntities>();
			entities.Setup(_ => _.MVPollSubmissions)
				.Returns(new InMemoryDbSet<MVPollSubmission> { entity });

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => entities.Object);

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
			{
				var criteria = new PollSubmissionCriteria(pollId, userId);
				new DataPortal<PollSubmission>().Create(criteria);
			}
		}

		[TestMethod]
		public void CreateWithStartDateInTheFuture()
		{
			var now = DateTime.UtcNow;
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();

			var poll = EntityCreator.Create<MVPoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollStartDate = now.AddDays(2);
				_.PollEndDate = now.AddDays(4);
			});

			poll.MVPollOptions = new List<MVPollOption> { EntityCreator.Create<MVPollOption>() };

			var category = EntityCreator.Create<MVCategory>(_ => _.CategoryID = poll.PollCategoryID);

			var entities = new Mock<IEntities>();
			entities.Setup(_ => _.MVPolls)
				.Returns(new InMemoryDbSet<MVPoll> { poll });
			entities.Setup(_ => _.MVPollSubmissions)
				.Returns(new InMemoryDbSet<MVPollSubmission>());
			entities.Setup(_ => _.MVCategories)
				.Returns(new InMemoryDbSet<MVCategory> { category });

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => entities.Object);

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
			{
				var submission = new DataPortal<PollSubmission>().Create(
					new PollSubmissionCriteria(poll.PollID, userId));
				submission.BrokenRulesCollection.AssertRuleCount(PollSubmission.PollStartDateProperty, 1);
				submission.BrokenRulesCollection.AssertBusinessRuleExists<PollSubmissionPollStartDateRule>(
					PollSubmission.PollStartDateProperty, true);
			}
		}

		[TestMethod]
		public void CreateWithEndDateInThePast()
		{
			var now = DateTime.UtcNow;
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();

			var poll = EntityCreator.Create<MVPoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollStartDate = now.AddDays(-4);
				_.PollEndDate = now.AddDays(-2);
			});

			poll.MVPollOptions = new List<MVPollOption> { EntityCreator.Create<MVPollOption>() };

			var category = EntityCreator.Create<MVCategory>(_ => _.CategoryID = poll.PollCategoryID);

			var entities = new Mock<IEntities>();
			entities.Setup(_ => _.MVPolls)
				.Returns(new InMemoryDbSet<MVPoll> { poll });
			entities.Setup(_ => _.MVPollSubmissions)
				.Returns(new InMemoryDbSet<MVPollSubmission>());
			entities.Setup(_ => _.MVCategories)
				.Returns(new InMemoryDbSet<MVCategory> { category });

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => entities.Object);

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
			{
				var submission = new DataPortal<PollSubmission>().Create(
					new PollSubmissionCriteria(poll.PollID, userId));
				submission.BrokenRulesCollection.AssertRuleCount(PollSubmission.PollEndDateProperty, 1);
				submission.BrokenRulesCollection.AssertBusinessRuleExists<PollSubmissionPollEndDateRule>(
					PollSubmission.PollEndDateProperty, true);
			}
		}

		[TestMethod]
		public void CreateWhenPollIsDeleted()
		{
			var now = DateTime.UtcNow;
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();

			var poll = EntityCreator.Create<MVPoll>(_ =>
			{
				_.PollDeletedFlag = true;
				_.PollStartDate = now.AddDays(-2);
				_.PollEndDate = now.AddDays(2);
				_.PollMinAnswers = 2;
				_.PollMaxAnswers = 3;
			});

			poll.MVPollOptions = new List<MVPollOption> 
			{ 
				EntityCreator.Create<MVPollOption>(),
				EntityCreator.Create<MVPollOption>(),
				EntityCreator.Create<MVPollOption>(),
				EntityCreator.Create<MVPollOption>()
			};

			var category = EntityCreator.Create<MVCategory>(_ => _.CategoryID = poll.PollCategoryID);

			var entities = new Mock<IEntities>();
			entities.Setup(_ => _.MVPolls)
				.Returns(new InMemoryDbSet<MVPoll> { poll });
			entities.Setup(_ => _.MVPollSubmissions)
				.Returns(new InMemoryDbSet<MVPollSubmission>());
			entities.Setup(_ => _.MVCategories)
				.Returns(new InMemoryDbSet<MVCategory> { category });

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => entities.Object);

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
			{
				var submission = new DataPortal<PollSubmission>().Create(
					new PollSubmissionCriteria(poll.PollID, userId));
				submission.BrokenRulesCollection.AssertRuleCount(PollSubmission.PollDeletedFlagProperty, 1);
				submission.BrokenRulesCollection.AssertBusinessRuleExists<PollSubmissionPollDeletedFlagRule>(
					PollSubmission.PollDeletedFlagProperty, true);
			}
		}

		[TestMethod]
		public void Insert()
		{
			var now = DateTime.UtcNow;
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();

			var poll = EntityCreator.Create<MVPoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollStartDate = now.AddDays(-2);
				_.PollEndDate = now.AddDays(2);
				_.PollMinAnswers = 2;
				_.PollMaxAnswers = 3;
			});

			poll.MVPollOptions = new List<MVPollOption> 
			{ 
				EntityCreator.Create<MVPollOption>(),
				EntityCreator.Create<MVPollOption>(),
				EntityCreator.Create<MVPollOption>(),
				EntityCreator.Create<MVPollOption>()
			};

			var category = EntityCreator.Create<MVCategory>(_ => _.CategoryID = poll.PollCategoryID);

			var polls = new InMemoryDbSet<MVPoll> { poll };
			var submissions = new InMemoryDbSet<MVPollSubmission>();
			var responses = new InMemoryDbSet<MVPollResponse>();

			var saveChangesCount = 0;
			var submissionId = generator.Generate<int>();

			var entities = new Mock<IEntities>();
			entities.Setup(_ => _.MVPolls).Returns(polls);
			entities.Setup(_ => _.MVPollSubmissions).Returns(submissions);
			entities.Setup(_ => _.MVPollResponses).Returns(responses);
			entities.Setup(_ => _.MVCategories)
				.Returns(new InMemoryDbSet<MVCategory> { category });
			entities.Setup(_ => _.SaveChanges()).Callback(() =>
				{
					if (saveChangesCount == 0)
					{
						submissions.Local[0].PollSubmissionID = submissionId;
					}

					saveChangesCount++;
				});

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => entities.Object);

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
			{
				var criteria = new PollSubmissionCriteria(poll.PollID, userId);
				var submission = new DataPortal<PollSubmission>().Create(criteria);

				submission.Responses[1].IsOptionSelected = true;
				submission.Responses[3].IsOptionSelected = true;

				submission = submission.Save();

				Assert.AreEqual(1, submissions.Local.Count, submissions.GetPropertyName(_ => _.Local.Count));
				Assert.AreEqual(submissionId, submission.PollSubmissionID, submission.GetPropertyName(_ => _.PollSubmissionID));
				Assert.AreEqual(4, responses.Local.Count, responses.GetPropertyName(_ => _.Local.Count));
				Assert.IsFalse(responses.Local[0].OptionSelected, responses.GetPropertyName(_ => _.Local[0].OptionSelected));
				Assert.IsTrue(responses.Local[1].OptionSelected, responses.GetPropertyName(_ => _.Local[1].OptionSelected));
				Assert.IsFalse(responses.Local[2].OptionSelected, responses.GetPropertyName(_ => _.Local[2].OptionSelected));
				Assert.IsTrue(responses.Local[3].OptionSelected, responses.GetPropertyName(_ => _.Local[3].OptionSelected));
			}
		}
	}
}
