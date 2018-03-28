using Autofac;
using Csla;
using FluentAssertions;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;
using MyVote.Data.Entities;
using Rocks;
using Rocks.Options;
using Spackle;
using Spackle.Extensions;
using System.Collections;
using System.Linq;
using Xunit;

namespace MyVote.BusinessObjects.Tests
{
	public sealed class PollDataResultsTests
	{
		[Fact]
		public void Fetch()
		{
			var generator = new RandomObjectGenerator();
			var pollId = generator.Generate<int>();

			var response1 = EntityCreator.Create<MvpollResponse>(_ =>
			{
				_.PollId = pollId;
				_.PollOptionId = 1;
				_.OptionSelected = true;
			});
			var response2 = EntityCreator.Create<MvpollResponse>(_ =>
			{
				_.PollId = pollId;
				_.PollOptionId = 2;
				_.OptionSelected = true;
			});
			var response3 = EntityCreator.Create<MvpollResponse>(_ =>
			{
				_.PollId = pollId;
				_.PollOptionId = 3;
				_.OptionSelected = false;
			});
			var response4 = EntityCreator.Create<MvpollResponse>(_ =>
			{
				_.PollId = pollId;
				_.PollOptionId = 1;
				_.OptionSelected = false;
			});
			var response5 = EntityCreator.Create<MvpollResponse>(_ =>
			{
				_.PollId = pollId;
				_.PollOptionId = 2;
				_.OptionSelected = true;
			});
			var response6 = EntityCreator.Create<MvpollResponse>(_ =>
			{
				_.PollId = pollId;
				_.PollOptionId = 1;
				_.OptionSelected = false;
			});

			var option1 = EntityCreator.Create<MvpollOption>(_ =>
			{
				_.PollOptionId = 1;
				_.PollId = pollId;
			});
			var option2 = EntityCreator.Create<MvpollOption>(_ =>
			{
				_.PollOptionId = 2;
				_.PollId = pollId;
			});
			var option3 = EntityCreator.Create<MvpollOption>(_ =>
			{
				_.PollOptionId = 3;
				_.PollId = pollId;
			});

			var poll = EntityCreator.Create<Mvpoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollId = pollId;
			});

			var entities = Rock.Create<IEntitiesContext>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			entities.Handle(nameof(IEntitiesContext.MvpollResponse), 
				() => new InMemoryDbSet<MvpollResponse>
				{
					response1, response2, response3, response4, response5, response6
				});
			entities.Handle(nameof(IEntitiesContext.MvpollOption),
				() => new InMemoryDbSet<MvpollOption>
				{
					option1, option2, option3
				});
			entities.Handle(nameof(IEntitiesContext.Mvpoll), () => new InMemoryDbSet<Mvpoll> { poll });
			entities.Handle(_ => _.Dispose());

			var pollDataResultsFactory = Rock.Create<IObjectFactory<ReadOnlySwitchList<IPollDataResult>>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollDataResultsFactory.Handle(_ => _.FetchChild()).Returns(DataPortal.FetchChild<ReadOnlySwitchList<IPollDataResult>>());

			var pollDataResultFactory = Rock.Create<IObjectFactory<IPollDataResult>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollDataResultFactory.Handle<object[], IPollDataResult>(
				_ => _.FetchChild(Arg.IsAny<object[]>()), 
				data => DataPortal.FetchChild<PollDataResult>(data[0] as PollData), 3);

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => entities.Make());
			builder.Register<IObjectFactory<ReadOnlySwitchList<IPollDataResult>>>(_ => pollDataResultsFactory.Make());
			builder.Register<IObjectFactory<IPollDataResult>>(_ => pollDataResultFactory.Make());

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var results = DataPortal.FetchChild<PollDataResults>(pollId);

				results.PollID.Should().Be(pollId);
				results.Question.Should().Be(poll.PollQuestion);
				(results.Results as ICollection).Count.Should().Be(3);
				results.Results.First(_ => _.PollOptionID == 1).ResponseCount.Should().Be(1);
				results.Results.First(_ => _.PollOptionID == 2).ResponseCount.Should().Be(2);
				results.Results.First(_ => _.PollOptionID == 3).ResponseCount.Should().Be(0);
			}

			entities.Verify();
			pollDataResultsFactory.Verify();
			pollDataResultFactory.Verify();
		}
	}
}