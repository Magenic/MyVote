using Autofac;
using Csla;
using FluentAssertions;
using MyVote.BusinessObjects.Contracts;
using MyVote.Data.Entities;
using Rocks;
using Rocks.Options;
using Spackle;
using Spackle.Extensions;
using Xunit;

namespace MyVote.BusinessObjects.Tests
{
	public sealed class PollCommentsTests
	{
		[Fact]
		public void Fetch()
		{
			var generator = new RandomObjectGenerator();
			var pollId = generator.Generate<int>();

			var userEntity = EntityCreator.Create<Mvuser>();
			var pollCommentEntity = EntityCreator.Create<MvpollComment>(_ =>
			{
				_.ParentCommentId = null;
				_.PollId = pollId;
				_.UserId = userEntity.UserId;
			});
			var childPollCommentEntity = EntityCreator.Create<MvpollComment>(_ =>
			{
				_.ParentCommentId = generator.Generate<int>();
				_.PollId = pollId;
				_.UserId = userEntity.UserId;
			});

			var pollCommentFactoryCount = 0;
			var pollComment = Rock.Make<IPollComment>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));

			var pollCommentFactory = Rock.Create<IObjectFactory<IPollComment>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollCommentFactory.Handle<object[], IPollComment>(_ => _.FetchChild(Arg.IsAny<object[]>()),
				args =>
				{
					pollCommentFactoryCount++;
					return pollComment;
				});

			var addCount = 0;

			var pollCommentCollection = Rock.Create<IPollCommentCollection>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollCommentCollection.Handle<IPollComment>(_ => _.Add(pollComment), comment => addCount++);
			pollCommentCollection.Handle(_ => _.SetParent(Arg.IsAny<PollComments>()));
			pollCommentCollection.Handle(nameof(IPollCommentCollection.EditLevel), () => 0, 2);
			var pollCommentCollectionChunk = pollCommentCollection.Make();


			var pollCommentsFactory = Rock.Create<IObjectFactory<IPollCommentCollection>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollCommentsFactory.Handle(_ => _.FetchChild()).Returns(pollCommentCollectionChunk);

			var entities = Rock.Create<IEntitiesContext>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			entities.Handle(nameof(IEntitiesContext.MvpollComment),
				() => new InMemoryDbSet<MvpollComment> { pollCommentEntity, childPollCommentEntity });
			entities.Handle(nameof(IEntitiesContext.Mvuser),
				() => new InMemoryDbSet<Mvuser> { userEntity });
			entities.Handle(_ => _.Dispose());

			var builder = new ContainerBuilder();
			builder.Register<IObjectFactory<IPollComment>>(_ => pollCommentFactory.Make());
			builder.Register<IObjectFactory<IPollCommentCollection>>(_ => pollCommentsFactory.Make());
			builder.Register<IEntitiesContext>(_ => entities.Make());

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var results = DataPortal.FetchChild<PollComments>(pollId);
				results.Comments.Should().BeSameAs(pollCommentCollectionChunk);
				pollCommentFactoryCount.Should().Be(1);
				addCount.Should().Be(1);
			}

			entities.Verify();
			pollCommentsFactory.Verify();
			pollCommentCollection.Verify();
			pollCommentFactory.Verify();
		}
	}
}