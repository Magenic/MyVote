using Autofac;
using Csla;
using FluentAssertions;
using Moq;
using MyVote.BusinessObjects.Contracts;
using MyVote.Data.Entities;
using Spackle;
using Spackle.Extensions;
using System;
using Xunit;

namespace MyVote.BusinessObjects.Tests
{
	public sealed class PollResultsTests
	{
		[Fact]
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

			var poll = EntityCreator.Create<Mvpoll>(_ =>
			{
				_.PollId = pollId;
				_.UserId = userId;
				_.PollDeletedFlag = null;
				_.PollStartDate = DateTime.UtcNow.AddDays(-1);
				_.PollEndDate = DateTime.UtcNow.AddDays(1);
				_.PollDeletedFlag = false;
			});

			var entities = new Mock<IEntitiesContext>(MockBehavior.Strict);
			entities.Setup(_ => _.Mvpoll).Returns(new InMemoryDbSet<Mvpoll> { poll });
			entities.Setup(_ => _.Dispose());

			var builder = new ContainerBuilder();
			builder.Register<IObjectFactory<IPollDataResults>>(_ => pollDataResultsFactory.Object);
			builder.Register<IObjectFactory<IPollComments>>(_ => pollCommentsFactory.Object);
			builder.Register<IEntitiesContext>(_ => entities.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var pollResults = DataPortal.Fetch<PollResults>(new PollResultsCriteria(userId, pollId));
				pollResults.PollImageLink.Should().Be(poll.PollImageLink);
				pollResults.PollID.Should().Be(pollId);
				pollResults.PollDataResults.Should().BeSameAs(pollDataResults);
				pollResults.PollComments.Should().BeSameAs(pollComments);
				pollResults.IsPollOwnedByUser.Should().BeTrue();
				pollResults.IsActive.Should().BeTrue();
			}

			pollDataResultsFactory.VerifyAll();
			pollCommentsFactory.VerifyAll();
			entities.VerifyAll();
		}

		[Fact]
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

			var poll = EntityCreator.Create<Mvpoll>(_ =>
			{
				_.PollId = pollId;
				_.UserId = userId + 1;
				_.PollDeletedFlag = null;
				_.PollStartDate = DateTime.UtcNow.AddDays(-1);
				_.PollEndDate = DateTime.UtcNow.AddDays(1);
				_.PollDeletedFlag = false;
			});

			var entities = new Mock<IEntitiesContext>(MockBehavior.Strict);
			entities.Setup(_ => _.Mvpoll).Returns(new InMemoryDbSet<Mvpoll> { poll });
			entities.Setup(_ => _.Dispose());

			var builder = new ContainerBuilder();
			builder.Register<IObjectFactory<IPollDataResults>>(_ => pollDataResultsFactory.Object);
			builder.Register<IObjectFactory<IPollComments>>(_ => pollCommentsFactory.Object);
			builder.Register<IEntitiesContext>(_ => entities.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var pollResults = DataPortal.Fetch<PollResults>(new PollResultsCriteria(userId, pollId));
				pollResults.PollImageLink.Should().Be(poll.PollImageLink);
				pollResults.PollID.Should().Be(pollId);
				pollResults.PollDataResults.Should().BeSameAs(pollDataResults);
				pollResults.PollComments.Should().BeSameAs(pollComments);
				pollResults.IsPollOwnedByUser.Should().BeFalse();
				pollResults.IsActive.Should().BeTrue();
			}

			pollDataResultsFactory.VerifyAll();
			pollCommentsFactory.VerifyAll();
			entities.VerifyAll();
		}

		[Fact]
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

			var poll = EntityCreator.Create<Mvpoll>(_ =>
			{
				_.PollId = pollId;
				_.UserId = userId;
				_.PollDeletedFlag = null;
				_.PollStartDate = DateTime.UtcNow.AddDays(-1);
				_.PollEndDate = DateTime.UtcNow.AddDays(1);
				_.PollDeletedFlag = false;
			});

			var entities = new Mock<IEntitiesContext>(MockBehavior.Strict);
			entities.Setup(_ => _.Mvpoll).Returns(new InMemoryDbSet<Mvpoll> { poll });
			entities.Setup(_ => _.Dispose());

			var builder = new ContainerBuilder();
			builder.Register<IObjectFactory<IPollDataResults>>(_ => pollDataResultsFactory.Object);
			builder.Register<IObjectFactory<IPollComments>>(_ => pollCommentsFactory.Object);
			builder.Register<IEntitiesContext>(_ => entities.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var pollResults = DataPortal.Fetch<PollResults>(new PollResultsCriteria(null, pollId));

				pollResults.PollImageLink.Should().Be(poll.PollImageLink);
				pollResults.PollID.Should().Be(pollId);
				pollResults.PollDataResults.Should().BeSameAs(pollDataResults);
				pollResults.PollComments.Should().BeSameAs(pollComments);
				pollResults.IsPollOwnedByUser.Should().BeFalse();
				pollResults.IsActive.Should().BeTrue();
			}

			pollDataResultsFactory.VerifyAll();
			pollCommentsFactory.VerifyAll();
			entities.VerifyAll();
		}

		[Fact]
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

			var poll = EntityCreator.Create<Mvpoll>(_ =>
			{
				_.PollDeletedFlag = true;
				_.PollId = pollId;
				_.UserId = userId;
				_.PollStartDate = DateTime.UtcNow.AddDays(-1);
				_.PollEndDate = DateTime.UtcNow.AddDays(1);
			});

			var entities = new Mock<IEntitiesContext>(MockBehavior.Strict);
			entities.Setup(_ => _.Mvpoll).Returns(new InMemoryDbSet<Mvpoll> { poll });
			entities.Setup(_ => _.Dispose());

			var builder = new ContainerBuilder();
			builder.Register<IObjectFactory<IPollDataResults>>(_ => pollDataResultsFactory.Object);
			builder.Register<IObjectFactory<IPollComments>>(_ => pollCommentsFactory.Object);
			builder.Register<IEntitiesContext>(_ => entities.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var pollResults = DataPortal.Fetch<PollResults>(new PollResultsCriteria(userId, pollId));

				pollResults.PollImageLink.Should().Be(poll.PollImageLink);
				pollResults.PollID.Should().Be(pollId);
				pollResults.PollDataResults.Should().BeSameAs(pollDataResults);
				pollResults.PollComments.Should().BeSameAs(pollComments);
				pollResults.IsPollOwnedByUser.Should().BeTrue();
				pollResults.IsActive.Should().BeFalse();
			}

			pollDataResultsFactory.VerifyAll();
			pollCommentsFactory.VerifyAll();
			entities.VerifyAll();
		}

		[Fact]
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

			var poll = EntityCreator.Create<Mvpoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollId = pollId;
				_.UserId = userId;
				_.PollStartDate = DateTime.UtcNow.AddDays(1);
				_.PollEndDate = DateTime.UtcNow.AddDays(2);
			});

			var entities = new Mock<IEntitiesContext>(MockBehavior.Strict);
			entities.Setup(_ => _.Mvpoll).Returns(new InMemoryDbSet<Mvpoll> { poll });
			entities.Setup(_ => _.Dispose());

			var builder = new ContainerBuilder();
			builder.Register<IObjectFactory<IPollDataResults>>(_ => pollDataResultsFactory.Object);
			builder.Register<IObjectFactory<IPollComments>>(_ => pollCommentsFactory.Object);
			builder.Register<IEntitiesContext>(_ => entities.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var pollResults = DataPortal.Fetch<PollResults>(new PollResultsCriteria(userId, pollId));

				pollResults.PollImageLink.Should().Be(poll.PollImageLink);
				pollResults.PollID.Should().Be(pollId);
				pollResults.PollDataResults.Should().BeSameAs(pollDataResults);
				pollResults.PollComments.Should().BeSameAs(pollComments);
				pollResults.IsPollOwnedByUser.Should().BeTrue();
				pollResults.IsActive.Should().BeFalse();
			}

			pollDataResultsFactory.VerifyAll();
			pollCommentsFactory.VerifyAll();
			entities.VerifyAll();
		}

		[Fact]
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

			var poll = EntityCreator.Create<Mvpoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollId = pollId;
				_.UserId = userId;
				_.PollStartDate = DateTime.UtcNow.AddDays(-2);
				_.PollEndDate = DateTime.UtcNow.AddDays(-1);
			});

			var entities = new Mock<IEntitiesContext>(MockBehavior.Strict);
			entities.Setup(_ => _.Mvpoll).Returns(new InMemoryDbSet<Mvpoll> { poll });
			entities.Setup(_ => _.Dispose());

			var builder = new ContainerBuilder();
			builder.Register<IObjectFactory<IPollDataResults>>(_ => pollDataResultsFactory.Object);
			builder.Register<IObjectFactory<IPollComments>>(_ => pollCommentsFactory.Object);
			builder.Register<IEntitiesContext>(_ => entities.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var pollResults = DataPortal.Fetch<PollResults>(new PollResultsCriteria(userId, pollId));

				pollResults.PollImageLink.Should().Be(poll.PollImageLink);
				pollResults.PollID.Should().Be(pollId);
				pollResults.PollDataResults.Should().BeSameAs(pollDataResults);
				pollResults.PollComments.Should().BeSameAs(pollComments);
				pollResults.IsPollOwnedByUser.Should().BeTrue();
				pollResults.IsActive.Should().BeFalse();
			}

			pollDataResultsFactory.VerifyAll();
			pollCommentsFactory.VerifyAll();
			entities.VerifyAll();
		}
	}
}
