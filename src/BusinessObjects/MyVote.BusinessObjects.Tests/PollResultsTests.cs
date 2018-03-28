using Autofac;
using Csla;
using FluentAssertions;
using MyVote.BusinessObjects.Contracts;
using MyVote.Data.Entities;
using Rocks;
using Rocks.Options;
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

			var pollDataResults = Rock.Make<IPollDataResults>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			var pollDataResultsFactory = Rock.Create<IObjectFactory<IPollDataResults>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollDataResultsFactory.Handle(_ => _.FetchChild(pollId)).Returns(pollDataResults);

			var pollComments = Rock.Make<IPollComments>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			var pollCommentsFactory = Rock.Create<IObjectFactory<IPollComments>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollCommentsFactory.Handle(_ => _.FetchChild(pollId)).Returns(pollComments);

			var poll = EntityCreator.Create<Mvpoll>(_ =>
			{
				_.PollId = pollId;
				_.UserId = userId;
				_.PollDeletedFlag = null;
				_.PollStartDate = DateTime.UtcNow.AddDays(-1);
				_.PollEndDate = DateTime.UtcNow.AddDays(1);
				_.PollDeletedFlag = false;
			});

			var entities = Rock.Create<IEntitiesContext>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			entities.Handle(nameof(IEntitiesContext.Mvpoll), () => new InMemoryDbSet<Mvpoll> { poll });
			entities.Handle(_ => _.Dispose());

			var builder = new ContainerBuilder();
			builder.Register<IObjectFactory<IPollDataResults>>(_ => pollDataResultsFactory.Make());
			builder.Register<IObjectFactory<IPollComments>>(_ => pollCommentsFactory.Make());
			builder.Register<IEntitiesContext>(_ => entities.Make());

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

			pollDataResultsFactory.Verify();
			pollCommentsFactory.Verify();
			entities.Verify();
		}

		[Fact]
		public void FetchNotOwnedByUser()
		{
			var generator = new RandomObjectGenerator();
			var pollId = generator.Generate<int>();
			var userId = generator.Generate<int>();

			var pollDataResults = Rock.Make<IPollDataResults>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			var pollDataResultsFactory = Rock.Create<IObjectFactory<IPollDataResults>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollDataResultsFactory.Handle(_ => _.FetchChild(pollId)).Returns(pollDataResults);

			var pollComments = Rock.Make<IPollComments>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			var pollCommentsFactory = Rock.Create<IObjectFactory<IPollComments>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollCommentsFactory.Handle(_ => _.FetchChild(pollId)).Returns(pollComments);

			var poll = EntityCreator.Create<Mvpoll>(_ =>
			{
				_.PollId = pollId;
				_.UserId = userId + 1;
				_.PollDeletedFlag = null;
				_.PollStartDate = DateTime.UtcNow.AddDays(-1);
				_.PollEndDate = DateTime.UtcNow.AddDays(1);
				_.PollDeletedFlag = false;
			});

			var entities = Rock.Create<IEntitiesContext>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			entities.Handle(nameof(IEntitiesContext.Mvpoll), () => new InMemoryDbSet<Mvpoll> { poll });
			entities.Handle(_ => _.Dispose());

			var builder = new ContainerBuilder();
			builder.Register<IObjectFactory<IPollDataResults>>(_ => pollDataResultsFactory.Make());
			builder.Register<IObjectFactory<IPollComments>>(_ => pollCommentsFactory.Make());
			builder.Register<IEntitiesContext>(_ => entities.Make());

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

			pollDataResultsFactory.Verify();
			pollCommentsFactory.Verify();
			entities.Verify();
		}

		[Fact]
		public void FetchNotSignedIn()
		{
			var generator = new RandomObjectGenerator();
			var pollId = generator.Generate<int>();
			var userId = generator.Generate<int>();

			var pollDataResults = Rock.Make<IPollDataResults>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			var pollDataResultsFactory = Rock.Create<IObjectFactory<IPollDataResults>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollDataResultsFactory.Handle(_ => _.FetchChild(pollId)).Returns(pollDataResults);

			var pollComments = Rock.Make<IPollComments>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			var pollCommentsFactory = Rock.Create<IObjectFactory<IPollComments>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollCommentsFactory.Handle(_ => _.FetchChild(pollId)).Returns(pollComments);

			var poll = EntityCreator.Create<Mvpoll>(_ =>
			{
				_.PollId = pollId;
				_.UserId = userId;
				_.PollDeletedFlag = null;
				_.PollStartDate = DateTime.UtcNow.AddDays(-1);
				_.PollEndDate = DateTime.UtcNow.AddDays(1);
				_.PollDeletedFlag = false;
			});

			var entities = Rock.Create<IEntitiesContext>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			entities.Handle(nameof(IEntitiesContext.Mvpoll), () => new InMemoryDbSet<Mvpoll> { poll });
			entities.Handle(_ => _.Dispose());

			var builder = new ContainerBuilder();
			builder.Register<IObjectFactory<IPollDataResults>>(_ => pollDataResultsFactory.Make());
			builder.Register<IObjectFactory<IPollComments>>(_ => pollCommentsFactory.Make());
			builder.Register<IEntitiesContext>(_ => entities.Make());

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

			pollDataResultsFactory.Verify();
			pollCommentsFactory.Verify();
			entities.Verify();
		}

		[Fact]
		public void FetchIsDeleted()
		{
			var generator = new RandomObjectGenerator();
			var pollId = generator.Generate<int>();
			var userId = generator.Generate<int>();

			var pollDataResults = Rock.Make<IPollDataResults>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			var pollDataResultsFactory = Rock.Create<IObjectFactory<IPollDataResults>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollDataResultsFactory.Handle(_ => _.FetchChild(pollId)).Returns(pollDataResults);

			var pollComments = Rock.Make<IPollComments>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			var pollCommentsFactory = Rock.Create<IObjectFactory<IPollComments>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollCommentsFactory.Handle(_ => _.FetchChild(pollId)).Returns(pollComments);

			var poll = EntityCreator.Create<Mvpoll>(_ =>
			{
				_.PollDeletedFlag = true;
				_.PollId = pollId;
				_.UserId = userId;
				_.PollStartDate = DateTime.UtcNow.AddDays(-1);
				_.PollEndDate = DateTime.UtcNow.AddDays(1);
			});

			var entities = Rock.Create<IEntitiesContext>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			entities.Handle(nameof(IEntitiesContext.Mvpoll), () => new InMemoryDbSet<Mvpoll> { poll });
			entities.Handle(_ => _.Dispose());

			var builder = new ContainerBuilder();
			builder.Register<IObjectFactory<IPollDataResults>>(_ => pollDataResultsFactory.Make());
			builder.Register<IObjectFactory<IPollComments>>(_ => pollCommentsFactory.Make());
			builder.Register<IEntitiesContext>(_ => entities.Make());

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

			pollDataResultsFactory.Verify();
			pollCommentsFactory.Verify();
			entities.Verify();
		}

		[Fact]
		public void FetchHasNotStarted()
		{
			var generator = new RandomObjectGenerator();
			var pollId = generator.Generate<int>();
			var userId = generator.Generate<int>();

			var pollDataResults = Rock.Make<IPollDataResults>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			var pollDataResultsFactory = Rock.Create<IObjectFactory<IPollDataResults>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollDataResultsFactory.Handle(_ => _.FetchChild(pollId)).Returns(pollDataResults);

			var pollComments = Rock.Make<IPollComments>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			var pollCommentsFactory = Rock.Create<IObjectFactory<IPollComments>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollCommentsFactory.Handle(_ => _.FetchChild(pollId)).Returns(pollComments);

			var poll = EntityCreator.Create<Mvpoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollId = pollId;
				_.UserId = userId;
				_.PollStartDate = DateTime.UtcNow.AddDays(1);
				_.PollEndDate = DateTime.UtcNow.AddDays(2);
			});

			var entities = Rock.Create<IEntitiesContext>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			entities.Handle(nameof(IEntitiesContext.Mvpoll), () => new InMemoryDbSet<Mvpoll> { poll });
			entities.Handle(_ => _.Dispose());

			var builder = new ContainerBuilder();
			builder.Register<IObjectFactory<IPollDataResults>>(_ => pollDataResultsFactory.Make());
			builder.Register<IObjectFactory<IPollComments>>(_ => pollCommentsFactory.Make());
			builder.Register<IEntitiesContext>(_ => entities.Make());

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

			pollDataResultsFactory.Verify();
			pollCommentsFactory.Verify();
			entities.Verify();
		}

		[Fact]
		public void FetchHasEnded()
		{
			var generator = new RandomObjectGenerator();
			var pollId = generator.Generate<int>();
			var userId = generator.Generate<int>();

			var pollDataResults = Rock.Make<IPollDataResults>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			var pollDataResultsFactory = Rock.Create<IObjectFactory<IPollDataResults>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollDataResultsFactory.Handle(_ => _.FetchChild(pollId)).Returns(pollDataResults);

			var pollComments = Rock.Make<IPollComments>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			var pollCommentsFactory = Rock.Create<IObjectFactory<IPollComments>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollCommentsFactory.Handle(_ => _.FetchChild(pollId)).Returns(pollComments);

			var poll = EntityCreator.Create<Mvpoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollId = pollId;
				_.UserId = userId;
				_.PollStartDate = DateTime.UtcNow.AddDays(-2);
				_.PollEndDate = DateTime.UtcNow.AddDays(-1);
			});

			var entities = Rock.Create<IEntitiesContext>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			entities.Handle(nameof(IEntitiesContext.Mvpoll), () => new InMemoryDbSet<Mvpoll> { poll });
			entities.Handle(_ => _.Dispose());

			var builder = new ContainerBuilder();
			builder.Register<IObjectFactory<IPollDataResults>>(_ => pollDataResultsFactory.Make());
			builder.Register<IObjectFactory<IPollComments>>(_ => pollCommentsFactory.Make());
			builder.Register<IEntitiesContext>(_ => entities.Make());

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

			pollDataResultsFactory.Verify();
			pollCommentsFactory.Verify();
			entities.Verify();
		}
	}
}
