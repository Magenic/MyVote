using System.ComponentModel.DataAnnotations;
using Autofac;
using Csla;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Net.Tests.Extensions;
using MyVote.Core.Extensions;
using MyVote.Repository;
using Spackle;
using Spackle.Extensions;

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

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
			{
				var pollOption = DataPortal.CreateChild<PollOption>();

				Assert.IsNull(pollOption.OptionPosition, pollOption.GetPropertyName(_ => _.OptionPosition));
				Assert.AreEqual(string.Empty, pollOption.OptionText, pollOption.GetPropertyName(_ => _.OptionText));
				Assert.IsNull(pollOption.PollID, pollOption.GetPropertyName(_ => _.PollID));
				Assert.IsNull(pollOption.PollOptionID, pollOption.GetPropertyName(_ => _.PollOptionID));

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

			var entities = new Mock<IEntities>();
			entities.Setup(_ => _.MVPollOptions)
				.Returns(new InMemoryDbSet<MVPollOption> { entity });

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => entities.Object);

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
			{
				var pollOption = DataPortal.FetchChild<PollOption>(entity);

				Assert.AreEqual(entity.OptionPosition, pollOption.OptionPosition, pollOption.GetPropertyName(_ => _.OptionPosition));
				Assert.AreEqual(entity.OptionText, pollOption.OptionText, pollOption.GetPropertyName(_ => _.OptionText));
				Assert.AreEqual(entity.PollID, pollOption.PollID, pollOption.GetPropertyName(_ => _.PollID));
				Assert.AreEqual(entity.PollOptionID, pollOption.PollOptionID, pollOption.GetPropertyName(_ => _.PollOptionID));
			}
		}

		[TestMethod]
		public void Insert()
		{
			var generator = new RandomObjectGenerator();
			var optionPosition = generator.Generate<short>();
			var optionText = generator.Generate<string>();
			var pollId = generator.Generate<int>();
			var pollOptionId = generator.Generate<int>();

			var poll = new Mock<IPoll>();
			poll.Setup(_ => _.PollID).Returns(pollId);

			var pollOptions = new InMemoryDbSet<MVPollOption>();
			var entities = new Mock<IEntities>();
			entities.Setup(_ => _.MVPollOptions)
				.Returns(pollOptions);
			entities.Setup(_ => _.SaveChanges()).Callback(() => pollOptions.Local[0].PollOptionID = pollOptionId);
			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => entities.Object);

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
			{
				var pollOption = DataPortal.CreateChild<PollOption>();
				pollOption.OptionPosition = optionPosition;
				pollOption.OptionText = optionText;

				DataPortal.UpdateChild(pollOption, poll.Object);

				Assert.AreEqual(pollId, pollOption.PollID, pollOption.GetPropertyName(_ => _.PollID));
				Assert.AreEqual(pollOptionId, pollOption.PollOptionID, pollOption.GetPropertyName(_ => _.PollOptionID));
				Assert.AreEqual(optionPosition, pollOption.OptionPosition, pollOption.GetPropertyName(_ => _.OptionPosition));
				Assert.AreEqual(optionText, pollOption.OptionText, pollOption.GetPropertyName(_ => _.OptionText));
			}
		}

		[TestMethod]
		public void Update()
		{
			var generator = new RandomObjectGenerator();
			var newOptionPosition = generator.Generate<short>();
			var newOptionText = generator.Generate<string>();

			var entity = EntityCreator.Create<MVPollOption>();

			var poll = new Mock<IPoll>();

			var entities = new Mock<IEntities>();
			entities.Setup(_ => _.MVPollOptions)
				.Returns(new InMemoryDbSet<MVPollOption> { entity });

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => entities.Object);

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
			{
				var pollOption = DataPortal.FetchChild<PollOption>(entity);
				pollOption.OptionPosition = newOptionPosition;
				pollOption.OptionText = newOptionText;

				DataPortal.UpdateChild(pollOption, poll.Object);

				Assert.AreEqual(newOptionPosition, pollOption.OptionPosition, pollOption.GetPropertyName(_ => _.OptionPosition));
				Assert.AreEqual(newOptionText, pollOption.OptionText, pollOption.GetPropertyName(_ => _.OptionText));
			}
		}
	}
}
