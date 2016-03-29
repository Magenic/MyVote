using Autofac;
using Csla;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyVote.BusinessObjects.Contracts;
using MyVote.Data.Entities;
using Spackle;
using Spackle.Extensions;

namespace MyVote.BusinessObjects.Net.Tests
{
	[TestClass]
	public sealed class PollSubmissionCommandTests
	{
		[TestMethod]
		public void ExecuteWhenUserHasSubmittedAnswersForThePoll()
		{
			var submission = EntityCreator.Create<MVPollSubmission>();

			var entities = new Mock<IEntities>(MockBehavior.Strict);
			entities.Setup(_ => _.MVPollSubmissions).Returns(
				new InMemoryDbSet<MVPollSubmission> { submission });
			entities.Setup(_ => _.Dispose());

			var factory = Mock.Of<IObjectFactory<IPollSubmission>>();

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => entities.Object);
			builder.Register<IObjectFactory<IPollSubmission>>(_ => factory);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var command = DataPortal.Execute<PollSubmissionCommand>(
					new PollSubmissionCommand
					{
						PollID = submission.PollID,
						UserID = submission.UserID
					});

				Assert.AreEqual(submission.PollID, command.PollID, nameof(command.PollID));
				Assert.AreEqual(submission.UserID, command.UserID, nameof(command.UserID));
				Assert.IsNull(command.Submission, nameof(command.Submission));
			}

			entities.VerifyAll();
		}

		[TestMethod]
		public void ExecuteWhenUserHasNotSubmittedAnswersForThePoll()
		{
			var generator = new RandomObjectGenerator();
			var pollId = generator.Generate<int>();
			var userId = generator.Generate<int>();

			var entities = new Mock<IEntities>(MockBehavior.Strict);
			entities.Setup(_ => _.MVPollSubmissions).Returns(
				new InMemoryDbSet<MVPollSubmission>());
			entities.Setup(_ => _.Dispose());

			var factory = new Mock<IObjectFactory<IPollSubmission>>(MockBehavior.Strict);
			factory.Setup(_ => _.Create(It.IsAny<PollSubmissionCriteria>()))
				.Returns(Mock.Of<IPollSubmission>());

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => entities.Object);
			builder.Register<IObjectFactory<IPollSubmission>>(_ => factory.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var command = DataPortal.Execute<PollSubmissionCommand>(
					new PollSubmissionCommand
					{
						PollID = pollId,
						UserID = userId
					});

				Assert.AreEqual(pollId, command.PollID, nameof(command.PollID));
				Assert.AreEqual(userId, command.UserID, nameof(command.UserID));
				Assert.IsNotNull(command.Submission, nameof(command.Submission));
			}

			factory.VerifyAll();
			entities.VerifyAll();
		}
	}
}
