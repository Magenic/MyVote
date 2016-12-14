using Autofac;
using Csla;
using Moq;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;
using MyVote.BusinessObjects.Net.Tests.Extensions;
using MyVote.BusinessObjects.Rules;
using MyVote.Data.Entities;
using Spackle;
using Spackle.Extensions;
using System;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace MyVote.BusinessObjects.Net.Tests
{
	public sealed class PollRuleTests
	{
		[Fact]
		public void ChangePollCategoryIdToValidValue()
		{
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();
			var pollCategoryId = generator.Generate<int>();

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());
			builder.Register<IObjectFactory<BusinessList<IPollOption>>>(_ => Mock.Of<IObjectFactory<BusinessList<IPollOption>>>());
			builder.Register<IObjectFactory<IPollOption>>(_ => Mock.Of<IObjectFactory<IPollOption>>());

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var poll = DataPortal.Create<Poll>(userId);
				poll.PollCategoryID = pollCategoryId;

				poll.BrokenRulesCollection.AssertRuleCount(Poll.PollCategoryIDProperty, 0);
			}
		}

		[Fact]
		public void ChangePollEndDateToValidValue()
		{
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();
			var endDate = generator.Generate<DateTime>();

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());
			builder.Register<IObjectFactory<BusinessList<IPollOption>>>(_ => Mock.Of<IObjectFactory<BusinessList<IPollOption>>>());
			builder.Register<IObjectFactory<IPollOption>>(_ => Mock.Of<IObjectFactory<IPollOption>>());

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var poll = DataPortal.Create<Poll>(userId);
				poll.PollEndDate = endDate;

				poll.BrokenRulesCollection.AssertValidationRuleExists<RequiredAttribute>(
					Poll.PollEndDateProperty, false);
			}
		}

		[Fact]
		public void ChangePollOptionsToValidValue()
		{
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();

			var pollOptions = new Mock<IObjectFactory<BusinessList<IPollOption>>>();
			pollOptions.Setup(_ => _.CreateChild()).Returns(new BusinessList<IPollOption>());

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());
			builder.Register<IObjectFactory<BusinessList<IPollOption>>>(_ => pollOptions.Object);
			builder.Register<IObjectFactory<IPollOption>>(_ => Mock.Of<IObjectFactory<IPollOption>>());

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var poll = DataPortal.Create<Poll>(userId);
				poll.PollOptions.Add(DataPortal.CreateChild<PollOption>());
				poll.PollOptions.Add(DataPortal.CreateChild<PollOption>());

				poll.BrokenRulesCollection.AssertRuleCount(Poll.PollOptionsProperty, 0);
			}
		}


		[Fact]
		public void ChangePollQuestionToInvalidValue()
		{
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();
			var pollQuestion = new string(generator.Generate<string>()[0], 1001);

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());
			builder.Register<IObjectFactory<BusinessList<IPollOption>>>(_ => Mock.Of<IObjectFactory<BusinessList<IPollOption>>>());
			builder.Register<IObjectFactory<IPollOption>>(_ => Mock.Of<IObjectFactory<IPollOption>>());

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var poll = DataPortal.Create<Poll>(userId);
				poll.PollQuestion = pollQuestion;

				poll.BrokenRulesCollection.AssertRuleCount(Poll.PollQuestionProperty, 1);
				poll.BrokenRulesCollection.AssertValidationRuleExists<StringLengthAttribute>(
					Poll.PollQuestionProperty, true);
			}
		}

		[Fact]
		public void ChangePollQuestionToValidValue()
		{
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();
			var pollQuestion = new string(generator.Generate<string>()[0], 1);

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());
			builder.Register<IObjectFactory<BusinessList<IPollOption>>>(_ => Mock.Of<IObjectFactory<BusinessList<IPollOption>>>());
			builder.Register<IObjectFactory<IPollOption>>(_ => Mock.Of<IObjectFactory<IPollOption>>());

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var poll = DataPortal.Create<Poll>(userId);
				poll.PollQuestion = pollQuestion;

				poll.BrokenRulesCollection.AssertRuleCount(Poll.PollQuestionProperty, 0);
			}
		}

		[Fact]
		public void ChangePollStartDateAfterEndDate()
		{
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();
			var startDate = generator.Generate<DateTime>();
			var endDate = startDate.AddDays(-2);

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());
			builder.Register<IObjectFactory<BusinessList<IPollOption>>>(_ => Mock.Of<IObjectFactory<BusinessList<IPollOption>>>());
			builder.Register<IObjectFactory<IPollOption>>(_ => Mock.Of<IObjectFactory<IPollOption>>());

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var poll = DataPortal.Create<Poll>(userId);
				poll.PollStartDate = startDate;
				poll.PollEndDate = endDate;

				poll.BrokenRulesCollection.AssertBusinessRuleExists<StartAndEndDateRule>(
					Poll.PollStartDateProperty, true);
				poll.BrokenRulesCollection.AssertBusinessRuleExists<StartAndEndDateRule>(
					Poll.PollEndDateProperty, true);
			}
		}

		[Fact]
		public void ChangePollStartDateBeforeEndDate()
		{
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();
			var startDate = generator.Generate<DateTime>();
			var endDate = startDate.AddDays(2);

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());
			builder.Register<IObjectFactory<BusinessList<IPollOption>>>(_ => Mock.Of<IObjectFactory<BusinessList<IPollOption>>>());
			builder.Register<IObjectFactory<IPollOption>>(_ => Mock.Of<IObjectFactory<IPollOption>>());

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var poll = DataPortal.Create<Poll>(userId);
				poll.PollStartDate = startDate;
				poll.PollEndDate = endDate;

				poll.BrokenRulesCollection.AssertBusinessRuleExists<StartAndEndDateRule>(
					Poll.PollStartDateProperty, false);
				poll.BrokenRulesCollection.AssertBusinessRuleExists<StartAndEndDateRule>(
					Poll.PollEndDateProperty, false);
			}
		}

		[Fact]
		public void ChangePollStartDateToValidValue()
		{
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();
			var startDate = generator.Generate<DateTime>();

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());
			builder.Register<IObjectFactory<BusinessList<IPollOption>>>(_ => Mock.Of<IObjectFactory<BusinessList<IPollOption>>>());
			builder.Register<IObjectFactory<IPollOption>>(_ => Mock.Of<IObjectFactory<IPollOption>>());

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var poll = DataPortal.Create<Poll>(userId);
				poll.PollStartDate = startDate;

				poll.BrokenRulesCollection.AssertValidationRuleExists<RequiredAttribute>(
					Poll.PollStartDateProperty, false);
			}
		}

		[Fact]
		public void ChangePollMinAndMaxAnswersToInvalidValue()
		{
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());
			builder.Register<IObjectFactory<BusinessList<IPollOption>>>(_ => Mock.Of<IObjectFactory<BusinessList<IPollOption>>>());
			builder.Register<IObjectFactory<IPollOption>>(_ => Mock.Of<IObjectFactory<IPollOption>>());

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var poll = DataPortal.Create<Poll>(userId);
				poll.PollMaxAnswers = 1;
				poll.PollMinAnswers = 2;

				poll.BrokenRulesCollection.AssertBusinessRuleExists<MinimumAndMaximumPollOptionRule>(
					Poll.PollMaxAnswersProperty, true);
				poll.BrokenRulesCollection.AssertBusinessRuleExists<MinimumAndMaximumPollOptionRule>(
					Poll.PollMinAnswersProperty, true);
			}
		}

		[Fact]
		public void ChangePollMinAndMaxAnswersToValidValue()
		{
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());
			builder.Register<IObjectFactory<BusinessList<IPollOption>>>(_ => Mock.Of<IObjectFactory<BusinessList<IPollOption>>>());
			builder.Register<IObjectFactory<IPollOption>>(_ => Mock.Of<IObjectFactory<IPollOption>>());

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var poll = DataPortal.Create<Poll>(userId);
				poll.PollMaxAnswers = 2;
				poll.PollMinAnswers = 2;

				poll.BrokenRulesCollection.AssertBusinessRuleExists<MinimumAndMaximumPollOptionRule>(
					Poll.PollMaxAnswersProperty, false);
				poll.BrokenRulesCollection.AssertBusinessRuleExists<MinimumAndMaximumPollOptionRule>(
					Poll.PollMinAnswersProperty, false);
			}
		}

		[Fact]
		public void ChangePollMaxAnswersToZero()
		{
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());
			builder.Register<IObjectFactory<BusinessList<IPollOption>>>(_ => Mock.Of<IObjectFactory<BusinessList<IPollOption>>>());
			builder.Register<IObjectFactory<IPollOption>>(_ => Mock.Of<IObjectFactory<IPollOption>>());

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var poll = DataPortal.Create<Poll>(userId);
				poll.PollMaxAnswers = 0;

				poll.BrokenRulesCollection.AssertRuleCount(Poll.PollMaxAnswersProperty, 1);
				poll.BrokenRulesCollection.AssertBusinessRuleExists<PollMaxAnswersRule>(
					Poll.PollMaxAnswersProperty, true);
			}
		}

		[Fact]
		public void ChangePollMaxAnswersToPositiveValueAndAnswerCountIsGreater()
		{
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();

			var pollOptions = new Mock<IObjectFactory<BusinessList<IPollOption>>>();
			pollOptions.Setup(_ => _.CreateChild()).Returns(new BusinessList<IPollOption>());

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());
			builder.Register<IObjectFactory<BusinessList<IPollOption>>>(_ => pollOptions.Object);
			builder.Register<IObjectFactory<IPollOption>>(_ => Mock.Of<IObjectFactory<IPollOption>>());

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var poll = DataPortal.Create<Poll>(userId);
				poll.PollOptions.Add(DataPortal.CreateChild<PollOption>());
				poll.PollMaxAnswers = 2;

				poll.BrokenRulesCollection.AssertRuleCount(Poll.PollMaxAnswersProperty, 1);
				poll.BrokenRulesCollection.AssertBusinessRuleExists<PollMaxAnswersRule>(
					Poll.PollMaxAnswersProperty, true);
			}
		}

		[Fact]
		public void ChangePollMaxAnswersToPositiveValueAndAnswerCountIsEqual()
		{
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();

			var pollOptions = new Mock<IObjectFactory<BusinessList<IPollOption>>>();
			pollOptions.Setup(_ => _.CreateChild()).Returns(new BusinessList<IPollOption>());

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());
			builder.Register<IObjectFactory<BusinessList<IPollOption>>>(_ => pollOptions.Object);
			builder.Register<IObjectFactory<IPollOption>>(_ => Mock.Of<IObjectFactory<IPollOption>>());

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var poll = DataPortal.Create<Poll>(userId);
				poll.PollOptions.Add(DataPortal.CreateChild<PollOption>());
				poll.PollMaxAnswers = 1;

				poll.BrokenRulesCollection.AssertRuleCount(Poll.PollMaxAnswersProperty, 0);
			}
		}

		[Fact]
		public void ChangePollMinAnswersToZero()
		{
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());
			builder.Register<IObjectFactory<BusinessList<IPollOption>>>(_ => Mock.Of<IObjectFactory<BusinessList<IPollOption>>>());
			builder.Register<IObjectFactory<IPollOption>>(_ => Mock.Of<IObjectFactory<IPollOption>>());

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var poll = DataPortal.Create<Poll>(userId);
				poll.PollMinAnswers = 0;

				poll.BrokenRulesCollection.AssertRuleCount(Poll.PollMinAnswersProperty, 1);
				poll.BrokenRulesCollection.AssertBusinessRuleExists<PollMinAnswersRule>(
					Poll.PollMinAnswersProperty, true);
			}
		}

		[Fact]
		public void ChangePollMinAnswersToPositiveValueAndAnswerCountIsGreater()
		{
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();

			var pollOptions = new Mock<IObjectFactory<BusinessList<IPollOption>>>();
			pollOptions.Setup(_ => _.CreateChild()).Returns(new BusinessList<IPollOption>());

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());
			builder.Register<IObjectFactory<BusinessList<IPollOption>>>(_ => pollOptions.Object);
			builder.Register<IObjectFactory<IPollOption>>(_ => Mock.Of<IObjectFactory<IPollOption>>());

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var poll = DataPortal.Create<Poll>(userId);
				poll.PollOptions.Add(DataPortal.CreateChild<PollOption>());
				poll.PollMinAnswers = 2;

				poll.BrokenRulesCollection.AssertRuleCount(Poll.PollMinAnswersProperty, 1);
				poll.BrokenRulesCollection.AssertBusinessRuleExists<PollMinAnswersRule>(
					Poll.PollMinAnswersProperty, true);
			}
		}

		[Fact]
		public void ChangePollMinAnswersToPositiveValueAndAnswerCountIsEqual()
		{
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();

			var pollOptions = new Mock<IObjectFactory<BusinessList<IPollOption>>>();
			pollOptions.Setup(_ => _.CreateChild()).Returns(new BusinessList<IPollOption>());

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());
			builder.Register<IObjectFactory<BusinessList<IPollOption>>>(_ => pollOptions.Object);
			builder.Register<IObjectFactory<IPollOption>>(_ => Mock.Of<IObjectFactory<IPollOption>>());

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var poll = DataPortal.Create<Poll>(userId);
				poll.PollOptions.Add(DataPortal.CreateChild<PollOption>());
				poll.PollMinAnswers = 1;

				poll.BrokenRulesCollection.AssertRuleCount(Poll.PollMinAnswersProperty, 0);
			}
		}
	}
}
