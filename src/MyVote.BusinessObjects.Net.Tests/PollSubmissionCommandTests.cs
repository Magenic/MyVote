using Autofac;
using Csla;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyVote.BusinessObjects.Contracts;
using MyVote.Core;
using MyVote.Core.Extensions;
using MyVote.Repository;
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

			var entities = new Mock<IEntities>();
			entities.Setup(_ => _.MVPollSubmissions).Returns(
				new InMemoryDbSet<MVPollSubmission> { submission });

			var factory = new Mock<IObjectFactory<IPollSubmission>>();

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => entities.Object);
			builder.Register<IObjectFactory<IPollSubmission>>(_ => factory.Object);

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
			{
				var command = DataPortal.Execute<PollSubmissionCommand>(
					new PollSubmissionCommand
					{
						PollID = submission.PollID,
						UserID = submission.UserID
					});

				Assert.AreEqual(submission.PollID, command.PollID, command.GetPropertyName(_ => _.PollID));
				Assert.AreEqual(submission.UserID, command.UserID, command.GetPropertyName(_ => _.UserID));
				Assert.IsNull(command.Submission, command.GetPropertyName(_ => _.Submission));
			}
		}

		[TestMethod]
		public void ExecuteWhenUserHasNotSubmittedAnswersForThePoll()
		{
			var generator = new RandomObjectGenerator();
			var pollId = generator.Generate<int>();
			var userId = generator.Generate<int>();

			var entities = new Mock<IEntities>();
			entities.Setup(_ => _.MVPollSubmissions).Returns(
				new InMemoryDbSet<MVPollSubmission>());

			var factory = new Mock<IObjectFactory<IPollSubmission>>();
			factory.Setup(_ => _.Create(It.IsAny<PollSubmissionCriteria>()))
				.Returns(new Mock<IPollSubmission>().Object);

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => entities.Object);
			builder.Register<IObjectFactory<IPollSubmission>>(_ => factory.Object);

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
			{
				var command = DataPortal.Execute<PollSubmissionCommand>(
					new PollSubmissionCommand
					{
						PollID = pollId,
						UserID = userId
					});

				Assert.AreEqual(pollId, command.PollID, command.GetPropertyName(_ => _.PollID));
				Assert.AreEqual(userId, command.UserID, command.GetPropertyName(_ => _.UserID));
				Assert.IsNotNull(command.Submission, command.GetPropertyName(_ => _.Submission));
			}
		}
	}
}
