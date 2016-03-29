using Autofac;
using Csla;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyVote.BusinessObjects.Contracts;
using MyVote.Data.Entities;
using Spackle;
using Spackle.Extensions;
using System.Collections.Generic;

namespace MyVote.BusinessObjects.Net.Tests
{
	[TestClass]
	public sealed class PollCommentsTests
	{
		[TestMethod]
		public void Fetch()
		{
			var generator = new RandomObjectGenerator();
			var pollId = generator.Generate<int>();

			var userEntity = EntityCreator.Create<MVUser>();
			var pollCommentEntity = EntityCreator.Create<MVPollComment>(_ =>
			{
				_.ParentCommentID = null;
				_.PollID = pollId;
				_.UserID = userEntity.UserID;
			});
			var childPollCommentEntity = EntityCreator.Create<MVPollComment>(_ =>
			{
				_.ParentCommentID = generator.Generate<int>();
				_.PollID = pollId;
				_.UserID = userEntity.UserID;
			});

			var pollCommentFactoryCount = 0;
			var pollComment = Mock.Of<IPollComment>();
			var pollCommentFactory = new Mock<IObjectFactory<IPollComment>>(MockBehavior.Strict);
			pollCommentFactory.Setup(_ => _.FetchChild(
				It.IsAny<PollCommentData>(), It.IsAny<List<PollCommentData>>()))
				.Callback(() => pollCommentFactoryCount++)
				.Returns(pollComment);

			var addCount = 0;
			var pollCommentCollection = new Mock<IPollCommentCollection>(MockBehavior.Strict);
			pollCommentCollection.Setup(_ => _.Add(pollComment)).Callback<IPollComment>(_ => addCount++);
			pollCommentCollection.Setup(_ => _.SetParent(It.IsAny<PollComments>()));
			pollCommentCollection.SetupGet(_ => _.EditLevel).Returns(0);

			var pollCommentsFactory = new Mock<IObjectFactory<IPollCommentCollection>>(MockBehavior.Strict);
			pollCommentsFactory.Setup(_ => _.FetchChild()).Returns(pollCommentCollection.Object);

			var entities = new Mock<IEntities>(MockBehavior.Strict);
			entities.Setup(_ => _.MVPollComments).Returns(
				new InMemoryDbSet<MVPollComment> { pollCommentEntity, childPollCommentEntity });
			entities.Setup(_ => _.MVUsers).Returns(
				new InMemoryDbSet<MVUser> { userEntity });
			entities.Setup(_ => _.Dispose());

			var builder = new ContainerBuilder();
			builder.Register<IObjectFactory<IPollComment>>(_ => pollCommentFactory.Object);
			builder.Register<IObjectFactory<IPollCommentCollection>>(_ => pollCommentsFactory.Object);
			builder.Register<IEntities>(_ => entities.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var results = DataPortal.FetchChild<PollComments>(pollId);
				Assert.AreSame(pollCommentCollection.Object, results.Comments);
				Assert.AreEqual(1, pollCommentFactoryCount);
				Assert.AreEqual(1, addCount);
			}

			entities.VerifyAll();
			pollCommentsFactory.VerifyAll();
			pollCommentCollection.VerifyAll();
			pollCommentFactory.VerifyAll();
		}
	}
}
