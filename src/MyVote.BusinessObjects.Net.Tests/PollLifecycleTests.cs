using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Security.Principal;
using Autofac;
using Csla;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Net.Tests.Extensions;
using MyVote.BusinessObjects.Rules;
using MyVote.Core.Extensions;
using MyVote.Repository;
using Spackle;
using Spackle.Extensions;

namespace MyVote.BusinessObjects.Net.Tests
{
	[TestClass]
	public sealed class PollLifecycleTests
	{
		private static PollOption CreatePollOption()
		{
			var option = DataPortal.CreateChild<PollOption>();
			option.OptionPosition = 1;
			option.OptionText = "1";
			return option;
		}

		[TestMethod]
		public void Create()
		{
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
			{
				var poll = new DataPortal<Poll>().Create(userId);

				Assert.IsNull(poll.PollAdminRemovedFlag, poll.GetPropertyName(_ => _.PollAdminRemovedFlag));
				Assert.IsNull(poll.PollCategoryID, poll.GetPropertyName(_ => _.PollCategoryID));
				Assert.IsNull(poll.PollDateRemoved, poll.GetPropertyName(_ => _.PollDateRemoved));
				Assert.IsNull(poll.PollDeletedDate, poll.GetPropertyName(_ => _.PollDeletedDate));
				Assert.IsNull(poll.PollDeletedFlag, poll.GetPropertyName(_ => _.PollDeletedFlag));
				Assert.AreEqual(string.Empty, poll.PollDescription, poll.GetPropertyName(_ => _.PollDescription));
				Assert.IsNull(poll.PollEndDate, poll.GetPropertyName(_ => _.PollEndDate));
				Assert.IsNull(poll.PollID, poll.GetPropertyName(_ => _.PollID));
				Assert.AreEqual(string.Empty, poll.PollImageLink, poll.GetPropertyName(_ => _.PollImageLink));
				Assert.IsNull(poll.PollMaxAnswers, poll.GetPropertyName(_ => _.PollMaxAnswers));
				Assert.IsNull(poll.PollMinAnswers, poll.GetPropertyName(_ => _.PollMinAnswers));
				Assert.AreEqual(string.Empty, poll.PollQuestion, poll.GetPropertyName(_ => _.PollQuestion));
				Assert.IsNull(poll.PollStartDate, poll.GetPropertyName(_ => _.PollStartDate));
				Assert.AreEqual(userId, poll.UserID, poll.GetPropertyName(_ => _.UserID));
				Assert.AreEqual(0, poll.PollOptions.Count, poll.GetPropertyName(_ => _.PollOptions));

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
		}

		[TestMethod]
		public void Fetch()
		{
			var entity = EntityCreator.Create<MVPoll>(_ => _.PollDeletedFlag = false);
			entity.MVPollOptions = new List<MVPollOption> { EntityCreator.Create<MVPollOption>() };

			var entities = new Mock<IEntities>(MockBehavior.Strict);
			entities.Setup(_ => _.MVPolls).Returns(new InMemoryDbSet<MVPoll> { entity });
			entities.Setup(_ => _.Dispose());

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => entities.Object);

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
			{
				var poll = new DataPortal<Poll>().Fetch(entity.PollID);

				Assert.AreEqual(entity.PollAdminRemovedFlag, poll.PollAdminRemovedFlag, poll.GetPropertyName(_ => _.PollAdminRemovedFlag));
				Assert.AreEqual(entity.PollCategoryID, poll.PollCategoryID, poll.GetPropertyName(_ => _.PollCategoryID));
				Assert.AreEqual(entity.PollDateRemoved, poll.PollDateRemoved, poll.GetPropertyName(_ => _.PollDateRemoved));
				Assert.AreEqual(entity.PollDeletedDate, poll.PollDeletedDate, poll.GetPropertyName(_ => _.PollDeletedDate));
				Assert.AreEqual(entity.PollDeletedFlag, poll.PollDeletedFlag, poll.GetPropertyName(_ => _.PollDeletedFlag));
				Assert.AreEqual(entity.PollDescription, poll.PollDescription, poll.GetPropertyName(_ => _.PollDescription));
				Assert.AreEqual(entity.PollEndDate, poll.PollEndDate, poll.GetPropertyName(_ => _.PollEndDate));
				Assert.AreEqual(entity.PollID, poll.PollID, poll.GetPropertyName(_ => _.PollID));
				Assert.AreEqual(entity.PollImageLink, poll.PollImageLink, poll.GetPropertyName(_ => _.PollImageLink));
				Assert.AreEqual(entity.PollMaxAnswers, poll.PollMaxAnswers, poll.GetPropertyName(_ => _.PollMaxAnswers));
				Assert.AreEqual(entity.PollMinAnswers, poll.PollMinAnswers, poll.GetPropertyName(_ => _.PollMinAnswers));
				Assert.AreEqual(entity.PollQuestion, poll.PollQuestion, poll.GetPropertyName(_ => _.PollQuestion));
				Assert.AreEqual(entity.PollStartDate, poll.PollStartDate, poll.GetPropertyName(_ => _.PollStartDate));
				Assert.AreEqual(entity.UserID, poll.UserID, poll.GetPropertyName(_ => _.UserID));
				Assert.AreEqual(1, poll.PollOptions.Count, poll.GetPropertyName(_ => _.PollOptions));
			}

			entities.VerifyAll();
		}

		[TestMethod]
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

			var polls = new InMemoryDbSet<MVPoll>();
			var pollOptions = new InMemoryDbSet<MVPollOption>();

			var entities = new Mock<IEntities>(MockBehavior.Strict);
			entities.Setup(_ => _.MVPolls).Returns(polls);
			entities.Setup(_ => _.MVPollOptions).Returns(pollOptions);
			entities.Setup(_ => _.SaveChanges()).Callback(() => polls.Local[0].PollID = pollId)
				.Returns(1);
			entities.Setup(_ => _.Dispose());

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => entities.Object);

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
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

				Assert.AreEqual(pollAdminRemoveFlag, poll.PollAdminRemovedFlag, poll.GetPropertyName(_ => _.PollAdminRemovedFlag));
				Assert.AreEqual(pollCategoryId, poll.PollCategoryID, poll.GetPropertyName(_ => _.PollCategoryID));
				Assert.AreEqual(pollDateRemoved, poll.PollDateRemoved, poll.GetPropertyName(_ => _.PollDateRemoved));
				Assert.AreEqual(pollDeletedDate, poll.PollDeletedDate, poll.GetPropertyName(_ => _.PollDeletedDate));
				Assert.AreEqual(pollDeletedFlag, poll.PollDeletedFlag, poll.GetPropertyName(_ => _.PollDeletedFlag));
				Assert.AreEqual(pollDescription, poll.PollDescription, poll.GetPropertyName(_ => _.PollDescription));
				Assert.AreEqual(pollEndDate.ToUniversalTime(), poll.PollEndDate, poll.GetPropertyName(_ => _.PollEndDate));
				Assert.AreEqual(pollId, poll.PollID, poll.GetPropertyName(_ => _.PollID));
				Assert.AreEqual(pollImageLink, poll.PollImageLink, poll.GetPropertyName(_ => _.PollImageLink));
				Assert.AreEqual(pollMaxAnswers, poll.PollMaxAnswers, poll.GetPropertyName(_ => _.PollMaxAnswers));
				Assert.AreEqual(pollMinAnswers, poll.PollMinAnswers, poll.GetPropertyName(_ => _.PollMinAnswers));
				Assert.AreEqual(pollQuestion, poll.PollQuestion, poll.GetPropertyName(_ => _.PollQuestion));
				Assert.AreEqual(pollStartDate.ToUniversalTime(), poll.PollStartDate, poll.GetPropertyName(_ => _.PollStartDate));
				Assert.AreEqual(userId, poll.UserID, poll.GetPropertyName(_ => _.UserID));
			}

			entities.VerifyAll();
		}

		[TestMethod]
		public void Update()
		{
			var now = DateTime.UtcNow;

			var entity = EntityCreator.Create<MVPoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollMinAnswers = 1;
				_.PollMaxAnswers = 2;
				_.PollStartDate = now;
				_.PollEndDate = now.AddDays(2);
			});
			entity.MVPollOptions = new List<MVPollOption> 
			{ 
				EntityCreator.Create<MVPollOption>(),
				EntityCreator.Create<MVPollOption>(),
				EntityCreator.Create<MVPollOption>() 
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

			var entities = new Mock<IEntities>(MockBehavior.Strict);
			entities.Setup(_ => _.MVPolls).Returns(new InMemoryDbSet<MVPoll> { entity });
			entities.Setup(_ => _.SetState(It.IsAny<MVPoll>(), EntityState.Modified));
			entities.Setup(_ => _.SaveChanges()).Returns(1);
			entities.Setup(_ => _.Dispose());

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => entities.Object);

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
			{
				var poll = new DataPortal<Poll>().Fetch(entity.PollID);
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

				Assert.AreEqual(newPollAdminRemoveFlag, poll.PollAdminRemovedFlag, poll.GetPropertyName(_ => _.PollAdminRemovedFlag));
				Assert.AreEqual(newPollCategoryId, poll.PollCategoryID, poll.GetPropertyName(_ => _.PollCategoryID));
				Assert.AreEqual(newPollDateRemoved, poll.PollDateRemoved, poll.GetPropertyName(_ => _.PollDateRemoved));
				Assert.AreEqual(newPollDeletedDate, poll.PollDeletedDate, poll.GetPropertyName(_ => _.PollDeletedDate));
				Assert.AreEqual(newPollDeletedFlag, poll.PollDeletedFlag, poll.GetPropertyName(_ => _.PollDeletedFlag));
				Assert.AreEqual(newPollDescription, poll.PollDescription, poll.GetPropertyName(_ => _.PollDescription));
				Assert.AreEqual(newPollEndDate.ToUniversalTime(), poll.PollEndDate, poll.GetPropertyName(_ => _.PollEndDate));
				Assert.AreEqual(newPollImageLink, poll.PollImageLink, poll.GetPropertyName(_ => _.PollImageLink));
				Assert.AreEqual(newPollMaxAnswers, poll.PollMaxAnswers, poll.GetPropertyName(_ => _.PollMaxAnswers));
				Assert.AreEqual(newPollMinAnswers, poll.PollMinAnswers, poll.GetPropertyName(_ => _.PollMinAnswers));
				Assert.AreEqual(newPollQuestion, poll.PollQuestion, poll.GetPropertyName(_ => _.PollQuestion));
				Assert.AreEqual(newPollStartDate.ToUniversalTime(), poll.PollStartDate, poll.GetPropertyName(_ => _.PollStartDate));
			}

			entities.VerifyAll();
		}

		[TestMethod]
		public void Delete()
		{
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();
			var now = DateTime.UtcNow;

			var entity = EntityCreator.Create<MVPoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollMinAnswers = 1;
				_.PollMaxAnswers = 2;
				_.PollStartDate = now;
				_.PollEndDate = now.AddDays(2);
				_.UserID = userId;
			});
			entity.MVPollOptions = new List<MVPollOption> 
			{ 
				EntityCreator.Create<MVPollOption>(),
				EntityCreator.Create<MVPollOption>(),
				EntityCreator.Create<MVPollOption>() 
			};

			var polls = new InMemoryDbSet<MVPoll> { entity };

			var entities = new Mock<IEntities>(MockBehavior.Strict);
			entities.Setup(_ => _.MVPolls).Returns(polls);
			entities.Setup(_ => _.SetState(It.IsAny<MVPoll>(), EntityState.Modified));
			entities.Setup(_ => _.SaveChanges()).Returns(1);
			entities.Setup(_ => _.Dispose());

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => entities.Object);

			var identity = new Mock<IUserIdentity>(MockBehavior.Strict);
			identity.Setup(_ => _.UserID).Returns(userId);
			identity.Setup(_ => _.IsInRole(UserRoles.Admin)).Returns(false);

			var principal = new Mock<IPrincipal>(MockBehavior.Strict);
			principal.Setup(_ => _.Identity).Returns(identity.Object);

			using (principal.Object.Bind(() => ApplicationContext.User))
			{
				using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
				{
					var poll = new DataPortal<Poll>().Fetch(entity.PollID);
					poll.Delete();
					poll.Save();

					Assert.AreEqual(2, polls.Local.Count);
					var deletedPoll = polls.Local[1];
					Assert.IsNotNull(deletedPoll.PollDeletedDate, deletedPoll.GetPropertyName(_ => _.PollDeletedDate));
					Assert.IsTrue(deletedPoll.PollDeletedFlag.Value, deletedPoll.GetPropertyName(_ => _.PollDeletedFlag));
				}
			}

			principal.VerifyAll();
			identity.VerifyAll();
			entities.VerifyAll();
		}
	}
}
