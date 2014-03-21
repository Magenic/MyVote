using System;
using System.ComponentModel.DataAnnotations;
using Autofac;
using Csla;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyVote.BusinessObjects.Net.Tests.Extensions;
using MyVote.BusinessObjects.Rules;
using MyVote.Repository;
using Spackle;
using Spackle.Extensions;

namespace MyVote.BusinessObjects.Net.Tests
{
	[TestClass]
	public sealed class PollRuleTests
	{
		[TestMethod]
		public void ChangePollCategoryIdToValidValue()
		{
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();
			var pollCategoryId = generator.Generate<int>();

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
			{
				var poll = DataPortal.Create<Poll>(userId);
				poll.PollCategoryID = pollCategoryId;

				poll.BrokenRulesCollection.AssertRuleCount(Poll.PollCategoryIDProperty, 0);
			}
		}

		[TestMethod]
		public void ChangePollEndDateToValidValue()
		{
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();
			var endDate = generator.Generate<DateTime>();

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
			{
				var poll = DataPortal.Create<Poll>(userId);
				poll.PollEndDate = endDate;

				poll.BrokenRulesCollection.AssertValidationRuleExists<RequiredAttribute>(
					Poll.PollEndDateProperty, false);
			}
		}

		[TestMethod]
		public void ChangePollOptionsToValidValue()
		{
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
			{
				var poll = DataPortal.Create<Poll>(userId);
				poll.PollOptions.Add(DataPortal.CreateChild<PollOption>());
				poll.PollOptions.Add(DataPortal.CreateChild<PollOption>());

				poll.BrokenRulesCollection.AssertRuleCount(Poll.PollOptionsProperty, 0);
			}
		}


		[TestMethod]
		public void ChangePollQuestionToInvalidValue()
		{
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();
			var pollQuestion = new string(generator.Generate<string>()[0], 1001);

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
			{
				var poll = DataPortal.Create<Poll>(userId);
				poll.PollQuestion = pollQuestion;

				poll.BrokenRulesCollection.AssertRuleCount(Poll.PollQuestionProperty, 1);
				poll.BrokenRulesCollection.AssertValidationRuleExists<StringLengthAttribute>(
					Poll.PollQuestionProperty, true);
			}
		}

		[TestMethod]
		public void ChangePollQuestionToValidValue()
		{
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();
			var pollQuestion = new string(generator.Generate<string>()[0], 1);

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
			{
				var poll = DataPortal.Create<Poll>(userId);
				poll.PollQuestion = pollQuestion;

				poll.BrokenRulesCollection.AssertRuleCount(Poll.PollQuestionProperty, 0);
			}
		}

		[TestMethod]
		public void ChangePollStartDateAfterEndDate()
		{
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();
			var startDate = generator.Generate<DateTime>();
			var endDate = startDate.AddDays(-2);

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
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

		[TestMethod]
		public void ChangePollStartDateBeforeEndDate()
		{
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();
			var startDate = generator.Generate<DateTime>();
			var endDate = startDate.AddDays(2);

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
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

		[TestMethod]
		public void ChangePollStartDateToValidValue()
		{
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();
			var startDate = generator.Generate<DateTime>();

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
			{
				var poll = DataPortal.Create<Poll>(userId);
				poll.PollStartDate = startDate;

				poll.BrokenRulesCollection.AssertValidationRuleExists<RequiredAttribute>(
					Poll.PollStartDateProperty, false);
			}
		}

		[TestMethod]
		public void ChangePollMinAndMaxAnswersToInvalidValue()
		{
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
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

		[TestMethod]
		public void ChangePollMinAndMaxAnswersToValidValue()
		{
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
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

		[TestMethod]
		public void ChangePollMaxAnswersToZero()
		{
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
			{
				var poll = DataPortal.Create<Poll>(userId);
				poll.PollMaxAnswers = 0;

				poll.BrokenRulesCollection.AssertRuleCount(Poll.PollMaxAnswersProperty, 1);
				poll.BrokenRulesCollection.AssertBusinessRuleExists<PollMaxAnswersRule>(
					Poll.PollMaxAnswersProperty, true);
			}
		}

		[TestMethod]
		public void ChangePollMaxAnswersToPositiveValueAndAnswerCountIsGreater()
		{
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
			{
				var poll = DataPortal.Create<Poll>(userId);
				poll.PollOptions.Add(DataPortal.CreateChild<PollOption>());
				poll.PollMaxAnswers = 2;

				poll.BrokenRulesCollection.AssertRuleCount(Poll.PollMaxAnswersProperty, 1);
				poll.BrokenRulesCollection.AssertBusinessRuleExists<PollMaxAnswersRule>(
					Poll.PollMaxAnswersProperty, true);
			}
		}

		[TestMethod]
		public void ChangePollMaxAnswersToPositiveValueAndAnswerCountIsEqual()
		{
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
			{
				var poll = DataPortal.Create<Poll>(userId);
				poll.PollOptions.Add(DataPortal.CreateChild<PollOption>());
				poll.PollMaxAnswers = 1;

				poll.BrokenRulesCollection.AssertRuleCount(Poll.PollMaxAnswersProperty, 0);
			}
		}

		[TestMethod]
		public void ChangePollMinAnswersToZero()
		{
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
			{
				var poll = DataPortal.Create<Poll>(userId);
				poll.PollMinAnswers = 0;

				poll.BrokenRulesCollection.AssertRuleCount(Poll.PollMinAnswersProperty, 1);
				poll.BrokenRulesCollection.AssertBusinessRuleExists<PollMinAnswersRule>(
					Poll.PollMinAnswersProperty, true);
			}
		}

		[TestMethod]
		public void ChangePollMinAnswersToPositiveValueAndAnswerCountIsGreater()
		{
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
			{
				var poll = DataPortal.Create<Poll>(userId);
				poll.PollOptions.Add(DataPortal.CreateChild<PollOption>());
				poll.PollMinAnswers = 2;

				poll.BrokenRulesCollection.AssertRuleCount(Poll.PollMinAnswersProperty, 1);
				poll.BrokenRulesCollection.AssertBusinessRuleExists<PollMinAnswersRule>(
					Poll.PollMinAnswersProperty, true);
			}
		}

		[TestMethod]
		public void ChangePollMinAnswersToPositiveValueAndAnswerCountIsEqual()
		{
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
			{
				var poll = DataPortal.Create<Poll>(userId);
				poll.PollOptions.Add(DataPortal.CreateChild<PollOption>());
				poll.PollMinAnswers = 1;

				poll.BrokenRulesCollection.AssertRuleCount(Poll.PollMinAnswersProperty, 0);
			}
		}
	}
}
