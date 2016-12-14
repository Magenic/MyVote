using Autofac;
using Csla;
using FluentAssertions;
using Moq;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;
using MyVote.Data.Entities;
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

			var entities = new Mock<IEntitiesContext>(MockBehavior.Strict);
			entities.Setup(_ => _.MvpollResponse).Returns(new InMemoryDbSet<MvpollResponse>
				{
					response1, response2, response3, response4, response5, response6
				});
			entities.Setup(_ => _.MvpollOption).Returns(new InMemoryDbSet<MvpollOption>
				{
					option1, option2, option3
				});
			entities.Setup(_ => _.Mvpoll).Returns(new InMemoryDbSet<Mvpoll> { poll });
			entities.Setup(_ => _.Dispose());

			var pollDataResultsFactory = new Mock<IObjectFactory<ReadOnlySwitchList<IPollDataResult>>>(MockBehavior.Strict);
			pollDataResultsFactory.Setup(_ => _.FetchChild()).Returns(DataPortal.FetchChild<ReadOnlySwitchList<IPollDataResult>>());

			var pollDataResultFactory = new Mock<IObjectFactory<IPollDataResult>>(MockBehavior.Strict);
			pollDataResultFactory.Setup(_ => _.FetchChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.FetchChild<PollDataResult>(_[0] as PollData));

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => entities.Object);
			builder.Register<IObjectFactory<ReadOnlySwitchList<IPollDataResult>>>(_ => pollDataResultsFactory.Object);
			builder.Register<IObjectFactory<IPollDataResult>>(_ => pollDataResultFactory.Object);

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

			entities.VerifyAll();
			pollDataResultsFactory.VerifyAll();
			pollDataResultFactory.VerifyAll();
		}
	}
}
