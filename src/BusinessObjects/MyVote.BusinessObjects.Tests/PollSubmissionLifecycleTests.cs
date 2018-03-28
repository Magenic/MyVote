using Autofac;
using Csla;
using FluentAssertions;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;
using MyVote.BusinessObjects.Rules;
using MyVote.BusinessObjects.Tests.Extensions;
using MyVote.Data.Entities;
using Rocks;
using Rocks.Options;
using Spackle;
using Spackle.Extensions;
using System;
using System.Collections.Generic;
using Xunit;

namespace MyVote.BusinessObjects.Tests
{
	public sealed class PollSubmissionLifecycleTests
	{
		[Fact]
		public void Create()
		{
			var now = DateTime.UtcNow;
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();

			var pollEntity = EntityCreator.Create<Mvpoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollStartDate = now.AddDays(-2);
				_.PollEndDate = now.AddDays(2);
				_.PollMinAnswers = 2;
				_.PollMaxAnswers = 3;
			});

			pollEntity.MvpollOption = new List<MvpollOption>
			{
				EntityCreator.Create<MvpollOption>(),
				EntityCreator.Create<MvpollOption>(),
				EntityCreator.Create<MvpollOption>(),
				EntityCreator.Create<MvpollOption>()
			};

			var category = EntityCreator.Create<Mvcategory>(_ => _.CategoryId = pollEntity.PollCategoryId);

			var pollFactory = Rock.Create<IObjectFactory<IPoll>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollFactory.Handle<object, IPoll>(_ => _.Fetch(pollEntity.PollId),
				data => DataPortal.Fetch<Poll>(data));

			var pollOptions = new BusinessList<IPollOption>();

			var pollSubmissionResponsesFactory = Rock.Create<IObjectFactory<IPollSubmissionResponseCollection>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollSubmissionResponsesFactory.Handle<object[], IPollSubmissionResponseCollection>(_ => _.CreateChild(Arg.IsAny<object[]>()),
				data =>
				{
					data[0].Should().BeSameAs(pollOptions);
					return DataPortal.CreateChild<PollSubmissionResponseCollection>(data[0] as BusinessList<IPollOption>);
				});

			var entities = Rock.Create<IEntitiesContext>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			entities.Handle(nameof(IEntitiesContext.Mvpoll), () => new InMemoryDbSet<Mvpoll> { pollEntity });
			entities.Handle(nameof(IEntitiesContext.MvpollSubmission), () => new InMemoryDbSet<MvpollSubmission>());
			entities.Handle(nameof(IEntitiesContext.Mvcategory), () => new InMemoryDbSet<Mvcategory> { category });
			entities.Handle(_ => _.Dispose());

			var pollOptionsFactory = Rock.Create<IObjectFactory<BusinessList<IPollOption>>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollOptionsFactory.Handle(_ => _.FetchChild()).Returns(pollOptions);

			var pollOption = Rock.Create<IObjectFactory<IPollOption>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollOption.Handle<object[], IPollOption>(_ => _.FetchChild(Arg.IsAny<object[]>()),
				data => DataPortal.FetchChild<PollOption>(data[0] as MvpollOption), 4);

			var pollSubmissionResponseFactory = Rock.Create<IObjectFactory<IPollSubmissionResponse>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollSubmissionResponseFactory.Handle<object[], IPollSubmissionResponse>(_ => _.CreateChild(Arg.IsAny<object[]>()),
				data => DataPortal.CreateChild<PollSubmissionResponse>(data[0] as IPollOption), 4);

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => entities.Make());
			builder.Register<IObjectFactory<IPoll>>(_ => pollFactory.Make());
			builder.Register<IObjectFactory<BusinessList<IPollOption>>>(_ => pollOptionsFactory.Make());
			builder.Register<IObjectFactory<IPollOption>>(_ => pollOption.Make());
			builder.Register<IObjectFactory<IPollSubmissionResponseCollection>>(_ => pollSubmissionResponsesFactory.Make());
			builder.Register<IObjectFactory<IPollSubmissionResponse>>(_ => pollSubmissionResponseFactory.Make());

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var criteria = new PollSubmissionCriteria(pollEntity.PollId, userId);
				var submission = DataPortal.Create<PollSubmission>(criteria);

				submission.PollID.Should().Be(pollEntity.PollId);
				submission.PollImageLink.Should().Be(pollEntity.PollImageLink);
				submission.UserID.Should().Be(userId);
				submission.PollQuestion.Should().Be(pollEntity.PollQuestion);
				submission.CategoryName.Should().Be(category.CategoryName);
				submission.PollDescription.Should().Be(pollEntity.PollDescription);
				submission.PollMaxAnswers.Should().Be(pollEntity.PollMaxAnswers.Value);
				submission.PollMinAnswers.Should().Be(pollEntity.PollMinAnswers.Value);
				submission.Responses.Count.Should().Be(4);

				submission.BrokenRulesCollection.AssertRuleCount(1);
				submission.BrokenRulesCollection.AssertRuleCount(PollSubmission.ResponsesProperty, 1);
				submission.BrokenRulesCollection.AssertBusinessRuleExists<MinimumAndMaximumPollSubmissionResponsesRule>(
					PollSubmission.ResponsesProperty, true);
			}

			pollFactory.Verify();
			pollSubmissionResponseFactory.Verify();
			pollOptionsFactory.Verify();
			entities.Verify();
			pollOption.Verify();
			pollSubmissionResponsesFactory.Verify();
		}

		[Fact]
		public void CreateWhenUserHasSubmittedAnswersBefore()
		{
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();
			var pollId = generator.Generate<int>();

			var entity = EntityCreator.Create<MvpollSubmission>(_ =>
			{
				_.UserId = userId;
				_.PollId = pollId;
			});

			var entities = Rock.Create<IEntitiesContext>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			entities.Handle(nameof(IEntitiesContext.MvpollSubmission), 
				() => new InMemoryDbSet<MvpollSubmission> { entity });
			entities.Handle(_ => _.Dispose());

			var pollFactory = Rock.Make<IObjectFactory<IPoll>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));

			var pollSubmissionResponsesFactory = Rock.Make<IObjectFactory<IPollSubmissionResponseCollection>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => entities.Make());
			builder.Register<IObjectFactory<IPoll>>(_ => pollFactory);
			builder.Register<IObjectFactory<IPollSubmissionResponseCollection>>(_ => pollSubmissionResponsesFactory);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var criteria = new PollSubmissionCriteria(pollId, userId);
				new Action(() => new DataPortal<PollSubmission>().Create(criteria)).ShouldThrow<DataPortalException>();
			}

			entities.Verify();
		}

		[Fact]
		public void CreateWithStartDateInTheFuture()
		{
			var now = DateTime.UtcNow;
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();

			var poll = EntityCreator.Create<Mvpoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollStartDate = now.AddDays(2);
				_.PollEndDate = now.AddDays(4);
			});

			poll.MvpollOption = new List<MvpollOption> { EntityCreator.Create<MvpollOption>() };

			var category = EntityCreator.Create<Mvcategory>(_ => _.CategoryId = poll.PollCategoryId);

			var entities = Rock.Create<IEntitiesContext>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			entities.Handle(nameof(IEntitiesContext.Mvpoll), 
				() => new InMemoryDbSet<Mvpoll> { poll });
			entities.Handle(nameof(IEntitiesContext.MvpollSubmission),
				() => new InMemoryDbSet<MvpollSubmission>());
			entities.Handle(nameof(IEntitiesContext.Mvcategory),
				() => new InMemoryDbSet<Mvcategory> { category });
			entities.Handle(_ => _.Dispose());

			var pollOptions = Rock.Create<IObjectFactory<BusinessList<IPollOption>>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollOptions.Handle(_ => _.FetchChild()).Returns(new BusinessList<IPollOption>());

			var pollOption = Rock.Create<IObjectFactory<IPollOption>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollOption.Handle<object[], IPollOption>(_ => _.FetchChild(Arg.IsAny<object[]>()),
				data => DataPortal.FetchChild<PollOption>(data[0] as MvpollOption));

			var pollFactory = Rock.Create<IObjectFactory<IPoll>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollFactory.Handle<object, IPoll>(_ => _.Fetch(poll.PollId),
				id => DataPortal.Fetch<Poll>(id));

			var pollSubmissionResponsesFactory = Rock.Create<IObjectFactory<IPollSubmissionResponseCollection>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollSubmissionResponsesFactory.Handle<object[], IPollSubmissionResponseCollection>(_ => _.CreateChild(Arg.IsAny<object[]>()),
				data => DataPortal.CreateChild<PollSubmissionResponseCollection>(data[0] as BusinessList<IPollOption>));

			var pollSubmissionResponseFactory = Rock.Create<IObjectFactory<IPollSubmissionResponse>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollSubmissionResponseFactory.Handle<object[], IPollSubmissionResponse>(_ => _.CreateChild(Arg.IsAny<object[]>()),
				data => DataPortal.CreateChild<PollSubmissionResponse>(data[0] as IPollOption));

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => entities.Make());
			builder.Register<IObjectFactory<BusinessList<IPollOption>>>(_ => pollOptions.Make());
			builder.Register<IObjectFactory<IPollOption>>(_ => pollOption.Make());
			builder.Register<IObjectFactory<IPoll>>(_ => pollFactory.Make());
			builder.Register<IObjectFactory<IPollSubmissionResponseCollection>>(_ => pollSubmissionResponsesFactory.Make());
			builder.Register<IObjectFactory<IPollSubmissionResponse>>(_ => pollSubmissionResponseFactory.Make());

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var submission = new DataPortal<PollSubmission>().Create(
					new PollSubmissionCriteria(poll.PollId, userId));
				submission.BrokenRulesCollection.AssertRuleCount(PollSubmission.PollStartDateProperty, 1);
				submission.BrokenRulesCollection.AssertBusinessRuleExists<PollSubmissionPollStartDateRule>(
					PollSubmission.PollStartDateProperty, true);
			}

			pollFactory.Verify();
			pollSubmissionResponseFactory.Verify();
			entities.Verify();
			pollOption.Verify();
			pollSubmissionResponsesFactory.Verify();
		}

		[Fact]
		public void CreateWithEndDateInThePast()
		{
			var now = DateTime.UtcNow;
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();

			var poll = EntityCreator.Create<Mvpoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollStartDate = now.AddDays(-4);
				_.PollEndDate = now.AddDays(-2);
			});

			poll.MvpollOption = new List<MvpollOption> { EntityCreator.Create<MvpollOption>() };

			var category = EntityCreator.Create<Mvcategory>(_ => _.CategoryId = poll.PollCategoryId);

			var entities = Rock.Create<IEntitiesContext>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			entities.Handle(nameof(IEntitiesContext.Mvpoll), 
				() => new InMemoryDbSet<Mvpoll> { poll });
			entities.Handle(nameof(IEntitiesContext.MvpollSubmission), 
				() => new InMemoryDbSet<MvpollSubmission>());
			entities.Handle(nameof(IEntitiesContext.Mvcategory),
				() => new InMemoryDbSet<Mvcategory> { category });
			entities.Handle(_ => _.Dispose());

			var pollOptions = Rock.Create<IObjectFactory<BusinessList<IPollOption>>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollOptions.Handle(_ => _.FetchChild()).Returns(new BusinessList<IPollOption>());

			var pollOption = Rock.Create<IObjectFactory<IPollOption>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollOption.Handle<object[], IPollOption>(_ => _.FetchChild(Arg.IsAny<object[]>()),
				data => DataPortal.FetchChild<PollOption>(data[0] as MvpollOption));

			var pollFactory = Rock.Create<IObjectFactory<IPoll>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollFactory.Handle<object, IPoll>(_ => _.Fetch(poll.PollId),
				id => DataPortal.Fetch<Poll>(id));

			var pollSubmissionResponsesFactory = Rock.Create<IObjectFactory<IPollSubmissionResponseCollection>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollSubmissionResponsesFactory.Handle<object[], IPollSubmissionResponseCollection>(_ => _.CreateChild(Arg.IsAny<object[]>()),
				data => DataPortal.CreateChild<PollSubmissionResponseCollection>(data[0] as BusinessList<IPollOption>));

			var pollSubmissionResponseFactory = Rock.Create<IObjectFactory<IPollSubmissionResponse>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollSubmissionResponseFactory.Handle<object[], IPollSubmissionResponse>(_ => _.CreateChild(Arg.IsAny<object[]>()),
				data => DataPortal.CreateChild<PollSubmissionResponse>(data[0] as IPollOption));

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => entities.Make());
			builder.Register<IObjectFactory<BusinessList<IPollOption>>>(_ => pollOptions.Make());
			builder.Register<IObjectFactory<IPollOption>>(_ => pollOption.Make());
			builder.Register<IObjectFactory<IPoll>>(_ => pollFactory.Make());
			builder.Register<IObjectFactory<IPollSubmissionResponseCollection>>(_ => pollSubmissionResponsesFactory.Make());
			builder.Register<IObjectFactory<IPollSubmissionResponse>>(_ => pollSubmissionResponseFactory.Make());

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var submission = new DataPortal<PollSubmission>().Create(
					new PollSubmissionCriteria(poll.PollId, userId));
				submission.BrokenRulesCollection.AssertRuleCount(PollSubmission.PollEndDateProperty, 1);
				submission.BrokenRulesCollection.AssertBusinessRuleExists<PollSubmissionPollEndDateRule>(
					PollSubmission.PollEndDateProperty, true);
			}

			pollFactory.Verify();
			pollSubmissionResponseFactory.Verify();
			entities.Verify();
			pollOption.Verify();
			pollSubmissionResponsesFactory.Verify();
		}

		[Fact]
		public void CreateWhenPollIsDeleted()
		{
			var now = DateTime.UtcNow;
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();

			var poll = EntityCreator.Create<Mvpoll>(_ =>
			{
				_.PollDeletedFlag = true;
				_.PollStartDate = now.AddDays(-2);
				_.PollEndDate = now.AddDays(2);
				_.PollMinAnswers = 2;
				_.PollMaxAnswers = 3;
			});

			poll.MvpollOption = new List<MvpollOption>
			{
				EntityCreator.Create<MvpollOption>(),
				EntityCreator.Create<MvpollOption>(),
				EntityCreator.Create<MvpollOption>(),
				EntityCreator.Create<MvpollOption>()
			};

			var category = EntityCreator.Create<Mvcategory>(_ => _.CategoryId = poll.PollCategoryId);

			var pollFactory = Rock.Create<IObjectFactory<IPoll>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollFactory.Handle<object, IPoll>(_ => _.Fetch(poll.PollId),
				id => DataPortal.Fetch<Poll>(id));

			var entities = Rock.Create<IEntitiesContext>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			entities.Handle(nameof(IEntitiesContext.Mvpoll), () => new InMemoryDbSet<Mvpoll> { poll });
			entities.Handle(nameof(IEntitiesContext.MvpollSubmission), () => new InMemoryDbSet<MvpollSubmission>());
			entities.Handle(nameof(IEntitiesContext.Mvcategory), () => new InMemoryDbSet<Mvcategory> { category });
			entities.Handle(_ => _.Dispose());

			var pollOptions = Rock.Create<IObjectFactory<BusinessList<IPollOption>>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollOptions.Handle(_ => _.FetchChild()).Returns(new BusinessList<IPollOption>());

			var pollOption = Rock.Create<IObjectFactory<IPollOption>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollOption.Handle<object[], IPollOption>(_ => _.FetchChild(Arg.IsAny<object[]>()),
				data => DataPortal.FetchChild<PollOption>(data[0] as MvpollOption), 4);

			var pollSubmissionResponsesFactory = Rock.Create<IObjectFactory<IPollSubmissionResponseCollection>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollSubmissionResponsesFactory.Handle<object[], IPollSubmissionResponseCollection>(_ => _.CreateChild(Arg.IsAny<object[]>()),
				data => DataPortal.CreateChild<PollSubmissionResponseCollection>(data[0] as BusinessList<IPollOption>));

			var pollSubmissionResponseFactory = Rock.Create<IObjectFactory<IPollSubmissionResponse>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollSubmissionResponseFactory.Handle<object[], IPollSubmissionResponse>(_ => _.CreateChild(Arg.IsAny<object[]>()),
				data => DataPortal.CreateChild<PollSubmissionResponse>(data[0] as IPollOption), 4);

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => entities.Make());
			builder.Register<IObjectFactory<BusinessList<IPollOption>>>(_ => pollOptions.Make());
			builder.Register<IObjectFactory<IPollOption>>(_ => pollOption.Make());
			builder.Register<IObjectFactory<IPoll>>(_ => pollFactory.Make());
			builder.Register<IObjectFactory<IPollSubmissionResponseCollection>>(_ => pollSubmissionResponsesFactory.Make());
			builder.Register<IObjectFactory<IPollSubmissionResponse>>(_ => pollSubmissionResponseFactory.Make());

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var submission = new DataPortal<PollSubmission>().Create(
					new PollSubmissionCriteria(poll.PollId, userId));
				submission.BrokenRulesCollection.AssertRuleCount(PollSubmission.PollDeletedFlagProperty, 1);
				submission.BrokenRulesCollection.AssertBusinessRuleExists<PollSubmissionPollDeletedFlagRule>(
					PollSubmission.PollDeletedFlagProperty, true);
			}

			pollFactory.Verify();
			pollSubmissionResponseFactory.Verify();
			entities.Verify();
			pollOption.Verify();
			pollSubmissionResponsesFactory.Verify();
		}

		[Fact]
		public void Insert()
		{
			var now = DateTime.UtcNow;
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();

			var poll = EntityCreator.Create<Mvpoll>(_ =>
			{
				_.PollDeletedFlag = false;
				_.PollStartDate = now.AddDays(-2);
				_.PollEndDate = now.AddDays(2);
				_.PollMinAnswers = 2;
				_.PollMaxAnswers = 3;
			});

			poll.MvpollOption = new List<MvpollOption>
			{
				EntityCreator.Create<MvpollOption>(),
				EntityCreator.Create<MvpollOption>(),
				EntityCreator.Create<MvpollOption>(),
				EntityCreator.Create<MvpollOption>()
			};

			var category = EntityCreator.Create<Mvcategory>(_ => _.CategoryId = poll.PollCategoryId);

			var polls = new InMemoryDbSet<Mvpoll> { poll };
			var submissions = new InMemoryDbSet<MvpollSubmission>();
			var responses = new InMemoryDbSet<MvpollResponse>();

			var saveChangesCount = 0;
			var submissionId = generator.Generate<int>();

			var entities = Rock.Create<IEntitiesContext>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			entities.Handle(nameof(IEntitiesContext.Mvpoll), () => polls);
			entities.Handle(nameof(IEntitiesContext.MvpollSubmission), () => submissions);
			entities.Handle(nameof(IEntitiesContext.MvpollResponse), () => responses);
			entities.Handle(nameof(IEntitiesContext.Mvcategory), () => new InMemoryDbSet<Mvcategory> { category });
			entities.Handle(_ => _.SaveChanges(),
				() =>
				{
					if (saveChangesCount == 0)
					{
						submissions.LocalData[0].PollSubmissionId = submissionId;
					}

					saveChangesCount++;
					return 1;
				}, 5);
			entities.Handle(_ => _.Dispose());

			var pollOptions = Rock.Create<IObjectFactory<BusinessList<IPollOption>>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollOptions.Handle(_ => _.FetchChild()).Returns(new BusinessList<IPollOption>());

			var pollOption = Rock.Create<IObjectFactory<IPollOption>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollOption.Handle<object[], IPollOption>(_ => _.FetchChild(Arg.IsAny<object[]>()),
				data => DataPortal.FetchChild<PollOption>(data[0] as MvpollOption), 4);

			var pollFactory = Rock.Create<IObjectFactory<IPoll>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollFactory.Handle<object, IPoll>(_ => _.Fetch(poll.PollId),
				id => DataPortal.Fetch<Poll>(id));

			var pollSubmissionResponsesFactory = Rock.Create<IObjectFactory<IPollSubmissionResponseCollection>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollSubmissionResponsesFactory.Handle<object[], IPollSubmissionResponseCollection>(_ => _.CreateChild(Arg.IsAny<object[]>()),
				data => DataPortal.CreateChild<PollSubmissionResponseCollection>(data[0] as BusinessList<IPollOption>));

			var pollSubmissionResponseFactory = Rock.Create<IObjectFactory<IPollSubmissionResponse>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollSubmissionResponseFactory.Handle<object[], IPollSubmissionResponse>(_ => _.CreateChild(Arg.IsAny<object[]>()),
				data => DataPortal.CreateChild<PollSubmissionResponse>(data[0] as IPollOption), 4);

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => entities.Make());
			builder.Register<IObjectFactory<BusinessList<IPollOption>>>(_ => pollOptions.Make());
			builder.Register<IObjectFactory<IPollOption>>(_ => pollOption.Make());
			builder.Register<IObjectFactory<IPoll>>(_ => pollFactory.Make());
			builder.Register<IObjectFactory<IPollSubmissionResponseCollection>>(_ => pollSubmissionResponsesFactory.Make());
			builder.Register<IObjectFactory<IPollSubmissionResponse>>(_ => pollSubmissionResponseFactory.Make());

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var criteria = new PollSubmissionCriteria(poll.PollId, userId);
				var submission = new DataPortal<PollSubmission>().Create(criteria);

				submission.Responses[1].IsOptionSelected = true;
				submission.Responses[3].IsOptionSelected = true;

				submission = submission.Save();

				submissions.LocalData.Count.Should().Be(1);
				submission.PollSubmissionID.Should().Be(submissionId);
				responses.LocalData.Count.Should().Be(4);
				responses.LocalData[0].OptionSelected.Should().BeFalse();
				responses.LocalData[1].OptionSelected.Should().BeTrue();
				responses.LocalData[2].OptionSelected.Should().BeFalse();
				responses.LocalData[3].OptionSelected.Should().BeTrue();
			}

			pollFactory.Verify();
			pollSubmissionResponseFactory.Verify();
			pollOption.Verify();
			pollSubmissionResponsesFactory.Verify();
		}
	}
}