using Autofac;
using Csla;
using FluentAssertions;
using Moq;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;
using MyVote.BusinessObjects.Rules;
using MyVote.BusinessObjects.Tests.Extensions;
using MyVote.Data.Entities;
using Spackle;
using Spackle.Extensions;
using System;
using System.Collections.Generic;
using Xunit;

namespace MyVote.BusinessObjects.Tests
{
	public sealed class PollSubmissionLifecycleTests
	{
		[Fact]
		public void Create()
		{
			var now = DateTime.UtcNow;
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();

			var pollEntity = EntityCreator.Create<Mvpoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollStartDate = now.AddDays(-2);
				_.PollEndDate = now.AddDays(2);
				_.PollMinAnswers = 2;
				_.PollMaxAnswers = 3;
			});

			pollEntity.MvpollOption = new List<MvpollOption>
			{
				EntityCreator.Create<MvpollOption>(),
				EntityCreator.Create<MvpollOption>(),
				EntityCreator.Create<MvpollOption>(),
				EntityCreator.Create<MvpollOption>()
			};

			var category = EntityCreator.Create<Mvcategory>(_ => _.CategoryId = pollEntity.PollCategoryId);

			var pollFactory = new Mock<IObjectFactory<IPoll>>(MockBehavior.Strict);
			pollFactory.Setup(_ => _.Fetch(pollEntity.PollId))
				.Returns<int>(_ => DataPortal.Fetch<Poll>(_));

			var pollOptions = new BusinessList<IPollOption>();

			var pollSubmissionResponsesFactory = new Mock<IObjectFactory<IPollSubmissionResponseCollection>>(MockBehavior.Strict);
			pollSubmissionResponsesFactory.Setup(_ => _.CreateChild(It.IsAny<object[]>()))
				.Callback<object[]>(_ => _[0].Should().BeSameAs(pollOptions))
				.Returns<object[]>(_ => DataPortal.CreateChild<PollSubmissionResponseCollection>(_[0] as BusinessList<IPollOption>));

			var entities = new Mock<IEntitiesContext>(MockBehavior.Strict);
			entities.Setup(_ => _.Mvpoll)
				.Returns(new InMemoryDbSet<Mvpoll> { pollEntity });
			entities.Setup(_ => _.MvpollSubmission)
				.Returns(new InMemoryDbSet<MvpollSubmission>());
			entities.Setup(_ => _.Mvcategory)
				.Returns(new InMemoryDbSet<Mvcategory> { category });
			entities.Setup(_ => _.Dispose());

			var pollOptionsFactory = new Mock<IObjectFactory<BusinessList<IPollOption>>>();
			pollOptionsFactory.Setup(_ => _.FetchChild()).Returns(pollOptions);

			var pollOption = new Mock<IObjectFactory<IPollOption>>();
			pollOption.Setup(_ => _.FetchChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.FetchChild<PollOption>(_[0] as MvpollOption));

			var pollSubmissionResponseFactory = new Mock<IObjectFactory<IPollSubmissionResponse>>();
			pollSubmissionResponseFactory.Setup(_ => _.CreateChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.CreateChild<PollSubmissionResponse>(_[0] as IPollOption));

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => entities.Object);
			builder.Register<IObjectFactory<IPoll>>(_ => pollFactory.Object);
			builder.Register<IObjectFactory<BusinessList<IPollOption>>>(_ => pollOptionsFactory.Object);
			builder.Register<IObjectFactory<IPollOption>>(_ => pollOption.Object);
			builder.Register<IObjectFactory<IPollSubmissionResponseCollection>>(_ => pollSubmissionResponsesFactory.Object);
			builder.Register<IObjectFactory<IPollSubmissionResponse>>(_ => pollSubmissionResponseFactory.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var criteria = new PollSubmissionCriteria(pollEntity.PollId, userId);
				var submission = DataPortal.Create<PollSubmission>(criteria);

				submission.PollID.Should().Be(pollEntity.PollId);
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

			var entity = EntityCreator.Create<MvpollSubmission>(_ =>
			{
				_.UserId = userId;
				_.PollId = pollId;
			});

			var entities = new Mock<IEntitiesContext>();
			entities.Setup(_ => _.MvpollSubmission)
				.Returns(new InMemoryDbSet<MvpollSubmission> { entity });
			entities.Setup(_ => _.Dispose());

			var pollFactory = new Mock<IObjectFactory<IPoll>>();
			pollFactory.Setup(_ => _.Fetch(pollId))
				.Returns<int>(_ => DataPortal.Fetch<Poll>(_));

			var pollSubmissionResponsesFactory = new Mock<IObjectFactory<IPollSubmissionResponseCollection>>();
			pollSubmissionResponsesFactory.Setup(_ => _.CreateChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.CreateChild<PollSubmissionResponseCollection>(_[0] as BusinessList<IPollOption>));

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => entities.Object);
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

			var poll = EntityCreator.Create<Mvpoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollStartDate = now.AddDays(2);
				_.PollEndDate = now.AddDays(4);
			});

			poll.MvpollOption = new List<MvpollOption> { EntityCreator.Create<MvpollOption>() };

			var category = EntityCreator.Create<Mvcategory>(_ => _.CategoryId = poll.PollCategoryId);

			var entities = new Mock<IEntitiesContext>(MockBehavior.Strict);
			entities.Setup(_ => _.Mvpoll)
				.Returns(new InMemoryDbSet<Mvpoll> { poll });
			entities.Setup(_ => _.MvpollSubmission)
				.Returns(new InMemoryDbSet<MvpollSubmission>());
			entities.Setup(_ => _.Mvcategory)
				.Returns(new InMemoryDbSet<Mvcategory> { category });
			entities.Setup(_ => _.Dispose());

			var pollOptions = new Mock<IObjectFactory<BusinessList<IPollOption>>>();
			pollOptions.Setup(_ => _.FetchChild()).Returns(new BusinessList<IPollOption>());

			var pollOption = new Mock<IObjectFactory<IPollOption>>();
			pollOption.Setup(_ => _.FetchChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.FetchChild<PollOption>(_[0] as MvpollOption));

			var pollFactory = new Mock<IObjectFactory<IPoll>>();
			pollFactory.Setup(_ => _.Fetch(poll.PollId))
				.Returns<int>(_ => DataPortal.Fetch<Poll>(_));

			var pollSubmissionResponsesFactory = new Mock<IObjectFactory<IPollSubmissionResponseCollection>>();
			pollSubmissionResponsesFactory.Setup(_ => _.CreateChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.CreateChild<PollSubmissionResponseCollection>(_[0] as BusinessList<IPollOption>));

			var pollSubmissionResponseFactory = new Mock<IObjectFactory<IPollSubmissionResponse>>();
			pollSubmissionResponseFactory.Setup(_ => _.CreateChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.CreateChild<PollSubmissionResponse>(_[0] as IPollOption));

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => entities.Object);
			builder.Register<IObjectFactory<BusinessList<IPollOption>>>(_ => pollOptions.Object);
			builder.Register<IObjectFactory<IPollOption>>(_ => pollOption.Object);
			builder.Register<IObjectFactory<IPoll>>(_ => pollFactory.Object);
			builder.Register<IObjectFactory<IPollSubmissionResponseCollection>>(_ => pollSubmissionResponsesFactory.Object);
			builder.Register<IObjectFactory<IPollSubmissionResponse>>(_ => pollSubmissionResponseFactory.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var submission = new DataPortal<PollSubmission>().Create(
					new PollSubmissionCriteria(poll.PollId, userId));
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

			var poll = EntityCreator.Create<Mvpoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollStartDate = now.AddDays(-4);
				_.PollEndDate = now.AddDays(-2);
			});

			poll.MvpollOption = new List<MvpollOption> { EntityCreator.Create<MvpollOption>() };

			var category = EntityCreator.Create<Mvcategory>(_ => _.CategoryId = poll.PollCategoryId);

			var entities = new Mock<IEntitiesContext>(MockBehavior.Strict);
			entities.Setup(_ => _.Mvpoll)
				.Returns(new InMemoryDbSet<Mvpoll> { poll });
			entities.Setup(_ => _.MvpollSubmission)
				.Returns(new InMemoryDbSet<MvpollSubmission>());
			entities.Setup(_ => _.Mvcategory)
				.Returns(new InMemoryDbSet<Mvcategory> { category });
			entities.Setup(_ => _.Dispose());

			var pollOptions = new Mock<IObjectFactory<BusinessList<IPollOption>>>();
			pollOptions.Setup(_ => _.FetchChild()).Returns(new BusinessList<IPollOption>());

			var pollOption = new Mock<IObjectFactory<IPollOption>>();
			pollOption.Setup(_ => _.FetchChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.FetchChild<PollOption>(_[0] as MvpollOption));

			var pollFactory = new Mock<IObjectFactory<IPoll>>();
			pollFactory.Setup(_ => _.Fetch(poll.PollId))
				.Returns<int>(_ => DataPortal.Fetch<Poll>(_));

			var pollSubmissionResponsesFactory = new Mock<IObjectFactory<IPollSubmissionResponseCollection>>();
			pollSubmissionResponsesFactory.Setup(_ => _.CreateChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.CreateChild<PollSubmissionResponseCollection>(_[0] as BusinessList<IPollOption>));

			var pollSubmissionResponseFactory = new Mock<IObjectFactory<IPollSubmissionResponse>>();
			pollSubmissionResponseFactory.Setup(_ => _.CreateChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.CreateChild<PollSubmissionResponse>(_[0] as IPollOption));

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => entities.Object);
			builder.Register<IObjectFactory<BusinessList<IPollOption>>>(_ => pollOptions.Object);
			builder.Register<IObjectFactory<IPollOption>>(_ => pollOption.Object);
			builder.Register<IObjectFactory<IPoll>>(_ => pollFactory.Object);
			builder.Register<IObjectFactory<IPollSubmissionResponseCollection>>(_ => pollSubmissionResponsesFactory.Object);
			builder.Register<IObjectFactory<IPollSubmissionResponse>>(_ => pollSubmissionResponseFactory.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var submission = new DataPortal<PollSubmission>().Create(
					new PollSubmissionCriteria(poll.PollId, userId));
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

			var poll = EntityCreator.Create<Mvpoll>(_ =>
			{
				_.PollDeletedFlag = true;
				_.PollStartDate = now.AddDays(-2);
				_.PollEndDate = now.AddDays(2);
				_.PollMinAnswers = 2;
				_.PollMaxAnswers = 3;
			});

			poll.MvpollOption = new List<MvpollOption>
			{
				EntityCreator.Create<MvpollOption>(),
				EntityCreator.Create<MvpollOption>(),
				EntityCreator.Create<MvpollOption>(),
				EntityCreator.Create<MvpollOption>()
			};

			var category = EntityCreator.Create<Mvcategory>(_ => _.CategoryId = poll.PollCategoryId);

			var pollFactory = new Mock<IObjectFactory<IPoll>>();
			pollFactory.Setup(_ => _.Fetch(poll.PollId))
				.Returns<int>(_ => DataPortal.Fetch<Poll>(_));

			var entities = new Mock<IEntitiesContext>(MockBehavior.Strict);
			entities.Setup(_ => _.Mvpoll)
				.Returns(new InMemoryDbSet<Mvpoll> { poll });
			entities.Setup(_ => _.MvpollSubmission)
				.Returns(new InMemoryDbSet<MvpollSubmission>());
			entities.Setup(_ => _.Mvcategory)
				.Returns(new InMemoryDbSet<Mvcategory> { category });
			entities.Setup(_ => _.Dispose());

			var pollOptions = new Mock<IObjectFactory<BusinessList<IPollOption>>>();
			pollOptions.Setup(_ => _.FetchChild()).Returns(new BusinessList<IPollOption>());

			var pollOption = new Mock<IObjectFactory<IPollOption>>();
			pollOption.Setup(_ => _.FetchChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.FetchChild<PollOption>(_[0] as MvpollOption));

			var pollSubmissionResponsesFactory = new Mock<IObjectFactory<IPollSubmissionResponseCollection>>();
			pollSubmissionResponsesFactory.Setup(_ => _.CreateChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.CreateChild<PollSubmissionResponseCollection>(_[0] as BusinessList<IPollOption>));

			var pollSubmissionResponseFactory = new Mock<IObjectFactory<IPollSubmissionResponse>>();
			pollSubmissionResponseFactory.Setup(_ => _.CreateChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.CreateChild<PollSubmissionResponse>(_[0] as IPollOption));

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => entities.Object);
			builder.Register<IObjectFactory<BusinessList<IPollOption>>>(_ => pollOptions.Object);
			builder.Register<IObjectFactory<IPollOption>>(_ => pollOption.Object);
			builder.Register<IObjectFactory<IPoll>>(_ => pollFactory.Object);
			builder.Register<IObjectFactory<IPollSubmissionResponseCollection>>(_ => pollSubmissionResponsesFactory.Object);
			builder.Register<IObjectFactory<IPollSubmissionResponse>>(_ => pollSubmissionResponseFactory.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var submission = new DataPortal<PollSubmission>().Create(
					new PollSubmissionCriteria(poll.PollId, userId));
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

			var poll = EntityCreator.Create<Mvpoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollStartDate = now.AddDays(-2);
				_.PollEndDate = now.AddDays(2);
				_.PollMinAnswers = 2;
				_.PollMaxAnswers = 3;
			});

			poll.MvpollOption = new List<MvpollOption>
			{
				EntityCreator.Create<MvpollOption>(),
				EntityCreator.Create<MvpollOption>(),
				EntityCreator.Create<MvpollOption>(),
				EntityCreator.Create<MvpollOption>()
			};

			var category = EntityCreator.Create<Mvcategory>(_ => _.CategoryId = poll.PollCategoryId);

			var polls = new InMemoryDbSet<Mvpoll> { poll };
			var submissions = new InMemoryDbSet<MvpollSubmission>();
			var responses = new InMemoryDbSet<MvpollResponse>();

			var saveChangesCount = 0;
			var submissionId = generator.Generate<int>();

			var entities = new Mock<IEntitiesContext>(MockBehavior.Strict);
			entities.Setup(_ => _.Mvpoll).Returns(polls);
			entities.Setup(_ => _.MvpollSubmission).Returns(submissions);
			entities.Setup(_ => _.MvpollResponse).Returns(responses);
			entities.Setup(_ => _.Mvcategory)
				.Returns(new InMemoryDbSet<Mvcategory> { category });
			entities.Setup(_ => _.SaveChanges()).Callback(() =>
				{
					if (saveChangesCount == 0)
					{
						submissions.Local[0].PollSubmissionId = submissionId;
					}

					saveChangesCount++;
				}).Returns(1);
			entities.Setup(_ => _.Dispose());

			var pollOptions = new Mock<IObjectFactory<BusinessList<IPollOption>>>();
			pollOptions.Setup(_ => _.FetchChild()).Returns(new BusinessList<IPollOption>());

			var pollOption = new Mock<IObjectFactory<IPollOption>>();
			pollOption.Setup(_ => _.FetchChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.FetchChild<PollOption>(_[0] as MvpollOption));

			var pollFactory = new Mock<IObjectFactory<IPoll>>();
			pollFactory.Setup(_ => _.Fetch(poll.PollId))
				.Returns<int>(_ => DataPortal.Fetch<Poll>(_));

			var pollSubmissionResponsesFactory = new Mock<IObjectFactory<IPollSubmissionResponseCollection>>();
			pollSubmissionResponsesFactory.Setup(_ => _.CreateChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.CreateChild<PollSubmissionResponseCollection>(_[0] as BusinessList<IPollOption>));

			var pollSubmissionResponseFactory = new Mock<IObjectFactory<IPollSubmissionResponse>>();
			pollSubmissionResponseFactory.Setup(_ => _.CreateChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.CreateChild<PollSubmissionResponse>(_[0] as IPollOption));

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => entities.Object);
			builder.Register<IObjectFactory<BusinessList<IPollOption>>>(_ => pollOptions.Object);
			builder.Register<IObjectFactory<IPollOption>>(_ => pollOption.Object);
			builder.Register<IObjectFactory<IPoll>>(_ => pollFactory.Object);
			builder.Register<IObjectFactory<IPollSubmissionResponseCollection>>(_ => pollSubmissionResponsesFactory.Object);
			builder.Register<IObjectFactory<IPollSubmissionResponse>>(_ => pollSubmissionResponseFactory.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var criteria = new PollSubmissionCriteria(poll.PollId, userId);
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
