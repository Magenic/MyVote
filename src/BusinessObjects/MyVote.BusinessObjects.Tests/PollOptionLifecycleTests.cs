using Autofac;
using Csla;
using FluentAssertions;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Tests.Extensions;
using MyVote.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Spackle;
using Spackle.Extensions;
using System.ComponentModel.DataAnnotations;
using Xunit;
using Rocks;
using Rocks.Options;

namespace MyVote.BusinessObjects.Tests
{
	public sealed class PollOptionLifecycleTests
	{
		[Fact]
		public void Create()
		{
			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => Rock.Make<IEntitiesContext>(
				new RockOptions(allowWarnings: AllowWarnings.Yes)));

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

			var entities = Rock.Create<IEntitiesContext>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			entities.Handle(_ => _.Dispose());

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => entities.Make());

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var pollOption = DataPortal.FetchChild<PollOption>(entity);

				pollOption.OptionPosition.Should().Be(entity.OptionPosition);
				pollOption.OptionText.Should().Be(entity.OptionText);
				pollOption.PollID.Should().Be(entity.PollId);
				pollOption.PollOptionID.Should().Be(entity.PollOptionId);
			}

			entities.Verify();
		}

		[Fact]
		public void Insert()
		{
			var generator = new RandomObjectGenerator();
			var optionPosition = generator.Generate<short>();
			var optionText = generator.Generate<string>();
			var pollId = generator.Generate<int>();
			var pollOptionId = generator.Generate<int>();

			var poll = Rock.Create<IPoll>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			poll.Handle(nameof(IPoll.PollID), () => pollId as int?);
			var pollChunk = poll.Make();

			var pollOptions = new InMemoryDbSet<MvpollOption>();
			var entities = Rock.Create<IEntitiesContext>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			entities.Handle(nameof(IEntitiesContext.MvpollOption), () => pollOptions);
			entities.Handle(_ => _.SaveChanges(),
				() =>
				{
					pollOptions.LocalData[0].PollOptionId = pollOptionId;
					return 1;
				});
			entities.Handle(_ => _.Dispose());

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => entities.Make());

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var pollOption = DataPortal.CreateChild<PollOption>();
				pollOption.OptionPosition = optionPosition;
				pollOption.OptionText = optionText;

				DataPortal.UpdateChild(pollOption, pollChunk);

				pollOption.PollID.Should().Be(pollId);
				pollOption.PollOptionID.Should().Be(pollOptionId);
				pollOption.OptionPosition.Should().Be(optionPosition);
				pollOption.OptionText.Should().Be(optionText);
			}

			entities.Verify();
			poll.Verify();
		}

		[Fact]
		public void Update()
		{
			var generator = new RandomObjectGenerator();
			var newOptionPosition = generator.Generate<short>();
			var newOptionText = generator.Generate<string>();

			var entity = EntityCreator.Create<MvpollOption>();

			var poll = Rock.Make<IPoll>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));

			var entities = Rock.Create<IEntitiesContext>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			entities.Handle(nameof(IEntitiesContext.MvpollOption), () => new InMemoryDbSet<MvpollOption> { entity });
			entities.Handle(_ => _.SetState(Arg.IsAny<MvpollOption>(), EntityState.Modified));
			entities.Handle(_ => _.SaveChanges()).Returns(1);
			entities.Handle(_ => _.Dispose());

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => entities.Make());

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

			entities.Verify();
		}
	}
}