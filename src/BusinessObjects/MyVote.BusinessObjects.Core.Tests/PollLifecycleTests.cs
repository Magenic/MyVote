using Autofac;
using Csla;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
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
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;
using Xunit;

namespace MyVote.BusinessObjects.Tests
{
	public sealed class PollLifecycleTests
	{
		private static PollOption CreatePollOption()
		{
			var option = DataPortal.CreateChild<PollOption>();
			option.OptionPosition = 1;
			option.OptionText = "1";
			return option;
		}

		[Fact]
		public void Create()
		{
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();

			var pollOptions = new Mock<IObjectFactory<BusinessList<IPollOption>>>(MockBehavior.Strict);
			pollOptions.Setup(_ => _.CreateChild()).Returns(new BusinessList<IPollOption>());

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => Mock.Of<IEntitiesContext>());
			builder.Register<IObjectFactory<BusinessList<IPollOption>>>(_ => pollOptions.Object);
			builder.Register<IObjectFactory<IPollOption>>(_ => Mock.Of<IObjectFactory<IPollOption>>());

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var poll = new DataPortal<Poll>().Create(userId);

				poll.PollAdminRemovedFlag.Should().NotHaveValue();
				poll.PollCategoryID.Should().NotHaveValue();
				poll.PollDateRemoved.Should().NotHaveValue();
				poll.PollDeletedDate.Should().NotHaveValue();
				poll.PollDeletedFlag.Should().NotHaveValue();
				poll.PollDescription.Should().BeEmpty();
				poll.PollEndDate.Should().NotHaveValue();
				poll.PollID.Should().NotHaveValue();
				poll.PollImageLink.Should().BeEmpty();
				poll.PollMaxAnswers.Should().NotHaveValue();
				poll.PollMinAnswers.Should().NotHaveValue();
				poll.PollQuestion.Should().BeEmpty();
				poll.PollStartDate.Should().NotHaveValue();
				poll.UserID.Should().Be(userId);
				poll.PollOptions.Count.Should().Be(0);

				poll.BrokenRulesCollection.AssertRuleCount(7);
				poll.BrokenRulesCollection.AssertRuleCount(Poll.PollStartDateProperty, 1);
				poll.BrokenRulesCollection.AssertValidationRuleExists<RequiredAttribute>(
					Poll.PollStartDateProperty, true);
				poll.BrokenRulesCollection.AssertRuleCount(Poll.PollEndDateProperty, 1);
				poll.BrokenRulesCollection.AssertValidationRuleExists<RequiredAttribute>(
					Poll.PollEndDateProperty, true);
				poll.BrokenRulesCollection.AssertRuleCount(Poll.PollCategoryIDProperty, 1);
				poll.BrokenRulesCollection.AssertValidationRuleExists<RequiredAttribute>(
					Poll.PollCategoryIDProperty, true);
				poll.BrokenRulesCollection.AssertRuleCount(Poll.PollQuestionProperty, 1);
				poll.BrokenRulesCollection.AssertValidationRuleExists<RequiredAttribute>(
					Poll.PollQuestionProperty, true);
				poll.BrokenRulesCollection.AssertRuleCount(Poll.PollMinAnswersProperty, 1);
				poll.BrokenRulesCollection.AssertValidationRuleExists<RequiredAttribute>(
					Poll.PollMinAnswersProperty, true);
				poll.BrokenRulesCollection.AssertRuleCount(Poll.PollMaxAnswersProperty, 1);
				poll.BrokenRulesCollection.AssertValidationRuleExists<RequiredAttribute>(
					Poll.PollMaxAnswersProperty, true);
				poll.BrokenRulesCollection.AssertRuleCount(Poll.PollOptionsProperty, 1);
				poll.BrokenRulesCollection.AssertBusinessRuleExists<PollOptionsRule>(
					Poll.PollOptionsProperty, true);
			}

			pollOptions.VerifyAll();
		}

		[Fact]
		public void Fetch()
		{
			var entity = EntityCreator.Create<Mvpoll>(_ => _.PollDeletedFlag = false);
			entity.MvpollOption = new List<MvpollOption> { EntityCreator.Create<MvpollOption>() };

			var pollOptionsFactory = new Mock<IObjectFactory<BusinessList<IPollOption>>>(MockBehavior.Strict);
			pollOptionsFactory.Setup(_ => _.FetchChild()).Returns(new BusinessList<IPollOption>());

			var pollOption = new Mock<IPollOption>(MockBehavior.Loose);
			pollOption.Setup(_ => _.IsChild).Returns(true);

			var pollOptionFactory = new Mock<IObjectFactory<IPollOption>>(MockBehavior.Strict);
			pollOptionFactory.Setup(_ => _.FetchChild(It.IsAny<object[]>())).Returns(pollOption.Object);

			var entities = new Mock<IEntitiesContext>(MockBehavior.Strict);
			entities.Setup(_ => _.Mvpoll).Returns(new InMemoryDbSet<Mvpoll>() { entity });
			entities.Setup(_ => _.Dispose());

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => entities.Object);
			builder.Register<IObjectFactory<BusinessList<IPollOption>>>(_ => pollOptionsFactory.Object);
			builder.Register<IObjectFactory<IPollOption>>(_ => pollOptionFactory.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var poll = new DataPortal<Poll>().Fetch(entity.PollId);

				poll.PollAdminRemovedFlag.Should().Be(entity.PollAdminRemovedFlag);
				poll.PollCategoryID.Should().Be(entity.PollCategoryId);
				poll.PollDateRemoved.Should().Be(entity.PollDateRemoved);
				poll.PollDeletedDate.Should().Be(entity.PollDeletedDate);
				poll.PollDeletedFlag.Should().Be(entity.PollDeletedFlag);
				poll.PollDescription.Should().Be(entity.PollDescription);
				poll.PollEndDate.Should().Be(entity.PollEndDate);
				poll.PollID.Should().Be(entity.PollId);
				poll.PollImageLink.Should().Be(entity.PollImageLink);
				poll.PollMaxAnswers.Should().Be(entity.PollMaxAnswers);
				poll.PollMinAnswers.Should().Be(entity.PollMinAnswers);
				poll.PollQuestion.Should().Be(entity.PollQuestion);
				poll.PollStartDate.Should().Be(entity.PollStartDate);
				poll.UserID.Should().Be(entity.UserId);
				poll.PollOptions.Count.Should().Be(1);
				poll.PollOptions[0].Should().BeSameAs(pollOption.Object);
			}

			entities.VerifyAll();
			pollOptionsFactory.VerifyAll();
			pollOptionFactory.VerifyAll();
		}

		[Fact]
		public void Insert()
		{
			var generator = new RandomObjectGenerator();
			var pollAdminRemoveFlag = generator.Generate<bool>();
			var pollCategoryId = generator.Generate<int>();
			var pollDateRemoved = generator.Generate<DateTime>();
			var pollDeletedDate = generator.Generate<DateTime>();
			var pollDeletedFlag = generator.Generate<bool>();
			var pollDescription = generator.Generate<string>();
			var pollId = generator.Generate<int>();
			var pollImageLink = generator.Generate<string>();
			short pollMaxAnswers = 2;
			short pollMinAnswers = 1;
			var pollQuestion = generator.Generate<string>();
			var pollStartDate = generator.Generate<DateTime>();
			var pollEndDate = pollStartDate.AddDays(2);
			var userId = generator.Generate<int>();

			var polls = new InMemoryDbSet<Mvpoll>();
			var pollOptions = new InMemoryDbSet<MvpollOption>();

			var entities = new Mock<IEntitiesContext>(MockBehavior.Strict);
			entities.Setup(_ => _.Mvpoll).Returns(polls);
			entities.Setup(_ => _.MvpollOption).Returns(pollOptions);
			entities.Setup(_ => _.SaveChanges()).Callback(() => polls.Local[0].PollId = pollId)
				.Returns(1);
			entities.Setup(_ => _.Dispose());

			var pollOptionsFactory = new Mock<IObjectFactory<BusinessList<IPollOption>>>();
			pollOptionsFactory.Setup(_ => _.CreateChild()).Returns(new BusinessList<IPollOption>());

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => entities.Object);
			builder.Register<IObjectFactory<BusinessList<IPollOption>>>(_ => pollOptionsFactory.Object);
			builder.Register<IObjectFactory<IPollOption>>(_ => Mock.Of<IObjectFactory<IPollOption>>());

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var poll = new DataPortal<Poll>().Create(userId);
				poll.PollAdminRemovedFlag = pollAdminRemoveFlag;
				poll.PollCategoryID = pollCategoryId;
				poll.PollDateRemoved = pollDateRemoved;
				poll.PollDeletedDate = pollDeletedDate;
				poll.PollDeletedFlag = pollDeletedFlag;
				poll.PollDescription = pollDescription;
				poll.PollEndDate = pollEndDate;
				poll.PollImageLink = pollImageLink;
				poll.PollMaxAnswers = pollMaxAnswers;
				poll.PollMinAnswers = pollMinAnswers;
				poll.PollQuestion = pollQuestion;
				poll.PollStartDate = pollStartDate;
				poll.PollOptions.Add(PollLifecycleTests.CreatePollOption());
				poll.PollOptions.Add(PollLifecycleTests.CreatePollOption());

				poll = poll.Save();

				poll.PollAdminRemovedFlag.Should().Be(pollAdminRemoveFlag);
				poll.PollCategoryID.Should().Be(pollCategoryId);
				poll.PollDateRemoved.Should().Be(pollDateRemoved);
				poll.PollDeletedDate.Should().Be(pollDeletedDate);
				poll.PollDeletedFlag.Should().Be(pollDeletedFlag);
				poll.PollDescription.Should().Be(pollDescription);
				poll.PollEndDate.Should().Be(pollEndDate.ToUniversalTime());
				poll.PollID.Should().Be(pollId);
				poll.PollImageLink.Should().Be(pollImageLink);
				poll.PollMaxAnswers.Should().Be(pollMaxAnswers);
				poll.PollMinAnswers.Should().Be(pollMinAnswers);
				poll.PollQuestion.Should().Be(pollQuestion);
				poll.PollStartDate.Should().Be(pollStartDate.ToUniversalTime());
				poll.UserID.Should().Be(userId);
			}

			entities.VerifyAll();
		}

		[Fact]
		public void Update()
		{
			var now = DateTime.UtcNow;

			var entity = EntityCreator.Create<Mvpoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollMinAnswers = 1;
				_.PollMaxAnswers = 2;
				_.PollStartDate = now;
				_.PollEndDate = now.AddDays(2);
			});
			entity.MvpollOption = new List<MvpollOption>
			{
				EntityCreator.Create<MvpollOption>(),
				EntityCreator.Create<MvpollOption>(),
				EntityCreator.Create<MvpollOption>()
			};

			var generator = new RandomObjectGenerator();
			var newPollAdminRemoveFlag = generator.Generate<bool>();
			var newPollCategoryId = generator.Generate<int>();
			var newPollDateRemoved = generator.Generate<DateTime>();
			var newPollDeletedDate = generator.Generate<DateTime>();
			var newPollDeletedFlag = generator.Generate<bool>();
			var newPollDescription = generator.Generate<string>();
			var newPollEndDate = entity.PollEndDate.AddDays(1);
			var newPollImageLink = generator.Generate<string>();
			short newPollMaxAnswers = 3;
			short newPollMinAnswers = 2;
			var newPollQuestion = generator.Generate<string>();
			var newPollStartDate = entity.PollStartDate.AddDays(1);

			var entities = new Mock<IEntitiesContext>(MockBehavior.Strict);
			entities.Setup(_ => _.Mvpoll).Returns(new InMemoryDbSet<Mvpoll> { entity });
			entities.Setup(_ => _.SetState(It.IsAny<Mvpoll>(), EntityState.Modified));
			entities.Setup(_ => _.SaveChanges()).Returns(1);
			entities.Setup(_ => _.Dispose());

			var pollOptions = new Mock<IObjectFactory<BusinessList<IPollOption>>>();
			pollOptions.Setup(_ => _.FetchChild()).Returns(new BusinessList<IPollOption>());

			var pollOption = new Mock<IObjectFactory<IPollOption>>();
			pollOption.Setup(_ => _.FetchChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.FetchChild<PollOption>(_[0] as MvpollOption));

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => entities.Object);
			builder.Register<IObjectFactory<BusinessList<IPollOption>>>(_ => pollOptions.Object);
			builder.Register<IObjectFactory<IPollOption>>(_ => pollOption.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var poll = new DataPortal<Poll>().Fetch(entity.PollId);
				poll.PollAdminRemovedFlag = newPollAdminRemoveFlag;
				poll.PollCategoryID = newPollCategoryId;
				poll.PollDateRemoved = newPollDateRemoved;
				poll.PollDeletedDate = newPollDeletedDate;
				poll.PollDeletedFlag = newPollDeletedFlag;
				poll.PollDescription = newPollDescription;
				poll.PollEndDate = newPollEndDate;
				poll.PollImageLink = newPollImageLink;
				poll.PollMaxAnswers = newPollMaxAnswers;
				poll.PollMinAnswers = newPollMinAnswers;
				poll.PollQuestion = newPollQuestion;
				poll.PollStartDate = newPollStartDate;

				poll = poll.Save();

				poll.PollAdminRemovedFlag.Should().Be(newPollAdminRemoveFlag);
				poll.PollCategoryID.Should().Be(newPollCategoryId);
				poll.PollDateRemoved.Should().Be(newPollDateRemoved);
				poll.PollDeletedDate.Should().Be(newPollDeletedDate);
				poll.PollDeletedFlag.Should().Be(newPollDeletedFlag);
				poll.PollDescription.Should().Be(newPollDescription);
				poll.PollEndDate.Should().Be(newPollEndDate.ToUniversalTime());
				poll.PollImageLink.Should().Be(newPollImageLink);
				poll.PollMaxAnswers.Should().Be(newPollMaxAnswers);
				poll.PollMinAnswers.Should().Be(newPollMinAnswers);
				poll.PollQuestion.Should().Be(newPollQuestion);
				poll.PollStartDate.Should().Be(newPollStartDate.ToUniversalTime());
			}

			entities.VerifyAll();
		}

		[Fact]
		public void Delete()
		{
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();
			var now = DateTime.UtcNow;

			var entity = EntityCreator.Create<Mvpoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollMinAnswers = 1;
				_.PollMaxAnswers = 2;
				_.PollStartDate = now;
				_.PollEndDate = now.AddDays(2);
				_.UserId = userId;
			});
			entity.MvpollOption = new List<MvpollOption>
			{
				EntityCreator.Create<MvpollOption>(),
				EntityCreator.Create<MvpollOption>(),
				EntityCreator.Create<MvpollOption>()
			};

			var polls = new InMemoryDbSet<Mvpoll> { entity };

			var entities = new Mock<IEntitiesContext>(MockBehavior.Strict);
			entities.Setup(_ => _.Mvpoll).Returns(polls);
			entities.Setup(_ => _.SetState(It.IsAny<Mvpoll>(), EntityState.Modified));
			entities.Setup(_ => _.SaveChanges()).Returns(1);
			entities.Setup(_ => _.Dispose());

			var pollOptions = new Mock<IObjectFactory<BusinessList<IPollOption>>>();
			pollOptions.Setup(_ => _.FetchChild()).Returns(new BusinessList<IPollOption>());

			var pollOption = new Mock<IObjectFactory<IPollOption>>();
			pollOption.Setup(_ => _.FetchChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.FetchChild<PollOption>(_[0] as MvpollOption));

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => entities.Object);
			builder.Register<IObjectFactory<BusinessList<IPollOption>>>(_ => pollOptions.Object);
			builder.Register<IObjectFactory<IPollOption>>(_ => pollOption.Object);

			var identity = new Mock<IUserIdentity>(MockBehavior.Strict);
			identity.Setup(_ => _.UserID).Returns(userId);
			identity.Setup(_ => _.IsInRole(UserRoles.Admin)).Returns(false);

			var principal = new Mock<IPrincipal>(MockBehavior.Strict);
			principal.Setup(_ => _.Identity).Returns(identity.Object);

			using (principal.Object.Bind(() => ApplicationContext.User))
			{
				using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
					.Bind(() => ApplicationContext.DataPortalActivator))
				{
					var poll = new DataPortal<Poll>().Fetch(entity.PollId);
					poll.Delete();
					poll = poll.Save();

					polls.Local.Count.Should().Be(2);
					var deletedPoll = polls.Local[1];
					deletedPoll.PollDeletedDate.Should().HaveValue();
					deletedPoll.PollDeletedFlag.Value.Should().BeTrue();
				}
			}

			principal.VerifyAll();
			identity.VerifyAll();
			entities.VerifyAll();
		}
	}
}
