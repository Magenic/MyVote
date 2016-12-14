using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
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

namespace MyVote.Services.AppServer.Tests
{
	public sealed class PollControllerTests
	{
		[Fact]
		public void GetPollDefault()
		{
			var generator = new RandomObjectGenerator();
			var category = generator.Generate<string>();
			var id = generator.Generate<int>();
			var imageLink = generator.Generate<string>();
			var question = generator.Generate<string>();
			var submissionCount = generator.Generate<int>();

			var pollSearchResult = new Mock<IPollSearchResult>(MockBehavior.Strict);
			pollSearchResult.SetupGet(_ => _.Id).Returns(id);
			pollSearchResult.SetupGet(_ => _.ImageLink).Returns(imageLink);
			pollSearchResult.SetupGet(_ => _.Question).Returns(question);
			pollSearchResult.SetupGet(_ => _.SubmissionCount).Returns(submissionCount);

			var searchResults = new Mock<IReadOnlyListBaseCore<IPollSearchResult>>(MockBehavior.Strict);
			searchResults.Setup(_ => _.GetEnumerator()).Returns(new List<IPollSearchResult> { pollSearchResult.Object }.GetEnumerator());

			var pollSearchResultsByCategory = new Mock<IPollSearchResultsByCategory>(MockBehavior.Strict);
			pollSearchResultsByCategory.SetupGet(_ => _.SearchResults).Returns(searchResults.Object);
			pollSearchResultsByCategory.SetupGet(_ => _.Category).Returns(category);

			var searchResultsByCategory = new Mock<IReadOnlyListBaseCore<IPollSearchResultsByCategory>>(MockBehavior.Strict);
			searchResultsByCategory.Setup(_ => _.GetEnumerator()).Returns(new List<IPollSearchResultsByCategory> { pollSearchResultsByCategory.Object }.GetEnumerator());

			var pollSearchResults = new Mock<IPollSearchResults>(MockBehavior.Strict);
			pollSearchResults.SetupGet(_ => _.SearchResultsByCategory).Returns(searchResultsByCategory.Object);

			var pollSearchResultsFactory = new Mock<IObjectFactory<IPollSearchResults>>(MockBehavior.Strict);
			pollSearchResultsFactory.Setup(_ => _.Fetch(PollSearchResultsQueryType.MostPopular)).Returns(pollSearchResults.Object);

			var controller = new PollController();
			controller.PollSearchResultsFactory = new Lazy<IObjectFactory<IPollSearchResults>>(() => pollSearchResultsFactory.Object);
			var polls = controller.Get();

			polls.Count().Should().Be(1);
			var poll = polls.First();
			poll.Category.Should().Be(category);
			poll.Id.Should().Be(id);
			poll.ImageLink.Should().Be(imageLink);
			poll.Question.Should().Be(question);
			poll.SubmissionCount.Should().Be(submissionCount);

			pollSearchResultsFactory.VerifyAll();
			pollSearchResults.VerifyAll();
			searchResultsByCategory.VerifyAll();
			pollSearchResultsByCategory.VerifyAll();
			searchResults.VerifyAll();
			pollSearchResult.VerifyAll();
		}

		[Fact]
		public void GetPollNewest()
		{
			var searchResultsByCategory = new Mock<IReadOnlyListBaseCore<IPollSearchResultsByCategory>>(MockBehavior.Strict);
			searchResultsByCategory.Setup(_ => _.GetEnumerator()).Returns(new List<IPollSearchResultsByCategory>().GetEnumerator());

			var pollSearchResults = new Mock<IPollSearchResults>(MockBehavior.Strict);
			pollSearchResults.SetupGet(_ => _.SearchResultsByCategory).Returns(searchResultsByCategory.Object);

			var pollSearchResultsFactory = new Mock<IObjectFactory<IPollSearchResults>>(MockBehavior.Strict);
			pollSearchResultsFactory.Setup(_ => _.Fetch(PollSearchResultsQueryType.Newest)).Returns(pollSearchResults.Object);

			var controller = new PollController();
			controller.PollSearchResultsFactory = new Lazy<IObjectFactory<IPollSearchResults>>(() => pollSearchResultsFactory.Object);
			var polls = controller.Get("newest");

			polls.Count().Should().Be(0);

			pollSearchResultsFactory.VerifyAll();
			pollSearchResults.VerifyAll();
			searchResultsByCategory.VerifyAll();
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

			var pollOption = new Mock<IPollOption>(MockBehavior.Strict);
			pollOption.Setup(_ => _.SetParent(It.IsAny<BusinessList<IPollOption>>()));
			pollOption.SetupGet(_ => _.IsChild).Returns(true);
			pollOption.SetupGet(_ => _.PollID).Returns(pollId);
			pollOption.SetupGet(_ => _.PollOptionID).Returns(pollOptionId);
			pollOption.SetupGet(_ => _.OptionPosition).Returns(optionPosition);
			pollOption.SetupGet(_ => _.OptionText).Returns(optionText);
			pollOption.SetupGet(_ => _.EditLevel).Returns(0);
			pollOption.SetupSet(_ => _.EditLevelAdded = 0);

			var poll = new Mock<IPoll>(MockBehavior.Strict);
			poll.SetupGet(_ => _.PollID).Returns(pollId);
			poll.SetupGet(_ => _.UserID).Returns(userId);
			poll.SetupGet(_ => _.PollCategoryID).Returns(pollCategoryID);
			poll.SetupGet(_ => _.PollQuestion).Returns(pollQuestion);
			poll.SetupGet(_ => _.PollImageLink).Returns(pollImageLink);
			poll.SetupGet(_ => _.PollMaxAnswers).Returns(pollMaxAnswers);
			poll.SetupGet(_ => _.PollMinAnswers).Returns(pollMinAnswers);
			poll.SetupGet(_ => _.PollStartDate).Returns(pollStartDate);
			poll.SetupGet(_ => _.PollEndDate).Returns(pollEndDate);
			poll.SetupGet(_ => _.PollAdminRemovedFlag).Returns(pollAdminRemovedFlag);
			poll.SetupGet(_ => _.PollDateRemoved).Returns(pollDateRemoved);
			poll.SetupGet(_ => _.PollDeletedFlag).Returns(pollDeletedFlag);
			poll.SetupGet(_ => _.PollDeletedDate).Returns(pollDeletedDate);
			poll.SetupGet(_ => _.PollDescription).Returns(pollDescription);
			poll.SetupGet(_ => _.PollOptions).Returns(new BusinessList<IPollOption> { pollOption.Object });

			var pollFactory = new Mock<IObjectFactory<IPoll>>(MockBehavior.Strict);
			pollFactory.Setup(_ => _.Fetch(pollId)).Returns(poll.Object);

			var controller = new PollController();
			controller.PollFactory = new Lazy<IObjectFactory<IPoll>>(() => pollFactory.Object);
			var pollResult = controller.Get(pollId);

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

			pollFactory.VerifyAll();
			pollOption.VerifyAll();
			poll.VerifyAll();
		}

		private static void TestPollPut(Action<Mock<IObjectFactory<IPoll>>, Poll, Mock<IPoll>> pollFactorySetup,
			Func<PollController, Poll, Poll> pollControllerInvocation)
		{
			var pollModel = EntityCreator.Create<Poll>(_ => _.PollOptions = null);

			var poll = new Mock<IPoll>(MockBehavior.Strict);
			poll.SetupSet(_ => _.PollCategoryID = pollModel.PollCategoryID);
			poll.SetupSet(_ => _.PollQuestion = pollModel.PollQuestion);
			poll.SetupSet(_ => _.PollImageLink = pollModel.PollImageLink);
			poll.SetupSet(_ => _.PollMaxAnswers = pollModel.PollMaxAnswers);
			poll.SetupSet(_ => _.PollMinAnswers = pollModel.PollMinAnswers);
			poll.SetupSet(_ => _.PollStartDate = pollModel.PollStartDate);
			poll.SetupSet(_ => _.PollEndDate = pollModel.PollEndDate);
			poll.SetupSet(_ => _.PollAdminRemovedFlag = pollModel.PollAdminRemovedFlag);
			poll.SetupSet(_ => _.PollDateRemoved = pollModel.PollDateRemoved);
			poll.SetupSet(_ => _.PollDeletedFlag = pollModel.PollDeletedFlag);
			poll.SetupSet(_ => _.PollDeletedDate = pollModel.PollDeletedDate);
			poll.SetupSet(_ => _.PollDescription = pollModel.PollDescription);

			poll.SetupGet(_ => _.PollID).Returns(pollModel.PollID);
			poll.SetupGet(_ => _.UserID).Returns(pollModel.UserID);
			poll.SetupGet(_ => _.PollCategoryID).Returns(pollModel.PollCategoryID);
			poll.SetupGet(_ => _.PollQuestion).Returns(pollModel.PollQuestion);
			poll.SetupGet(_ => _.PollImageLink).Returns(pollModel.PollImageLink);
			poll.SetupGet(_ => _.PollMaxAnswers).Returns(pollModel.PollMaxAnswers);
			poll.SetupGet(_ => _.PollMinAnswers).Returns(pollModel.PollMinAnswers);
			poll.SetupGet(_ => _.PollStartDate).Returns(pollModel.PollStartDate);
			poll.SetupGet(_ => _.PollEndDate).Returns(pollModel.PollEndDate);
			poll.SetupGet(_ => _.PollAdminRemovedFlag).Returns(pollModel.PollAdminRemovedFlag);
			poll.SetupGet(_ => _.PollDateRemoved).Returns(pollModel.PollDateRemoved);
			poll.SetupGet(_ => _.PollDeletedFlag).Returns(pollModel.PollDeletedFlag);
			poll.SetupGet(_ => _.PollDeletedDate).Returns(pollModel.PollDeletedDate);
			poll.SetupGet(_ => _.PollDescription).Returns(pollModel.PollDescription);
			poll.SetupGet(_ => _.PollOptions).Returns(new BusinessList<IPollOption>());
			poll.Setup(_ => _.Save()).Returns(poll.Object);

			var pollFactory = new Mock<IObjectFactory<IPoll>>(MockBehavior.Strict);
			pollFactorySetup(pollFactory, pollModel, poll);

			var auth = new Mock<IMyVoteAuthentication>(MockBehavior.Strict);
			auth.Setup(_ => _.GetCurrentUserID()).Returns(pollModel.UserID);

			var controller = new PollController();
			controller.PollFactory = new Lazy<IObjectFactory<IPoll>>(() => pollFactory.Object);
			controller.MyVoteAuthentication = auth.Object;

			var result = pollControllerInvocation(controller, pollModel);

			pollFactory.VerifyAll();
			poll.Verify();
		}

		[Fact]
		public void CreatePoll()
		{
			PollControllerTests.TestPollPut((pollFactory, pollModel, poll) =>
			{
				pollFactory.Setup(_ => _.Create(pollModel.UserID)).Returns(poll.Object);
			}, (controller, pollModel) =>
			{
				return controller.Put(pollModel);
			});
		}

		[Fact]
		public void UpdatePoll()
		{
			PollControllerTests.TestPollPut((pollFactory, pollModel, poll) =>
			{
				pollFactory.Setup(_ => _.Fetch(pollModel.PollID)).Returns(poll.Object);
			}, (controller, pollModel) =>
			{
				return controller.Put(pollModel.PollID, pollModel);
			});
		}
	}
}
