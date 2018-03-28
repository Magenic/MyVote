using System;
using System.Collections.Generic;
using System.Linq;
using MyVote.Services.AppServer.Auth;
using MyVote.Services.AppServer.Controllers;
using MyVote.Services.AppServer.Models;
using MyVote.BusinessObjects;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;
using MyVote.BusinessObjects.Core.Contracts;
using Spackle;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Rocks;
using Rocks.Options;

namespace MyVote.Services.AppServer.Tests.Controllers
{
	public sealed class PollControllerTests
	{
		[Fact]
		public void GetPoll()
		{
			var generator = new RandomObjectGenerator();
			var category = generator.Generate<string>();
			var id = generator.Generate<int>();
			var imageLink = generator.Generate<string>();
			var question = generator.Generate<string>();
			var submissionCount = generator.Generate<int>();

			var pollSearchResult = Rock.Create<IPollSearchResult>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollSearchResult.Handle(nameof(IPollSearchResult.Id), () => id);
			pollSearchResult.Handle(nameof(IPollSearchResult.ImageLink), () => imageLink);
			pollSearchResult.Handle(nameof(IPollSearchResult.Question), () => question);
			pollSearchResult.Handle(nameof(IPollSearchResult.SubmissionCount), () => submissionCount);

			var searchResults = Rock.Create<IReadOnlyListBaseCore<IPollSearchResult>>();
			searchResults.Handle(_ => _.GetEnumerator()).Returns(new List<IPollSearchResult> { pollSearchResult.Make() }.GetEnumerator());

			var pollSearchResultsByCategory = Rock.Create<IPollSearchResultsByCategory>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollSearchResultsByCategory.Handle(nameof(IPollSearchResultsByCategory.SearchResults), () => searchResults.Make());
			pollSearchResultsByCategory.Handle(nameof(IPollSearchResultsByCategory.Category), () => category);

			var searchResultsByCategory = Rock.Create<IReadOnlyListBaseCore<IPollSearchResultsByCategory>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			searchResultsByCategory.Handle(_ => _.GetEnumerator())
				.Returns(new List<IPollSearchResultsByCategory> { pollSearchResultsByCategory.Make() }.GetEnumerator());

			var pollSearchResults = Rock.Create<IPollSearchResults>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollSearchResults.Handle(nameof(IPollSearchResults.SearchResultsByCategory), () => searchResultsByCategory.Make());

			var pollSearchResultsFactory = Rock.Create<IObjectFactory<IPollSearchResults>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollSearchResultsFactory.Handle(_ => _.Fetch(PollSearchResultsQueryType.MostPopular)).Returns(pollSearchResults.Make());

			var controller = new PollController(new Lazy<IObjectFactory<IPoll>>(),
				new Lazy<IObjectFactory<IPollOption>>(),
				new Lazy<IObjectFactory<IPollSearchResults>>(() => pollSearchResultsFactory.Make()),
				Rock.Make<IMyVoteAuthentication>());
			var polls = (controller.Get() as OkObjectResult).Value as List<PollSummary>;

			polls.Count().Should().Be(1);
			var poll = polls.First();
			poll.Category.Should().Be(category);
			poll.Id.Should().Be(id);
			poll.ImageLink.Should().Be(imageLink);
			poll.Question.Should().Be(question);
			poll.SubmissionCount.Should().Be(submissionCount);

			pollSearchResultsFactory.Verify();
			pollSearchResults.Verify();
			searchResultsByCategory.Verify();
			pollSearchResultsByCategory.Verify();
			searchResults.Verify();
			pollSearchResult.Verify();
		}

		[Fact]
		public void GetPollByPollSearchResultsQueryType()
		{
			var searchResultsByCategory = Rock.Create<IReadOnlyListBaseCore<IPollSearchResultsByCategory>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			searchResultsByCategory.Handle(_ => _.GetEnumerator()).Returns(new List<IPollSearchResultsByCategory>().GetEnumerator());

			var pollSearchResults = Rock.Create<IPollSearchResults>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollSearchResults.Handle(nameof(IPollSearchResults.SearchResultsByCategory), () => searchResultsByCategory.Make());

			var pollSearchResultsFactory = Rock.Create<IObjectFactory<IPollSearchResults>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollSearchResultsFactory.Handle(_ => _.Fetch(PollSearchResultsQueryType.Newest)).Returns(pollSearchResults.Make());

			var controller = new PollController(new Lazy<IObjectFactory<IPoll>>(),
				new Lazy<IObjectFactory<IPollOption>>(),
				new Lazy<IObjectFactory<IPollSearchResults>>(() => pollSearchResultsFactory.Make()),
				Rock.Make<IMyVoteAuthentication>());
			var polls = (controller.Get(PollSearchResultsQueryType.Newest.ToString()) as OkObjectResult).Value as List<PollSummary>;

			polls.Count().Should().Be(0);

			pollSearchResultsFactory.Verify();
			pollSearchResults.Verify();
			searchResultsByCategory.Verify();
		}

		[Fact]
		public void GetSinglePoll()
		{
			var generator = new RandomObjectGenerator();
			var pollId = generator.Generate<int>();
			var userId = generator.Generate<int>();
			var pollCategoryID = generator.Generate<int>();
			var pollQuestion = generator.Generate<string>();
			var pollImageLink = generator.Generate<string>();
			var pollMaxAnswers = generator.Generate<short>();
			var pollMinAnswers = generator.Generate<short>();
			var pollStartDate = generator.Generate<DateTime>();
			var pollEndDate = generator.Generate<DateTime>();
			var pollAdminRemovedFlag = generator.Generate<bool>();
			var pollDateRemoved = generator.Generate<DateTime>();
			var pollDeletedFlag = generator.Generate<bool>();
			var pollDeletedDate = generator.Generate<DateTime>();
			var pollDescription = generator.Generate<string>();
			var pollOptionId = generator.Generate<int>();
			var optionPosition = generator.Generate<short>();
			var optionText = generator.Generate<string>();

			var pollOption = Rock.Create<IPollOption>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollOption.Handle(_ => _.SetParent(Arg.IsAny<BusinessList<IPollOption>>()));
			pollOption.Handle(nameof(IPollOption.IsChild), () => true);
			pollOption.Handle(nameof(IPollOption.PollID), () => pollId as int?);
			pollOption.Handle(nameof(IPollOption.PollOptionID), () => pollOptionId as int?);
			pollOption.Handle(nameof(IPollOption.OptionPosition), () => optionPosition as short?);
			pollOption.Handle(nameof(IPollOption.OptionText), () => optionText);
			pollOption.Handle(nameof(IPollOption.EditLevel), () => 0, 2);
			pollOption.Handle<int>(nameof(IPollOption.EditLevelAdded), _ => { });

			var poll = Rock.Create<IPoll>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			poll.Handle(nameof(IPoll.PollID), () => pollId as int?);
			poll.Handle(nameof(IPoll.UserID), () => userId);
			poll.Handle(nameof(IPoll.PollCategoryID), () => pollCategoryID as int?);
			poll.Handle(nameof(IPoll.PollQuestion), () => pollQuestion);
			poll.Handle(nameof(IPoll.PollImageLink), () => pollImageLink);
			poll.Handle(nameof(IPoll.PollMaxAnswers), () => pollMaxAnswers as short?);
			poll.Handle(nameof(IPoll.PollMinAnswers), () => pollMinAnswers as short?);
			poll.Handle(nameof(IPoll.PollStartDate), () => pollStartDate as DateTime?);
			poll.Handle(nameof(IPoll.PollEndDate), () => pollEndDate as DateTime?);
			poll.Handle(nameof(IPoll.PollAdminRemovedFlag), () => pollAdminRemovedFlag as bool?);
			poll.Handle(nameof(IPoll.PollDateRemoved), () => pollDateRemoved as DateTime?);
			poll.Handle(nameof(IPoll.PollDeletedFlag), () => pollDeletedFlag as bool?);
			poll.Handle(nameof(IPoll.PollDeletedDate), () => pollDeletedDate as DateTime?);
			poll.Handle(nameof(IPoll.PollDescription), () => pollDescription);
			poll.Handle(nameof(IPoll.PollOptions), () => new BusinessList<IPollOption> { pollOption.Make() });

			var pollFactory = Rock.Create<IObjectFactory<IPoll>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollFactory.Handle(_ => _.Fetch(pollId)).Returns(poll.Make());

			var controller = new PollController(new Lazy<IObjectFactory<IPoll>>(() => pollFactory.Make()),
				new Lazy<IObjectFactory<IPollOption>>(),
				new Lazy<IObjectFactory<IPollSearchResults>>(),
				Rock.Make<IMyVoteAuthentication>(new RockOptions(allowWarnings: AllowWarnings.Yes)));
			var pollResult = (controller.Get(pollId) as OkObjectResult).Value as Poll;

			pollResult.PollID.Should().Be(pollId);
			pollResult.UserID.Should().Be(userId);
			pollResult.PollCategoryID.Should().Be(pollCategoryID);
			pollResult.PollQuestion.Should().Be(pollQuestion);
			pollResult.PollImageLink.Should().Be(pollImageLink);
			pollResult.PollMaxAnswers.Should().Be(pollMaxAnswers);
			pollResult.PollMinAnswers.Should().Be(pollMinAnswers);
			pollResult.PollStartDate.Should().Be(pollStartDate);
			pollResult.PollEndDate.Should().Be(pollEndDate);
			pollResult.PollAdminRemovedFlag.Should().Be(pollAdminRemovedFlag);
			pollResult.PollDateRemoved.Should().Be(pollDateRemoved);
			pollResult.PollDeletedFlag.Should().Be(pollDeletedFlag);
			pollResult.PollDeletedDate.Should().Be(pollDeletedDate);
			pollResult.PollDescription.Should().Be(pollDescription);
			pollResult.PollOptions.Count.Should().Be(1);

			var pollOptionResult = pollResult.PollOptions[0];
			pollOptionResult.PollOptionID.Should().Be(pollOptionId);
			pollOptionResult.PollID.Should().Be(pollId);
			pollOptionResult.OptionPosition.Should().Be(optionPosition);
			pollOptionResult.OptionText.Should().Be(optionText);

			pollFactory.Verify();
			pollOption.Verify();
			poll.Verify();
		}


		[Fact(Skip = "debugging CreatedAtRoute 500 issue")]
		public async Task CreatePoll()
		{
			await PollControllerTests.TestPollPut((pollFactory, pollModel, poll) =>
			{
				pollFactory.Handle(_ => _.Create(pollModel.UserID)).Returns(poll.Make());
			}, async (controller, pollModel) =>
			{
				var result = await controller.Put(pollModel);
				result.Should().BeOfType<CreatedAtRouteResult>();
			}, () => true);
		}

		[Fact]
		public async Task UpdatePoll()
		{
			await PollControllerTests.TestPollPut((pollFactory, pollModel, poll) =>
			{
				pollFactory.Handle(_ => _.Fetch(pollModel.PollID)).Returns(poll.Make());
			}, async (controller, pollModel) =>
			{
				var result = await controller.Put(pollModel.PollID, pollModel);
				result.Should().BeOfType<NoContentResult>();
			}, () => false);
		}

		private static async Task TestPollPut(Action<IRock<IObjectFactory<IPoll>>, Poll, IRock<IPoll>> pollFactorySetup,
			Func<PollController, Poll, Task> pollControllerInvocation, Func<bool> verifyPoll)
		{
			var pollModel = EntityCreator.Create<Poll>(_ => _.PollOptions = null);

			var poll = Rock.Create<IPoll>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			poll.Handle<int?>(nameof(IPoll.PollCategoryID), _ => { });
			poll.Handle<string>(nameof(IPoll.PollQuestion), _ => { });
			poll.Handle<string>(nameof(IPoll.PollImageLink), _ => { });
			poll.Handle<short?>(nameof(IPoll.PollMaxAnswers), _ => { });
			poll.Handle<short?>(nameof(IPoll.PollMinAnswers), _ => { });
			poll.Handle<DateTime?>(nameof(IPoll.PollStartDate), _ => { });
			poll.Handle<DateTime?>(nameof(IPoll.PollEndDate), _ => { });
			poll.Handle<bool?>(nameof(IPoll.PollAdminRemovedFlag), _ => { });
			poll.Handle<DateTime?>(nameof(IPoll.PollDateRemoved), _ => { });
			poll.Handle<bool?>(nameof(IPoll.PollDeletedFlag), _ => { });
			poll.Handle<DateTime?>(nameof(IPoll.PollDeletedDate), _ => { });
			poll.Handle<string>(nameof(IPoll.PollDescription), _ => { });

			poll.Handle(nameof(IPoll.PollID), () => pollModel.PollID as int?);
			poll.Handle(nameof(IPoll.UserID), () => pollModel.UserID);
			poll.Handle(nameof(IPoll.PollCategoryID), () => pollModel.PollCategoryID as int?);
			poll.Handle(nameof(IPoll.PollQuestion), () => pollModel.PollQuestion);
			poll.Handle(nameof(IPoll.PollImageLink), () => pollModel.PollImageLink);
			poll.Handle(nameof(IPoll.PollMaxAnswers), () => pollModel.PollMaxAnswers as short?);
			poll.Handle(nameof(IPoll.PollMinAnswers), () => pollModel.PollMinAnswers as short?);
			poll.Handle(nameof(IPoll.PollStartDate), () => pollModel.PollStartDate as DateTime?);
			poll.Handle(nameof(IPoll.PollEndDate), () => pollModel.PollEndDate as DateTime?);
			poll.Handle(nameof(IPoll.PollAdminRemovedFlag), () => pollModel.PollAdminRemovedFlag as bool?);
			poll.Handle(nameof(IPoll.PollDateRemoved), () => pollModel.PollDateRemoved as DateTime?);
			poll.Handle(nameof(IPoll.PollDeletedFlag), () => pollModel.PollDeletedFlag as bool?);
			poll.Handle(nameof(IPoll.PollDeletedDate), () => pollModel.PollDeletedDate as DateTime?);
			poll.Handle(nameof(IPoll.PollDescription), () => pollModel.PollDescription);
			poll.Handle(nameof(IPoll.PollOptions), () => new BusinessList<IPollOption>());
			poll.Handle(nameof(IPoll.IsSavable), () => true);
			poll.Handle(_ => _.SaveAsync()).Returns(Task.FromResult<object>(poll.Make()));

			var pollFactory = Rock.Create<IObjectFactory<IPoll>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollFactorySetup(pollFactory, pollModel, poll);

			var auth = Rock.Create<IMyVoteAuthentication>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			auth.Handle(_ => _.GetCurrentUserID()).Returns(pollModel.UserID);

			var controller = new PollController(new Lazy<IObjectFactory<IPoll>>(() => pollFactory.Make()),
				new Lazy<IObjectFactory<IPollOption>>(),
				new Lazy<IObjectFactory<IPollSearchResults>>(), auth.Make());

			await pollControllerInvocation(controller, pollModel);

			pollFactory.Verify();

			if(verifyPoll())
			{
				poll.Verify();
			}
		}
	}
}
