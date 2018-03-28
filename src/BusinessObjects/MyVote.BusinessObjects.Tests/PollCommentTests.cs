using Autofac;
using Csla;
using FluentAssertions;
using MyVote.BusinessObjects.Contracts;
using MyVote.Data.Entities;
using Rocks;
using Rocks.Options;
using Spackle;
using Spackle.Extensions;
using System.Collections.Generic;
using Xunit;

namespace MyVote.BusinessObjects.Tests
{
	public sealed class PollCommentTests
	{
		[Fact]
		public void Create()
		{
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();
			var userName = generator.Generate<string>();

			var pollComments = Rock.Make<IPollCommentCollection>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			var pollCommentsFactory = Rock.Create<IObjectFactory<IPollCommentCollection>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollCommentsFactory.Handle(_ => _.CreateChild()).Returns(pollComments);

			var pollCommentFactory = Rock.Make<IObjectFactory<IPollComment>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));

			var builder = new ContainerBuilder();
			builder.Register<IObjectFactory<IPollComment>>(_ => pollCommentFactory);
			builder.Register<IObjectFactory<IPollCommentCollection>>(_ => pollCommentsFactory.Make());
			builder.Register<IEntitiesContext>(_ => Rock.Make<IEntitiesContext>(
				new RockOptions(allowWarnings: AllowWarnings.Yes)));

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var result = DataPortal.CreateChild<PollComment>(userId, userName);
				result.CommentDate.Should().HaveValue();
				result.Comments.Should().BeSameAs(pollComments);
				result.CommentText.Should().BeEmpty();
				result.PollCommentID.Should().NotHaveValue();
				result.UserID.Should().Be(userId);
				result.UserName.Should().Be(userName);
			}

			pollCommentsFactory.Verify();
		}

		[Fact]
		public void Fetch()
		{
			var generator = new RandomObjectGenerator();
			var userName = generator.Generate<string>();

			var pollComment = new PollCommentData
			{
				Comment = EntityCreator.Create<MvpollComment>(),
				UserName = userName
			};

			var childPollComment = new PollCommentData
			{
				Comment = EntityCreator.Create<MvpollComment>(_ =>
				{
					_.ParentCommentId = pollComment.Comment.PollCommentId;
					_.PollId = pollComment.Comment.PollId;
					_.UserId = pollComment.Comment.UserId;
				}),
				UserName = userName
			};

			var comments = new List<PollCommentData> { pollComment, childPollComment };

			var newPollComment = Rock.Make<IPollComment>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));

			var pollCommentFactoryCount = 0;
			var pollCommentFactory = Rock.Create<IObjectFactory<IPollComment>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollCommentFactory.Handle<object[], IPollComment>(_ => _.FetchChild(Arg.IsAny<object[]>()), 
				args =>
				{
					pollCommentFactoryCount++;
					return newPollComment;
				});

			var pollComments = Rock.Create<IPollCommentCollection>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollComments.Handle(_ => _.Add(newPollComment));
			pollComments.Handle(_ => _.SetParent(Arg.IsAny<PollComment>()));
			pollComments.Handle(nameof(IPollCommentCollection.EditLevel), () => 0, 2);
			var pollCommentsChunk = pollComments.Make();

			var pollCommentsFactoryCount = 0;
			var pollCommentsFactory = Rock.Create<IObjectFactory<IPollCommentCollection>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollCommentsFactory.Handle(_ => _.FetchChild(),
				() =>
				{
					pollCommentsFactoryCount++;
					return pollCommentsChunk;
				});

			var builder = new ContainerBuilder();
			builder.Register<IObjectFactory<IPollComment>>(_ => pollCommentFactory.Make());
			builder.Register<IObjectFactory<IPollCommentCollection>>(_ => pollCommentsFactory.Make());
			builder.Register<IEntitiesContext>(_ => Rock.Make<IEntitiesContext>(
				new RockOptions(allowWarnings: AllowWarnings.Yes)));

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var result = DataPortal.FetchChild<PollComment>(pollComment, comments);
				result.CommentDate.Should().Be(pollComment.Comment.CommentDate);
				result.CommentText.Should().Be(pollComment.Comment.CommentText);
				result.PollCommentID.Should().Be(pollComment.Comment.PollCommentId);
				result.UserID.Should().Be(pollComment.Comment.UserId);
				result.UserName.Should().Be(pollComment.UserName);
				result.Comments.Should().BeSameAs(pollCommentsChunk);
			}

			pollCommentsFactory.Verify();
			pollComments.Verify();
			pollCommentFactory.Verify();
		}

		[Fact]
		public void Insert()
		{
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();
			var pollId = generator.Generate<int>();
			var parentCommentId = generator.Generate<int>();
			var pollCommentId = generator.Generate<int>();
			var userName = generator.Generate<string>();
			var commentText = generator.Generate<string>();

			var pollComments = Rock.Make<IPollCommentCollectionChildUpdate>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			var pollCommentsFactory = Rock.Create<IObjectFactory<IPollCommentCollection>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollCommentsFactory.Handle(_ => _.CreateChild()).Returns(pollComments);

			var pollCommentFactory = Rock.Make<IObjectFactory<IPollComment>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));

			var entities = Rock.Create<IEntitiesContext>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			var commentEntities = new InMemoryDbSet<MvpollComment>();
			entities.Handle(nameof(IEntitiesContext.MvpollComment), () => commentEntities);
			entities.Handle(_ => _.SaveChanges(),
				() =>
				{
					commentEntities.LocalData[0].PollCommentId = pollCommentId;
					return 1;
				});
			entities.Handle(_ => _.Dispose());

			var builder = new ContainerBuilder();
			builder.Register<IObjectFactory<IPollComment>>(_ => pollCommentFactory);
			builder.Register<IObjectFactory<IPollCommentCollection>>(_ => pollCommentsFactory.Make());
			builder.Register<IEntitiesContext>(_ => entities.Make());

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var result = DataPortal.CreateChild<PollComment>(userId, userName);
				result.CommentText = commentText;
				DataPortal.UpdateChild(result, pollId, new int?(parentCommentId));

				result.CommentText.Should().Be(commentText);
				result.PollCommentID.Should().Be(pollCommentId);

				var savedEntity = commentEntities.LocalData[0];

				savedEntity.CommentDate.Should().HaveValue();
				savedEntity.UserId.Should().Be(userId);
				savedEntity.PollId.Should().Be(pollId);
				savedEntity.ParentCommentId.Value.Should().Be(parentCommentId);
				savedEntity.CommentText.Should().Be(commentText);
			}

			pollCommentsFactory.Verify();
		}

		public interface IChildUpdate
		{
			void Child_Update(int pollId, int? parentCommentId);
		}

		public interface IPollCommentCollectionChildUpdate
			: IPollCommentCollection, IChildUpdate
		{ }
	}
}
