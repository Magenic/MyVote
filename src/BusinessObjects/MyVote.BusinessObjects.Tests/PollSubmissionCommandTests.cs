using Autofac;
using Csla;
using FluentAssertions;
using MyVote.BusinessObjects.Contracts;
using MyVote.Data.Entities;
using Rocks;
using Rocks.Options;
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

			var entities = Rock.Create<IEntitiesContext>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			entities.Handle(nameof(IEntitiesContext.MvpollSubmission), () =>
				new InMemoryDbSet<MvpollSubmission> { submission });
			entities.Handle(_ => _.Dispose());

			var factory = Rock.Make<IObjectFactory<IPollSubmission>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => entities.Make());
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

			entities.Verify();
		}

		[Fact]
		public void ExecuteWhenUserHasNotSubmittedAnswersForThePoll()
		{
			var generator = new RandomObjectGenerator();
			var pollId = generator.Generate<int>();
			var userId = generator.Generate<int>();

			var entities = Rock.Create<IEntitiesContext>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			entities.Handle(nameof(IEntitiesContext.MvpollSubmission), () =>
				new InMemoryDbSet<MvpollSubmission>());
			entities.Handle(_ => _.Dispose());

			var factory = Rock.Create<IObjectFactory<IPollSubmission>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			factory.Handle(_ => _.CreateAsync(Arg.IsAny<PollSubmissionCriteria>()))
				.Returns(Task.FromResult<IPollSubmission>(Rock.Make<IPollSubmission>()));

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => entities.Make());
			builder.Register<IObjectFactory<IPollSubmission>>(_ => factory.Make());

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

			factory.Verify();
			entities.Verify();
		}
	}
}
