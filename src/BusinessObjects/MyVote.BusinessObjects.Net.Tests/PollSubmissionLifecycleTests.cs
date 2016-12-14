using Autofac;
using Csla;
using FluentAssertions;
using Moq;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;
using MyVote.BusinessObjects.Net.Tests.Extensions;
using MyVote.BusinessObjects.Rules;
using MyVote.Data.Entities;
using Spackle;
using Spackle.Extensions;
using System;
using System.Collections.Generic;
using Xunit;

namespace MyVote.BusinessObjects.Net.Tests
{
	public sealed class PollSubmissionLifecycleTests
	{
		[Fact]
		public void Create()
		{
			var now = DateTime.UtcNow;
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();

			var pollEntity = EntityCreator.Create<MVPoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollStartDate = now.AddDays(-2);
				_.PollEndDate = now.AddDays(2);
				_.PollMinAnswers = 2;
				_.PollMaxAnswers = 3;
			});

			pollEntity.MVPollOptions = new List<MVPollOption>
			{
				EntityCreator.Create<MVPollOption>(),
				EntityCreator.Create<MVPollOption>(),
				EntityCreator.Create<MVPollOption>(),
				EntityCreator.Create<MVPollOption>()
			};

			var category = EntityCreator.Create<MVCategory>(_ => _.CategoryID = pollEntity.PollCategoryID);

			Poll poll = null;

			var pollFactory = new Mock<IObjectFactory<IPoll>>(MockBehavior.Strict);
			pollFactory.Setup(_ => _.Fetch(pollEntity.PollID))
				.Returns<int>(_ =>
				{
					poll = DataPortal.Fetch<Poll>(_);
					return poll;
				});

			var pollSubmissionResponsesFactory = new Mock<IObjectFactory<IPollSubmissionResponseCollection>>(MockBehavior.Strict);
			pollSubmissionResponsesFactory.Setup(_ => _.CreateChild(It.IsAny<object[]>()))
				.Callback<object[]>(_ => _[0].Should().BeSameAs(poll.PollOptions))
				.Returns<object[]>(_ => DataPortal.CreateChild<PollSubmissionResponseCollection>(_[0] as BusinessList<IPollOption>));

			var entities = new Mock<IEntities>(MockBehavior.Strict);
			entities.Setup(_ => _.MVPolls)
				.Returns(new InMemoryDbSet<MVPoll> { pollEntity });
			entities.Setup(_ => _.MVPollSubmissions)
				.Returns(new InMemoryDbSet<MVPollSubmission>());
			entities.Setup(_ => _.MVCategories)
				.Returns(new InMemoryDbSet<MVCategory> { category });
			entities.Setup(_ => _.Dispose());

			var pollOptions = new Mock<IObjectFactory<BusinessList<IPollOption>>>();
			pollOptions.Setup(_ => _.FetchChild()).Returns(new BusinessList<IPollOption>());

			var pollOption = new Mock<IObjectFactory<IPollOption>>();
			pollOption.Setup(_ => _.FetchChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.FetchChild<PollOption>(_[0] as MVPollOption));

			var pollSubmissionResponseFactory = new Mock<IObjectFactory<IPollSubmissionResponse>>();
			pollSubmissionResponseFactory.Setup(_ => _.CreateChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.CreateChild<PollSubmissionResponse>(_[0] as IPollOption));

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => entities.Object);
			builder.Register<IObjectFactory<IPoll>>(_ => pollFactory.Object);
			builder.Register<IObjectFactory<BusinessList<IPollOption>>>(_ => pollOptions.Object);
			builder.Register<IObjectFactory<IPollOption>>(_ => pollOption.Object);
			builder.Register<IObjectFactory<IPollSubmissionResponseCollection>>(_ => pollSubmissionResponsesFactory.Object);
			builder.Register<IObjectFactory<IPollSubmissionResponse>>(_ => pollSubmissionResponseFactory.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var criteria = new PollSubmissionCriteria(pollEntity.PollID, userId);
				var submission = DataPortal.Create<PollSubmission>(criteria);

				submission.PollID.Should().Be(pollEntity.PollID);
				submission.PollImageLink.Should().Be(pollEntity.PollImageLink);
				submission.UserID.Should().Be(userId);
				submission.PollQuestion.Should().Be(pollEntity.PollQuestion);
				submission.CategoryName.Should().Be(category.CategoryName);
				submission.PollDescription.Should().Be(pollEntity.PollDescription);
				submission.PollMaxAnswers.Should().Be(pollEntity.PollMaxAnswers.Value);
				submission.PollMinAnswers.Should().Be(pollEntity.PollMinAnswers.Value);
				submission.Responses.Count.Should().Be(4);

				submission.BrokenRulesCollection.AssertRuleCount(1);
				submission.BrokenRulesCollection.AssertRuleCount(PollSubmission.ResponsesProperty, 1);
				submission.BrokenRulesCollection.AssertBusinessRuleExists<MinimumAndMaximumPollSubmissionResponsesRule>(
					PollSubmission.ResponsesProperty, true);
			}

			entities.VerifyAll();
			pollFactory.VerifyAll();
			pollSubmissionResponsesFactory.VerifyAll();
		}

		[Fact]
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
			entities.Setup(_ => _.Dispose());

			var pollFactory = new Mock<IObjectFactory<IPoll>>();
			pollFactory.Setup(_ => _.Fetch(pollId)).Returns<int>(_ => DataPortal.Fetch<Poll>(_));

			var pollSubmissionResponsesFactory = new Mock<IObjectFactory<IPollSubmissionResponseCollection>>();
			pollSubmissionResponsesFactory.Setup(_ => _.CreateChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.CreateChild<PollSubmissionResponseCollection>(_[0] as BusinessList<IPollOption>));

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => entities.Object);
			builder.Register<IObjectFactory<IPoll>>(_ => pollFactory.Object);
			builder.Register<IObjectFactory<IPollSubmissionResponseCollection>>(_ => pollSubmissionResponsesFactory.Object);

			using (new ObjectActivator(builder.Build(), Mock.Of<ICallContext>())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var criteria = new PollSubmissionCriteria(pollId, userId);
				new Action(() => new DataPortal<PollSubmission>().Create(criteria)).ShouldThrow<DataPortalException>();
			}
		}

		[Fact]
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

			var entities = new Mock<IEntities>(MockBehavior.Strict);
			entities.Setup(_ => _.MVPolls)
				.Returns(new InMemoryDbSet<MVPoll> { poll });
			entities.Setup(_ => _.MVPollSubmissions)
				.Returns(new InMemoryDbSet<MVPollSubmission>());
			entities.Setup(_ => _.MVCategories)
				.Returns(new InMemoryDbSet<MVCategory> { category });
			entities.Setup(_ => _.Dispose());

			var pollOptions = new Mock<IObjectFactory<BusinessList<IPollOption>>>();
			pollOptions.Setup(_ => _.FetchChild()).Returns(new BusinessList<IPollOption>());

			var pollOption = new Mock<IObjectFactory<IPollOption>>();
			pollOption.Setup(_ => _.FetchChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.FetchChild<PollOption>(_[0] as MVPollOption));

			var pollFactory = new Mock<IObjectFactory<IPoll>>();
			pollFactory.Setup(_ => _.Fetch(poll.PollID)).Returns<int>(_ => DataPortal.Fetch<Poll>(_));

			var pollSubmissionResponsesFactory = new Mock<IObjectFactory<IPollSubmissionResponseCollection>>();
			pollSubmissionResponsesFactory.Setup(_ => _.CreateChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.CreateChild<PollSubmissionResponseCollection>(_[0] as BusinessList<IPollOption>));

			var pollSubmissionResponseFactory = new Mock<IObjectFactory<IPollSubmissionResponse>>();
			pollSubmissionResponseFactory.Setup(_ => _.CreateChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.CreateChild<PollSubmissionResponse>(_[0] as IPollOption));

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => entities.Object);
			builder.Register<IObjectFactory<BusinessList<IPollOption>>>(_ => pollOptions.Object);
			builder.Register<IObjectFactory<IPollOption>>(_ => pollOption.Object);
			builder.Register<IObjectFactory<IPoll>>(_ => pollFactory.Object);
			builder.Register<IObjectFactory<IPollSubmissionResponseCollection>>(_ => pollSubmissionResponsesFactory.Object);
			builder.Register<IObjectFactory<IPollSubmissionResponse>>(_ => pollSubmissionResponseFactory.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var submission = new DataPortal<PollSubmission>().Create(
					new PollSubmissionCriteria(poll.PollID, userId));
				submission.BrokenRulesCollection.AssertRuleCount(PollSubmission.PollStartDateProperty, 1);
				submission.BrokenRulesCollection.AssertBusinessRuleExists<PollSubmissionPollStartDateRule>(
					PollSubmission.PollStartDateProperty, true);
			}

			entities.VerifyAll();
		}

		[Fact]
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

			var entities = new Mock<IEntities>(MockBehavior.Strict);
			entities.Setup(_ => _.MVPolls)
				.Returns(new InMemoryDbSet<MVPoll> { poll });
			entities.Setup(_ => _.MVPollSubmissions)
				.Returns(new InMemoryDbSet<MVPollSubmission>());
			entities.Setup(_ => _.MVCategories)
				.Returns(new InMemoryDbSet<MVCategory> { category });
			entities.Setup(_ => _.Dispose());

			var pollOptions = new Mock<IObjectFactory<BusinessList<IPollOption>>>();
			pollOptions.Setup(_ => _.FetchChild()).Returns(new BusinessList<IPollOption>());

			var pollOption = new Mock<IObjectFactory<IPollOption>>();
			pollOption.Setup(_ => _.FetchChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.FetchChild<PollOption>(_[0] as MVPollOption));

			var pollFactory = new Mock<IObjectFactory<IPoll>>();
			pollFactory.Setup(_ => _.Fetch(poll.PollID)).Returns<int>(_ => DataPortal.Fetch<Poll>(_));

			var pollSubmissionResponsesFactory = new Mock<IObjectFactory<IPollSubmissionResponseCollection>>();
			pollSubmissionResponsesFactory.Setup(_ => _.CreateChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.CreateChild<PollSubmissionResponseCollection>(_[0] as BusinessList<IPollOption>));

			var pollSubmissionResponseFactory = new Mock<IObjectFactory<IPollSubmissionResponse>>();
			pollSubmissionResponseFactory.Setup(_ => _.CreateChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.CreateChild<PollSubmissionResponse>(_[0] as IPollOption));

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => entities.Object);
			builder.Register<IObjectFactory<BusinessList<IPollOption>>>(_ => pollOptions.Object);
			builder.Register<IObjectFactory<IPollOption>>(_ => pollOption.Object);
			builder.Register<IObjectFactory<IPoll>>(_ => pollFactory.Object);
			builder.Register<IObjectFactory<IPollSubmissionResponseCollection>>(_ => pollSubmissionResponsesFactory.Object);
			builder.Register<IObjectFactory<IPollSubmissionResponse>>(_ => pollSubmissionResponseFactory.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var submission = new DataPortal<PollSubmission>().Create(
					new PollSubmissionCriteria(poll.PollID, userId));
				submission.BrokenRulesCollection.AssertRuleCount(PollSubmission.PollEndDateProperty, 1);
				submission.BrokenRulesCollection.AssertBusinessRuleExists<PollSubmissionPollEndDateRule>(
					PollSubmission.PollEndDateProperty, true);
			}

			entities.VerifyAll();
		}

		[Fact]
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

			var pollFactory = new Mock<IObjectFactory<IPoll>>();
			pollFactory.Setup(_ => _.Fetch(poll.PollID)).Returns<int>(_ => DataPortal.Fetch<Poll>(_));

			var entities = new Mock<IEntities>(MockBehavior.Strict);
			entities.Setup(_ => _.MVPolls)
				.Returns(new InMemoryDbSet<MVPoll> { poll });
			entities.Setup(_ => _.MVPollSubmissions)
				.Returns(new InMemoryDbSet<MVPollSubmission>());
			entities.Setup(_ => _.MVCategories)
				.Returns(new InMemoryDbSet<MVCategory> { category });
			entities.Setup(_ => _.Dispose());

			var pollOptions = new Mock<IObjectFactory<BusinessList<IPollOption>>>();
			pollOptions.Setup(_ => _.FetchChild()).Returns(new BusinessList<IPollOption>());

			var pollOption = new Mock<IObjectFactory<IPollOption>>();
			pollOption.Setup(_ => _.FetchChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.FetchChild<PollOption>(_[0] as MVPollOption));

			var pollSubmissionResponsesFactory = new Mock<IObjectFactory<IPollSubmissionResponseCollection>>();
			pollSubmissionResponsesFactory.Setup(_ => _.CreateChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.CreateChild<PollSubmissionResponseCollection>(_[0] as BusinessList<IPollOption>));

			var pollSubmissionResponseFactory = new Mock<IObjectFactory<IPollSubmissionResponse>>();
			pollSubmissionResponseFactory.Setup(_ => _.CreateChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.CreateChild<PollSubmissionResponse>(_[0] as IPollOption));

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => entities.Object);
			builder.Register<IObjectFactory<BusinessList<IPollOption>>>(_ => pollOptions.Object);
			builder.Register<IObjectFactory<IPollOption>>(_ => pollOption.Object);
			builder.Register<IObjectFactory<IPoll>>(_ => pollFactory.Object);
			builder.Register<IObjectFactory<IPollSubmissionResponseCollection>>(_ => pollSubmissionResponsesFactory.Object);
			builder.Register<IObjectFactory<IPollSubmissionResponse>>(_ => pollSubmissionResponseFactory.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var submission = new DataPortal<PollSubmission>().Create(
					new PollSubmissionCriteria(poll.PollID, userId));
				submission.BrokenRulesCollection.AssertRuleCount(PollSubmission.PollDeletedFlagProperty, 1);
				submission.BrokenRulesCollection.AssertBusinessRuleExists<PollSubmissionPollDeletedFlagRule>(
					PollSubmission.PollDeletedFlagProperty, true);
			}

			entities.VerifyAll();
		}

		[Fact]
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

			var entities = new Mock<IEntities>(MockBehavior.Strict);
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
				}).Returns(1);
			entities.Setup(_ => _.Dispose());

			var pollOptions = new Mock<IObjectFactory<BusinessList<IPollOption>>>();
			pollOptions.Setup(_ => _.FetchChild()).Returns(new BusinessList<IPollOption>());

			var pollOption = new Mock<IObjectFactory<IPollOption>>();
			pollOption.Setup(_ => _.FetchChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.FetchChild<PollOption>(_[0] as MVPollOption));

			var pollFactory = new Mock<IObjectFactory<IPoll>>();
			pollFactory.Setup(_ => _.Fetch(poll.PollID)).Returns<int>(_ => DataPortal.Fetch<Poll>(_));

			var pollSubmissionResponsesFactory = new Mock<IObjectFactory<IPollSubmissionResponseCollection>>();
			pollSubmissionResponsesFactory.Setup(_ => _.CreateChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.CreateChild<PollSubmissionResponseCollection>(_[0] as BusinessList<IPollOption>));

			var pollSubmissionResponseFactory = new Mock<IObjectFactory<IPollSubmissionResponse>>();
			pollSubmissionResponseFactory.Setup(_ => _.CreateChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.CreateChild<PollSubmissionResponse>(_[0] as IPollOption));

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => entities.Object);
			builder.Register<IObjectFactory<BusinessList<IPollOption>>>(_ => pollOptions.Object);
			builder.Register<IObjectFactory<IPollOption>>(_ => pollOption.Object);
			builder.Register<IObjectFactory<IPoll>>(_ => pollFactory.Object);
			builder.Register<IObjectFactory<IPollSubmissionResponseCollection>>(_ => pollSubmissionResponsesFactory.Object);
			builder.Register<IObjectFactory<IPollSubmissionResponse>>(_ => pollSubmissionResponseFactory.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var criteria = new PollSubmissionCriteria(poll.PollID, userId);
				var submission = new DataPortal<PollSubmission>().Create(criteria);

				submission.Responses[1].IsOptionSelected = true;
				submission.Responses[3].IsOptionSelected = true;

				submission = submission.Save();

				submissions.Local.Count.Should().Be(1);
				submission.PollSubmissionID.Should().Be(submissionId);
				responses.Local.Count.Should().Be(4);
				responses.Local[0].OptionSelected.Should().BeFalse();
				responses.Local[1].OptionSelected.Should().BeTrue();
				responses.Local[2].OptionSelected.Should().BeFalse();
				responses.Local[3].OptionSelected.Should().BeTrue();
			}

			entities.VerifyAll();
		}
	}
}
