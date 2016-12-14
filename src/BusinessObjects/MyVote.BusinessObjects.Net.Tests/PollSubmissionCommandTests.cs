using Autofac;
using Csla;
using FluentAssertions;
using Moq;
using MyVote.BusinessObjects.Contracts;
using MyVote.Data.Entities;
using Spackle;
using Spackle.Extensions;
using Xunit;

namespace MyVote.BusinessObjects.Net.Tests
{
	public sealed class PollSubmissionCommandTests
	{
		[Fact]
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

				command.PollID.Should().Be(submission.PollID);
				command.UserID.Should().Be(submission.UserID);
				command.Submission.Should().BeNull();
			}

			entities.VerifyAll();
		}

		[Fact]
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

				command.PollID.Should().Be(pollId);
				command.UserID.Should().Be(userId);
				command.Submission.Should().NotBeNull();
			}

			factory.VerifyAll();
			entities.VerifyAll();
		}
	}
}
