using Autofac;
using Csla;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Net.Tests.Extensions;
using MyVote.Data.Entities;
using Spackle;
using Spackle.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace MyVote.BusinessObjects.Net.Tests
{
	[TestClass]
	public sealed class PollOptionLifecycleTests
	{
		[TestMethod]
		public void Create()
		{
			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var pollOption = DataPortal.CreateChild<PollOption>();

				Assert.IsNull(pollOption.OptionPosition, nameof(pollOption.OptionPosition));
				Assert.AreEqual(string.Empty, pollOption.OptionText, nameof(pollOption.OptionText));
				Assert.IsNull(pollOption.PollID, nameof(pollOption.PollID));
				Assert.IsNull(pollOption.PollOptionID, nameof(pollOption.PollOptionID));

				pollOption.BrokenRulesCollection.AssertRuleCount(2);
				pollOption.BrokenRulesCollection.AssertRuleCount(PollOption.OptionPositionProperty, 1);
				pollOption.BrokenRulesCollection.AssertValidationRuleExists<RequiredAttribute>(
					PollOption.OptionPositionProperty, true);
				pollOption.BrokenRulesCollection.AssertRuleCount(PollOption.OptionTextProperty, 1);
				pollOption.BrokenRulesCollection.AssertValidationRuleExists<RequiredAttribute>(
					PollOption.OptionTextProperty, true);
			}
		}

		[TestMethod]
		public void Fetch()
		{
			var entity = EntityCreator.Create<MVPollOption>();

			var entities = new Mock<IEntities>(MockBehavior.Strict);
			entities.Setup(_ => _.Dispose());

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => entities.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var pollOption = DataPortal.FetchChild<PollOption>(entity);

				Assert.AreEqual(entity.OptionPosition, pollOption.OptionPosition, nameof(pollOption.OptionPosition));
				Assert.AreEqual(entity.OptionText, pollOption.OptionText, nameof(pollOption.OptionText));
				Assert.AreEqual(entity.PollID, pollOption.PollID, nameof(pollOption.PollID));
				Assert.AreEqual(entity.PollOptionID, pollOption.PollOptionID, nameof(pollOption.PollOptionID));
			}

			entities.VerifyAll();
		}

		[TestMethod]
		public void Insert()
		{
			var generator = new RandomObjectGenerator();
			var optionPosition = generator.Generate<short>();
			var optionText = generator.Generate<string>();
			var pollId = generator.Generate<int>();
			var pollOptionId = generator.Generate<int>();

			var poll = new Mock<IPoll>(MockBehavior.Strict);
			poll.Setup(_ => _.PollID).Returns(pollId);

			var pollOptions = new InMemoryDbSet<MVPollOption>();
			var entities = new Mock<IEntities>(MockBehavior.Strict);
			entities.Setup(_ => _.MVPollOptions)
				.Returns(pollOptions);
			entities.Setup(_ => _.SaveChanges())
				.Callback(() => pollOptions.Local[0].PollOptionID = pollOptionId)
				.Returns(1);
			entities.Setup(_ => _.Dispose());

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => entities.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var pollOption = DataPortal.CreateChild<PollOption>();
				pollOption.OptionPosition = optionPosition;
				pollOption.OptionText = optionText;

				DataPortal.UpdateChild(pollOption, poll.Object);

				Assert.AreEqual(pollId, pollOption.PollID, nameof(pollOption.PollID));
				Assert.AreEqual(pollOptionId, pollOption.PollOptionID, nameof(pollOption.PollOptionID));
				Assert.AreEqual(optionPosition, pollOption.OptionPosition, nameof(pollOption.OptionPosition));
				Assert.AreEqual(optionText, pollOption.OptionText, nameof(pollOption.OptionText));
			}

			entities.VerifyAll();
			poll.VerifyAll();
		}

		[TestMethod]
		public void Update()
		{
			var generator = new RandomObjectGenerator();
			var newOptionPosition = generator.Generate<short>();
			var newOptionText = generator.Generate<string>();

			var entity = EntityCreator.Create<MVPollOption>();

			var poll = Mock.Of<IPoll>();

			var entities = new Mock<IEntities>(MockBehavior.Strict);
			entities.Setup(_ => _.MVPollOptions)
				.Returns(new InMemoryDbSet<MVPollOption> { entity });
			entities.Setup(_ => _.SetState(It.IsAny<MVPollOption>(), EntityState.Modified));
			entities.Setup(_ => _.SaveChanges()).Returns(1);
			entities.Setup(_ => _.Dispose());

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => entities.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var pollOption = DataPortal.FetchChild<PollOption>(entity);
				pollOption.OptionPosition = newOptionPosition;
				pollOption.OptionText = newOptionText;

				DataPortal.UpdateChild(pollOption, poll);

				Assert.AreEqual(newOptionPosition, pollOption.OptionPosition, nameof(pollOption.OptionPosition));
				Assert.AreEqual(newOptionText, pollOption.OptionText, nameof(pollOption.OptionText));
			}

			entities.VerifyAll();
		}
	}
}
