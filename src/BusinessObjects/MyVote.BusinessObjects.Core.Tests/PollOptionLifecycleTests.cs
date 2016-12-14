using Autofac;
using Csla;
using FluentAssertions;
using Moq;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Tests.Extensions;
using MyVote.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Spackle;
using Spackle.Extensions;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace MyVote.BusinessObjects.Tests
{
	public sealed class PollOptionLifecycleTests
	{
		[Fact]
		public void Create()
		{
			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => Mock.Of<IEntitiesContext>());

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
			var entity = EntityCreator.Create<MvpollOption>();

			var entities = new Mock<IEntitiesContext>(MockBehavior.Strict);
			entities.Setup(_ => _.Dispose());

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => entities.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var pollOption = DataPortal.FetchChild<PollOption>(entity);

				pollOption.OptionPosition.Should().Be(entity.OptionPosition);
				pollOption.OptionText.Should().Be(entity.OptionText);
				pollOption.PollID.Should().Be(entity.PollId);
				pollOption.PollOptionID.Should().Be(entity.PollOptionId);
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

			var pollOptions = new InMemoryDbSet<MvpollOption>();
			var entities = new Mock<IEntitiesContext>(MockBehavior.Strict);
			entities.Setup(_ => _.MvpollOption)
				.Returns(pollOptions);
			entities.Setup(_ => _.SaveChanges())
				.Callback(() => pollOptions.Local[0].PollOptionId = pollOptionId)
				.Returns(1);
			entities.Setup(_ => _.Dispose());

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => entities.Object);

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

			var entity = EntityCreator.Create<MvpollOption>();

			var poll = Mock.Of<IPoll>();

			var entities = new Mock<IEntitiesContext>(MockBehavior.Strict);
			entities.Setup(_ => _.MvpollOption)
				.Returns(new InMemoryDbSet<MvpollOption> { entity });
			entities.Setup(_ => _.SetState(It.IsAny<MvpollOption>(), EntityState.Modified));
			entities.Setup(_ => _.SaveChanges()).Returns(1);
			entities.Setup(_ => _.Dispose());

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => entities.Object);

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
