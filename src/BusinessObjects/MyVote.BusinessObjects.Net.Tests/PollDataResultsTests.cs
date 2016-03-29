using Autofac;
using Csla;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;
using MyVote.Data.Entities;
using Spackle;
using Spackle.Extensions;
using System.Linq;

namespace MyVote.BusinessObjects.Net.Tests
{
	[TestClass]
	public sealed class PollDataResultsTests
	{
		[TestMethod]
		public void Fetch()
		{
			var generator = new RandomObjectGenerator();
			var pollID = generator.Generate<int>();

			var response1 = EntityCreator.Create<MVPollResponse>(_ =>
			{
				_.PollID = pollID;
				_.PollOptionID = 1;
				_.OptionSelected = true;
			});
			var response2 = EntityCreator.Create<MVPollResponse>(_ =>
			{
				_.PollID = pollID;
				_.PollOptionID = 2;
				_.OptionSelected = true;
			});
			var response3 = EntityCreator.Create<MVPollResponse>(_ =>
			{
				_.PollID = pollID;
				_.PollOptionID = 3;
				_.OptionSelected = false;
			});
			var response4 = EntityCreator.Create<MVPollResponse>(_ =>
			{
				_.PollID = pollID;
				_.PollOptionID = 1;
				_.OptionSelected = false;
			});
			var response5 = EntityCreator.Create<MVPollResponse>(_ =>
			{
				_.PollID = pollID;
				_.PollOptionID = 2;
				_.OptionSelected = true;
			});
			var response6 = EntityCreator.Create<MVPollResponse>(_ =>
			{
				_.PollID = pollID;
				_.PollOptionID = 1;
				_.OptionSelected = false;
			});

			var option1 = EntityCreator.Create<MVPollOption>(_ =>
			{
				_.PollOptionID = 1;
				_.PollID = pollID;
			});
			var option2 = EntityCreator.Create<MVPollOption>(_ =>
			{
				_.PollOptionID = 2;
				_.PollID = pollID;
			});
			var option3 = EntityCreator.Create<MVPollOption>(_ =>
			{
				_.PollOptionID = 3;
				_.PollID = pollID;
			});

			var poll = EntityCreator.Create<MVPoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollID = pollID;
			});

			var entities = new Mock<IEntities>(MockBehavior.Strict);
			entities.Setup(_ => _.MVPollResponses).Returns(new InMemoryDbSet<MVPollResponse>
				{
					response1, response2, response3, response4, response5, response6
				});
			entities.Setup(_ => _.MVPollOptions).Returns(new InMemoryDbSet<MVPollOption>
				{
					option1, option2, option3
				});
			entities.Setup(_ => _.MVPolls).Returns(new InMemoryDbSet<MVPoll> { poll });
			entities.Setup(_ => _.Dispose());

			var pollDataResultsFactory = new Mock<IObjectFactory<ReadOnlySwitchList<IPollDataResult>>>(MockBehavior.Strict);
			pollDataResultsFactory.Setup(_ => _.FetchChild()).Returns(DataPortal.FetchChild<ReadOnlySwitchList<IPollDataResult>>());

			var pollDataResultFactory = new Mock<IObjectFactory<IPollDataResult>>(MockBehavior.Strict);
			pollDataResultFactory.Setup(_ => _.FetchChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.FetchChild<PollDataResult>(_[0] as PollData));

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => entities.Object);
			builder.Register<IObjectFactory<ReadOnlySwitchList<IPollDataResult>>>(_ => pollDataResultsFactory.Object);
			builder.Register<IObjectFactory<IPollDataResult>>(_ => pollDataResultFactory.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var results = DataPortal.FetchChild<PollDataResults>(pollID);

				Assert.AreEqual(pollID, results.PollID, nameof(results.PollID));
				Assert.AreEqual(poll.PollQuestion, results.Question, nameof(results.Question));
				Assert.AreEqual(3, results.Results.Count, nameof(results.Results));
				Assert.AreEqual(1, results.Results.First(_ => _.PollOptionID == 1).ResponseCount);
				Assert.AreEqual(2, results.Results.First(_ => _.PollOptionID == 2).ResponseCount);
				Assert.AreEqual(0, results.Results.First(_ => _.PollOptionID == 3).ResponseCount);
			}

			entities.VerifyAll();
			pollDataResultsFactory.VerifyAll();
			pollDataResultFactory.VerifyAll();
		}
	}
}
