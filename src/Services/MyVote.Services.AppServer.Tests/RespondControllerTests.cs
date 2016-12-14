using System;
using System.Collections.Generic;
using Moq;
using MyVote.Services.AppServer.Auth;
using MyVote.Services.AppServer.Controllers;
using MyVote.BusinessObjects.Contracts;
using Spackle;
using Xunit;
using FluentAssertions;

namespace MyVote.Services.AppServer.Tests
{
	public sealed class RespondControllerTests
	{
		[Fact]
		public void GetPollInfo()
		{
			var generator = new RandomObjectGenerator();
			var pollId = generator.Generate<int>();
			var pollDescription = generator.Generate<string>();
			var pollMaxAnswers = generator.Generate<short>();
			var pollMinAnswers = generator.Generate<short>();
			var comment = generator.Generate<string>();
			var pollQuestion = generator.Generate<string>();
			var pollSubmissionId = generator.Generate<int>();
			var submissionDate = generator.Generate<DateTime>();
			var userId = generator.Generate<int>();
			var isOptionSelected = generator.Generate<bool>();
			var optionPosition = generator.Generate<short>();
			var optionText = generator.Generate<string>();
			var pollOptionId = generator.Generate<int>();
			var pollResponseId = generator.Generate<int>();

			var pollSubmissionResponse = new Mock<IPollSubmissionResponse>(MockBehavior.Strict);
			pollSubmissionResponse.SetupGet(_ => _.IsOptionSelected).Returns(isOptionSelected);
			pollSubmissionResponse.SetupGet(_ => _.OptionPosition).Returns(optionPosition);
			pollSubmissionResponse.SetupGet(_ => _.OptionText).Returns(optionText);
			pollSubmissionResponse.SetupGet(_ => _.PollOptionID).Returns(pollOptionId);
			pollSubmissionResponse.SetupGet(_ => _.PollResponseID).Returns(pollResponseId);

			var pollSubmissionResponses = new Mock<IPollSubmissionResponseCollection>(MockBehavior.Strict);
			pollSubmissionResponses.Setup(_ => _.GetEnumerator()).Returns(
				new List<IPollSubmissionResponse> { pollSubmissionResponse.Object }.GetEnumerator());

			var pollSubmission = new Mock<IPollSubmission>(MockBehavior.Strict);
			pollSubmission.SetupGet(_ => _.PollID).Returns(pollId);
			pollSubmission.SetupGet(_ => _.PollDescription).Returns(pollDescription);
			pollSubmission.SetupGet(_ => _.PollMaxAnswers).Returns(pollMaxAnswers);
			pollSubmission.SetupGet(_ => _.PollMinAnswers).Returns(pollMinAnswers);
			pollSubmission.SetupGet(_ => _.Comment).Returns(comment);
			pollSubmission.SetupGet(_ => _.PollQuestion).Returns(pollQuestion);
			pollSubmission.SetupGet(_ => _.PollSubmissionID).Returns(pollSubmissionId);
			pollSubmission.SetupGet(_ => _.SubmissionDate).Returns(submissionDate);
			pollSubmission.SetupGet(_ => _.UserID).Returns(userId);
			pollSubmission.SetupGet(_ => _.Responses).Returns(pollSubmissionResponses.Object);

			var pollSubmissionFactory = new Mock<IObjectFactory<IPollSubmission>>(MockBehavior.Strict);
			pollSubmissionFactory.Setup(_ => _.Fetch(It.IsAny<object>())).Returns(pollSubmission.Object);

			var auth = new Mock<IMyVoteAuthentication>(MockBehavior.Strict);
			auth.Setup(_ => _.GetCurrentUserID()).Returns(userId);

			var controller = new RespondController();
			controller.PollSubmissionFactory = pollSubmissionFactory.Object;
			controller.MyVoteAuthentication = auth.Object;

			var result = controller.Get(pollId, userId);
			result.PollID.Should().Be(pollId);
			result.PollDescription.Should().Be(pollDescription);
			result.MaxAnswers.Should().Be(pollMaxAnswers);
			result.MinAnswers.Should().Be(pollMinAnswers);
			result.Comment.Should().Be(comment);
			result.PollQuestion.Should().Be(pollQuestion);
			result.PollSubmissionID.Should().Be(pollSubmissionId);
			result.SubmissionDate.Should().Be(submissionDate);
			result.UserID.Should().Be(userId);
			result.PollOptions.Count.Should().Be(1);

			var option = result.PollOptions[0];
			option.IsOptionSelected.Should().Be(isOptionSelected);
			option.OptionPosition.Should().Be(optionPosition);
			option.OptionText.Should().Be(optionText);
			option.PollOptionID.Should().Be(pollOptionId);
			option.PollResponseID.Should().Be(pollResponseId);

			pollSubmissionFactory.VerifyAll();
			pollSubmission.VerifyAll();
			pollSubmissionResponses.VerifyAll();
			pollSubmissionResponse.VerifyAll();
		}

		[Fact]
		public void Respond()
		{
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();
			var response = EntityCreator.Create<Models.PollResponse>();

			var pollSubmission = new Mock<IPollSubmission>(MockBehavior.Strict);
			pollSubmission.Setup(_ => _.Save()).Returns(null);

			var pollSubmissionFactory = new Mock<IObjectFactory<IPollSubmission>>(MockBehavior.Strict);
			pollSubmissionFactory.Setup(_ => _.Create(It.IsAny<object>())).Returns(pollSubmission.Object);

			var auth = new Mock<IMyVoteAuthentication>(MockBehavior.Strict);
			auth.Setup(_ => _.GetCurrentUserID()).Returns(userId);

			var controller = new RespondController();
			controller.PollSubmissionFactory = pollSubmissionFactory.Object;
			controller.MyVoteAuthentication = auth.Object;

			controller.Put(response);

			pollSubmissionFactory.VerifyAll();
			pollSubmission.VerifyAll();
			auth.VerifyAll();
		}
	}
}
