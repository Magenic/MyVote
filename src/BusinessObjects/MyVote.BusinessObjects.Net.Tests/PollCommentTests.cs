using Autofac;
using Csla;
using FluentAssertions;
using Moq;
using MyVote.BusinessObjects.Contracts;
using MyVote.Data.Entities;
using Spackle;
using Spackle.Extensions;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace MyVote.BusinessObjects.Net.Tests
{
	public sealed class PollCommentTests
	{
		[Fact]
		public void Create()
		{
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();
			var userName = generator.Generate<string>();

			var pollComments = Mock.Of<IPollCommentCollection>();
			var pollCommentsFactory = new Mock<IObjectFactory<IPollCommentCollection>>(MockBehavior.Strict);
			pollCommentsFactory.Setup(_ => _.CreateChild()).Returns(pollComments);

			var pollCommentFactory = Mock.Of<IObjectFactory<IPollComment>>();

			var builder = new ContainerBuilder();
			builder.Register<IObjectFactory<IPollComment>>(_ => pollCommentFactory);
			builder.Register<IObjectFactory<IPollCommentCollection>>(_ => pollCommentsFactory.Object);
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());

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

			pollCommentsFactory.VerifyAll();
		}

		[Fact]
		public void Fetch()
		{
			var generator = new RandomObjectGenerator();
			var userName = generator.Generate<string>();

			var pollComment = new PollCommentData
			{
				Comment = EntityCreator.Create<MVPollComment>(),
				UserName = userName
			};

			var childPollComment = new PollCommentData
			{
				Comment = EntityCreator.Create<MVPollComment>(_ =>
				{
					_.ParentCommentID = pollComment.Comment.PollCommentID;
					_.PollID = pollComment.Comment.PollID;
					_.UserID = pollComment.Comment.UserID;
				}),
				UserName = userName
			};

			var comments = new List<PollCommentData> { pollComment, childPollComment };

			var newPollComment = Mock.Of<IPollComment>();

			var pollCommentFactoryCount = 0;
			var pollCommentFactory = new Mock<IObjectFactory<IPollComment>>(MockBehavior.Strict);
			pollCommentFactory.Setup(_ => _.FetchChild(childPollComment, comments))
				.Callback<object[]>(_ => pollCommentFactoryCount++)
				.Returns(newPollComment);

			var pollComments = new Mock<IPollCommentCollection>(MockBehavior.Strict);
			pollComments.Setup(_ => _.Add(newPollComment));
			pollComments.Setup(_ => _.SetParent(It.IsAny<PollComment>()));
			pollComments.SetupGet(_ => _.EditLevel).Returns(0);

			var pollCommentsFactoryCount = 0;
			var pollCommentsFactory = new Mock<IObjectFactory<IPollCommentCollection>>(MockBehavior.Strict);
			pollCommentsFactory.Setup(_ => _.FetchChild())
				.Callback(() => pollCommentsFactoryCount++)
				.Returns(pollComments.Object);

			var builder = new ContainerBuilder();
			builder.Register<IObjectFactory<IPollComment>>(_ => pollCommentFactory.Object);
			builder.Register<IObjectFactory<IPollCommentCollection>>(_ => pollCommentsFactory.Object);
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var result = DataPortal.FetchChild<PollComment>(pollComment, comments);
				result.CommentDate.Should().Be(pollComment.Comment.CommentDate);
				result.CommentText.Should().Be(pollComment.Comment.CommentText);
				result.PollCommentID.Should().Be(pollComment.Comment.PollCommentID);
				result.UserID.Should().Be(pollComment.Comment.UserID);
				result.UserName.Should().Be(pollComment.UserName);
				result.Comments.Should().BeSameAs(pollComments.Object);
			}

			pollCommentsFactory.VerifyAll();
			pollComments.VerifyAll();
			pollCommentFactory.VerifyAll();
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

			var pollComments = new Mock<IChildUpdate>(MockBehavior.Loose).As<IPollCommentCollection>();
			var pollCommentsFactory = new Mock<IObjectFactory<IPollCommentCollection>>(MockBehavior.Strict);
			pollCommentsFactory.Setup(_ => _.CreateChild()).Returns(pollComments.Object);

			var pollCommentFactory = Mock.Of<IObjectFactory<IPollComment>>();

			var entities = new Mock<IEntities>(MockBehavior.Strict);
			var commentEntities = new InMemoryDbSet<MVPollComment>();
			entities.Setup(_ => _.MVPollComments).Returns(commentEntities);
			entities.Setup(_ => _.SaveChanges())
				.Callback(() => commentEntities.Local[0].PollCommentID = pollCommentId)
				.Returns(1);
			entities.Setup(_ => _.Dispose());

			var builder = new ContainerBuilder();
			builder.Register<IObjectFactory<IPollComment>>(_ => pollCommentFactory);
			builder.Register<IObjectFactory<IPollCommentCollection>>(_ => pollCommentsFactory.Object);
			builder.Register<IEntities>(_ => entities.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var result = DataPortal.CreateChild<PollComment>(userId, userName);
				result.CommentText = commentText;
				DataPortal.UpdateChild(result, pollId, new int?(parentCommentId));

				result.CommentText.Should().Be(commentText);
				result.PollCommentID.Should().Be(pollCommentId);

				var savedEntity = commentEntities.Local[0];

				savedEntity.CommentDate.Should().HaveValue();
				savedEntity.UserID.Should().Be(userId);
				savedEntity.PollID.Should().Be(pollId);
				savedEntity.ParentCommentID.Value.Should().Be(parentCommentId);
				savedEntity.CommentText.Should().Be(commentText);
			}

			pollCommentsFactory.VerifyAll();
		}

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public interface IChildUpdate
		{
			[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
			void Child_Update(int pollId, int? parentCommentId);
		}
	}
}
