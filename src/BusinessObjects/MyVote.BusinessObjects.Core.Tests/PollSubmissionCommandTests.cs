using Autofac;
using Csla;
using FluentAssertions;
using Moq;
using MyVote.BusinessObjects.Contracts;
using MyVote.Data.Entities;
using Spackle;
using Spackle.Extensions;
using System.Threading.Tasks;
using Xunit;

namespace MyVote.BusinessObjects.Tests
{
	public sealed class PollSubmissionCommandTests
	{
		[Fact]
		public void ExecuteWhenUserHasSubmittedAnswersForThePoll()
		{
			var submission = EntityCreator.Create<MvpollSubmission>();

			var entities = new Mock<IEntitiesContext>(MockBehavior.Strict);
			entities.Setup(_ => _.MvpollSubmission).Returns(
				new InMemoryDbSet<MvpollSubmission> { submission });
			entities.Setup(_ => _.Dispose());

			var factory = Mock.Of<IObjectFactory<IPollSubmission>>();

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => entities.Object);
			builder.Register<IObjectFactory<IPollSubmission>>(_ => factory);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var command = DataPortal.Execute<PollSubmissionCommand>(
					new PollSubmissionCommand
					{
						PollID = submission.PollId,
						UserID = submission.UserId
					});

				command.PollID.Should().Be(submission.PollId);
				command.UserID.Should().Be(submission.UserId);
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

			var entities = new Mock<IEntitiesContext>(MockBehavior.Strict);
			entities.Setup(_ => _.MvpollSubmission).Returns(
				new InMemoryDbSet<MvpollSubmission>());
			entities.Setup(_ => _.Dispose());

			var factory = new Mock<IObjectFactory<IPollSubmission>>(MockBehavior.Strict);
			factory.Setup(_ => _.CreateAsync(It.IsAny<PollSubmissionCriteria>()))
				.Returns(Task.FromResult<IPollSubmission>(Mock.Of<IPollSubmission>()));

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => entities.Object);
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
