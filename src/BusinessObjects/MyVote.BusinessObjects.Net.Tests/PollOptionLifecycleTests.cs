using Autofac;
using Csla;
using FluentAssertions;
using Moq;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Net.Tests.Extensions;
using MyVote.Data.Entities;
using Spackle;
using Spackle.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using Xunit;

namespace MyVote.BusinessObjects.Net.Tests
{
	public sealed class PollOptionLifecycleTests
	{
		[Fact]
		public void Create()
		{
			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var pollOption = DataPortal.CreateChild<PollOption>();

				pollOption.OptionPosition.Should().NotHaveValue();
				pollOption.OptionText.Should().BeEmpty();
				pollOption.PollID.Should().NotHaveValue();
				pollOption.PollOptionID.Should().NotHaveValue();

				pollOption.BrokenRulesCollection.AssertRuleCount(2);
				pollOption.BrokenRulesCollection.AssertRuleCount(PollOption.OptionPositionProperty, 1);
				pollOption.BrokenRulesCollection.AssertValidationRuleExists<RequiredAttribute>(
					PollOption.OptionPositionProperty, true);
				pollOption.BrokenRulesCollection.AssertRuleCount(PollOption.OptionTextProperty, 1);
				pollOption.BrokenRulesCollection.AssertValidationRuleExists<RequiredAttribute>(
					PollOption.OptionTextProperty, true);
			}
		}

		[Fact]
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

				pollOption.OptionPosition.Should().Be(entity.OptionPosition);
				pollOption.OptionText.Should().Be(entity.OptionText);
				pollOption.PollID.Should().Be(entity.PollID);
				pollOption.PollOptionID.Should().Be(entity.PollOptionID);
			}

			entities.VerifyAll();
		}

		[Fact]
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

				pollOption.PollID.Should().Be(pollId);
				pollOption.PollOptionID.Should().Be(pollOptionId);
				pollOption.OptionPosition.Should().Be(optionPosition);
				pollOption.OptionText.Should().Be(optionText);
			}

			entities.VerifyAll();
			poll.VerifyAll();
		}

		[Fact]
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

				pollOption.OptionPosition.Should().Be(newOptionPosition);
				pollOption.OptionText.Should().Be(newOptionText);
			}

			entities.VerifyAll();
		}
	}
}
