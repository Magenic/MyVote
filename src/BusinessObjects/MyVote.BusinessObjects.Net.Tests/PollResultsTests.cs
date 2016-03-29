using Autofac;
using Csla;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyVote.BusinessObjects.Contracts;
using MyVote.Data.Entities;
using Spackle;
using Spackle.Extensions;
using System;

namespace MyVote.BusinessObjects.Net.Tests
{
	[TestClass]
	public sealed class PollResultsTests
	{
		[TestMethod]
		public void Fetch()
		{
			var generator = new RandomObjectGenerator();
			var pollId = generator.Generate<int>();
			var userId = generator.Generate<int>();

			var pollDataResults = Mock.Of<IPollDataResults>();
			var pollDataResultsFactory = new Mock<IObjectFactory<IPollDataResults>>(MockBehavior.Strict);
			pollDataResultsFactory.Setup(_ => _.FetchChild(pollId)).Returns(pollDataResults);

			var pollComments = Mock.Of<IPollComments>();
			var pollCommentsFactory = new Mock<IObjectFactory<IPollComments>>(MockBehavior.Strict);
			pollCommentsFactory.Setup(_ => _.FetchChild(pollId)).Returns(pollComments);

			var poll = EntityCreator.Create<MVPoll>(_ =>
			{
				_.PollID = pollId;
				_.UserID = userId;
				_.PollDeletedFlag = null;
				_.PollStartDate = DateTime.UtcNow.AddDays(-1);
				_.PollEndDate = DateTime.UtcNow.AddDays(1);
			});

			var entities = new Mock<IEntities>(MockBehavior.Strict);
			entities.Setup(_ => _.MVPolls).Returns(new InMemoryDbSet<MVPoll>
				{
					poll
				});
			entities.Setup(_ => _.Dispose());

			var builder = new ContainerBuilder();
			builder.Register<IObjectFactory<IPollDataResults>>(_ => pollDataResultsFactory.Object);
			builder.Register<IObjectFactory<IPollComments>>(_ => pollCommentsFactory.Object);
			builder.Register<IEntities>(_ => entities.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var pollResults = DataPortal.Fetch<PollResults>(new PollResultsCriteria(userId, pollId));
				Assert.AreEqual(poll.PollImageLink, pollResults.PollImageLink, nameof(pollResults.PollImageLink));
				Assert.AreEqual(pollId, pollResults.PollID, nameof(pollResults.PollID));
				Assert.AreSame(pollDataResults, pollResults.PollDataResults, nameof(pollResults.PollDataResults));
				Assert.AreSame(pollComments, pollResults.PollComments, nameof(pollResults.PollComments));
				Assert.IsTrue(pollResults.IsPollOwnedByUser, nameof(pollResults.IsPollOwnedByUser));
				Assert.IsTrue(pollResults.IsActive, nameof(pollResults.IsActive));
			}

			pollDataResultsFactory.VerifyAll();
			pollCommentsFactory.VerifyAll();
			entities.VerifyAll();
		}

		[TestMethod]
		public void FetchNotOwnedByUser()
		{
			var generator = new RandomObjectGenerator();
			var pollId = generator.Generate<int>();
			var userId = generator.Generate<int>();

			var pollDataResults = Mock.Of<IPollDataResults>();
			var pollDataResultsFactory = new Mock<IObjectFactory<IPollDataResults>>(MockBehavior.Strict);
			pollDataResultsFactory.Setup(_ => _.FetchChild(pollId)).Returns(pollDataResults);

			var pollComments = Mock.Of<IPollComments>();
			var pollCommentsFactory = new Mock<IObjectFactory<IPollComments>>(MockBehavior.Strict);
			pollCommentsFactory.Setup(_ => _.FetchChild(pollId)).Returns(pollComments);

			var poll = EntityCreator.Create<MVPoll>(_ =>
			{
				_.PollID = pollId;
				_.UserID = userId + 1;
				_.PollDeletedFlag = null;
				_.PollStartDate = DateTime.UtcNow.AddDays(-1);
				_.PollEndDate = DateTime.UtcNow.AddDays(1);
			});

			var entities = new Mock<IEntities>(MockBehavior.Strict);
			entities.Setup(_ => _.MVPolls).Returns(new InMemoryDbSet<MVPoll>
				{
					poll
				});
			entities.Setup(_ => _.Dispose());

			var builder = new ContainerBuilder();
			builder.Register<IObjectFactory<IPollDataResults>>(_ => pollDataResultsFactory.Object);
			builder.Register<IObjectFactory<IPollComments>>(_ => pollCommentsFactory.Object);
			builder.Register<IEntities>(_ => entities.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var pollResults = DataPortal.Fetch<PollResults>(new PollResultsCriteria(userId, pollId));
				Assert.AreEqual(pollId, pollResults.PollID, nameof(pollResults.PollID));
				Assert.AreEqual(poll.PollImageLink, pollResults.PollImageLink, nameof(pollResults.PollImageLink));
				Assert.AreSame(pollDataResults, pollResults.PollDataResults, nameof(pollResults.PollDataResults));
				Assert.AreSame(pollComments, pollResults.PollComments, nameof(pollResults.PollComments));
				Assert.IsFalse(pollResults.IsPollOwnedByUser, nameof(pollResults.IsPollOwnedByUser));
				Assert.IsTrue(pollResults.IsActive, nameof(pollResults.IsActive));
			}

			pollDataResultsFactory.VerifyAll();
			pollCommentsFactory.VerifyAll();
			entities.VerifyAll();
		}

		[TestMethod]
		public void FetchNotSignedIn()
		{
			var generator = new RandomObjectGenerator();
			var pollId = generator.Generate<int>();
			var userId = generator.Generate<int>();

			var pollDataResults = Mock.Of<IPollDataResults>();
			var pollDataResultsFactory = new Mock<IObjectFactory<IPollDataResults>>(MockBehavior.Strict);
			pollDataResultsFactory.Setup(_ => _.FetchChild(pollId)).Returns(pollDataResults);

			var pollComments = Mock.Of<IPollComments>();
			var pollCommentsFactory = new Mock<IObjectFactory<IPollComments>>(MockBehavior.Strict);
			pollCommentsFactory.Setup(_ => _.FetchChild(pollId)).Returns(pollComments);

			var poll = EntityCreator.Create<MVPoll>(_ =>
			{
				_.PollID = pollId;
				_.UserID = userId;
				_.PollDeletedFlag = null;
				_.PollStartDate = DateTime.UtcNow.AddDays(-1);
				_.PollEndDate = DateTime.UtcNow.AddDays(1);
			});

			var entities = new Mock<IEntities>(MockBehavior.Strict);
			entities.Setup(_ => _.MVPolls).Returns(new InMemoryDbSet<MVPoll>
				{
					poll
				});
			entities.Setup(_ => _.Dispose());

			var builder = new ContainerBuilder();
			builder.Register<IObjectFactory<IPollDataResults>>(_ => pollDataResultsFactory.Object);
			builder.Register<IObjectFactory<IPollComments>>(_ => pollCommentsFactory.Object);
			builder.Register<IEntities>(_ => entities.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var pollResults = DataPortal.Fetch<PollResults>(new PollResultsCriteria(null, pollId));
				Assert.AreEqual(pollId, pollResults.PollID, nameof(pollResults.PollID));
				Assert.AreEqual(poll.PollImageLink, pollResults.PollImageLink, nameof(pollResults.PollImageLink));
				Assert.AreSame(pollDataResults, pollResults.PollDataResults, nameof(pollResults.PollDataResults));
				Assert.AreSame(pollComments, pollResults.PollComments, nameof(pollResults.PollComments));
				Assert.IsFalse(pollResults.IsPollOwnedByUser, nameof(pollResults.IsPollOwnedByUser));
				Assert.IsTrue(pollResults.IsActive, nameof(pollResults.IsActive));
			}

			pollDataResultsFactory.VerifyAll();
			pollCommentsFactory.VerifyAll();
			entities.VerifyAll();
		}

		[TestMethod]
		public void FetchIsDeleted()
		{
			var generator = new RandomObjectGenerator();
			var pollId = generator.Generate<int>();
			var userId = generator.Generate<int>();

			var pollDataResults = Mock.Of<IPollDataResults>();
			var pollDataResultsFactory = new Mock<IObjectFactory<IPollDataResults>>(MockBehavior.Strict);
			pollDataResultsFactory.Setup(_ => _.FetchChild(pollId)).Returns(pollDataResults);

			var pollComments = Mock.Of<IPollComments>();
			var pollCommentsFactory = new Mock<IObjectFactory<IPollComments>>(MockBehavior.Strict);
			pollCommentsFactory.Setup(_ => _.FetchChild(pollId)).Returns(pollComments);

			var poll = EntityCreator.Create<MVPoll>(_ =>
			{
				_.PollDeletedFlag = true;
				_.PollID = pollId;
				_.UserID = userId;
				_.PollStartDate = DateTime.UtcNow.AddDays(-1);
				_.PollEndDate = DateTime.UtcNow.AddDays(1);
			});

			var entities = new Mock<IEntities>(MockBehavior.Strict);
			entities.Setup(_ => _.MVPolls).Returns(new InMemoryDbSet<MVPoll>
				{
					poll
				});
			entities.Setup(_ => _.Dispose());

			var builder = new ContainerBuilder();
			builder.Register<IObjectFactory<IPollDataResults>>(_ => pollDataResultsFactory.Object);
			builder.Register<IObjectFactory<IPollComments>>(_ => pollCommentsFactory.Object);
			builder.Register<IEntities>(_ => entities.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var pollResults = DataPortal.Fetch<PollResults>(new PollResultsCriteria(userId, pollId));
				Assert.AreEqual(pollId, pollResults.PollID, nameof(pollResults.PollID));
				Assert.AreEqual(poll.PollImageLink, pollResults.PollImageLink, nameof(pollResults.PollImageLink));
				Assert.AreSame(pollDataResults, pollResults.PollDataResults, nameof(pollResults.PollDataResults));
				Assert.AreSame(pollComments, pollResults.PollComments, nameof(pollResults.PollComments));
				Assert.IsTrue(pollResults.IsPollOwnedByUser, nameof(pollResults.IsPollOwnedByUser));
				Assert.IsFalse(pollResults.IsActive, nameof(pollResults.IsActive));
			}

			pollDataResultsFactory.VerifyAll();
			pollCommentsFactory.VerifyAll();
			entities.VerifyAll();
		}

		[TestMethod]
		public void FetchHasNotStarted()
		{
			var generator = new RandomObjectGenerator();
			var pollId = generator.Generate<int>();
			var userId = generator.Generate<int>();

			var pollDataResults = Mock.Of<IPollDataResults>();
			var pollDataResultsFactory = new Mock<IObjectFactory<IPollDataResults>>(MockBehavior.Strict);
			pollDataResultsFactory.Setup(_ => _.FetchChild(pollId)).Returns(pollDataResults);

			var pollComments = Mock.Of<IPollComments>();
			var pollCommentsFactory = new Mock<IObjectFactory<IPollComments>>(MockBehavior.Strict);
			pollCommentsFactory.Setup(_ => _.FetchChild(pollId)).Returns(pollComments);

			var poll = EntityCreator.Create<MVPoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollID = pollId;
				_.UserID = userId;
				_.PollStartDate = DateTime.UtcNow.AddDays(1);
				_.PollEndDate = DateTime.UtcNow.AddDays(2);
			});

			var entities = new Mock<IEntities>(MockBehavior.Strict);
			entities.Setup(_ => _.MVPolls).Returns(new InMemoryDbSet<MVPoll>
				{
					poll
				});
			entities.Setup(_ => _.Dispose());

			var builder = new ContainerBuilder();
			builder.Register<IObjectFactory<IPollDataResults>>(_ => pollDataResultsFactory.Object);
			builder.Register<IObjectFactory<IPollComments>>(_ => pollCommentsFactory.Object);
			builder.Register<IEntities>(_ => entities.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var pollResults = DataPortal.Fetch<PollResults>(new PollResultsCriteria(userId, pollId));
				Assert.AreEqual(pollId, pollResults.PollID, nameof(pollResults.PollID));
				Assert.AreEqual(poll.PollImageLink, pollResults.PollImageLink, nameof(pollResults.PollImageLink));
				Assert.AreSame(pollDataResults, pollResults.PollDataResults, nameof(pollResults.PollDataResults));
				Assert.AreSame(pollComments, pollResults.PollComments, nameof(pollResults.PollComments));
				Assert.IsTrue(pollResults.IsPollOwnedByUser, nameof(pollResults.IsPollOwnedByUser));
				Assert.IsFalse(pollResults.IsActive, nameof(pollResults.IsActive));
			}

			pollDataResultsFactory.VerifyAll();
			pollCommentsFactory.VerifyAll();
			entities.VerifyAll();
		}

		[TestMethod]
		public void FetchHasEnded()
		{
			var generator = new RandomObjectGenerator();
			var pollId = generator.Generate<int>();
			var userId = generator.Generate<int>();

			var pollDataResults = Mock.Of<IPollDataResults>();
			var pollDataResultsFactory = new Mock<IObjectFactory<IPollDataResults>>(MockBehavior.Strict);
			pollDataResultsFactory.Setup(_ => _.FetchChild(pollId)).Returns(pollDataResults);

			var pollComments = Mock.Of<IPollComments>();
			var pollCommentsFactory = new Mock<IObjectFactory<IPollComments>>(MockBehavior.Strict);
			pollCommentsFactory.Setup(_ => _.FetchChild(pollId)).Returns(pollComments);

			var poll = EntityCreator.Create<MVPoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollID = pollId;
				_.UserID = userId;
				_.PollStartDate = DateTime.UtcNow.AddDays(-2);
				_.PollEndDate = DateTime.UtcNow.AddDays(-1);
			});

			var entities = new Mock<IEntities>(MockBehavior.Strict);
			entities.Setup(_ => _.MVPolls).Returns(new InMemoryDbSet<MVPoll>
				{
					poll
				});
			entities.Setup(_ => _.Dispose());

			var builder = new ContainerBuilder();
			builder.Register<IObjectFactory<IPollDataResults>>(_ => pollDataResultsFactory.Object);
			builder.Register<IObjectFactory<IPollComments>>(_ => pollCommentsFactory.Object);
			builder.Register<IEntities>(_ => entities.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var pollResults = DataPortal.Fetch<PollResults>(new PollResultsCriteria(userId, pollId));
				Assert.AreEqual(pollId, pollResults.PollID, nameof(pollResults.PollID));
				Assert.AreEqual(poll.PollImageLink, pollResults.PollImageLink, nameof(pollResults.PollImageLink));
				Assert.AreSame(pollDataResults, pollResults.PollDataResults, nameof(pollResults.PollDataResults));
				Assert.AreSame(pollComments, pollResults.PollComments, nameof(pollResults.PollComments));
				Assert.IsTrue(pollResults.IsPollOwnedByUser, nameof(pollResults.IsPollOwnedByUser));
				Assert.IsFalse(pollResults.IsActive, nameof(pollResults.IsActive));
			}

			pollDataResultsFactory.VerifyAll();
			pollCommentsFactory.VerifyAll();
			entities.VerifyAll();
		}
	}
}
