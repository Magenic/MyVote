using System;
using System.Collections.Generic;
using Moq;
using MyVote.Services.AppServer.Auth;
using MyVote.Services.AppServer.Controllers;
using MyVote.BusinessObjects.Contracts;
using Spackle;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MyVote.Services.AppServer.Models;
using System.Threading.Tasks;

namespace MyVote.Services.AppServer.Tests.Controllers
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
			pollSubmissionResponses.SetupGet(_ => _.Count).Returns(1);
			pollSubmissionResponses.Setup(_ => _[0]).Returns(pollSubmissionResponse.Object);

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

			var controller = new RespondController(
				pollSubmissionFactory.Object, auth.Object);

			var result = (controller.Get(pollId, userId) as OkObjectResult).Value as PollInfo;
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
		public async Task Respond()
		{
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();
			var response = EntityCreator.Create<PollResponse>();

			var pollSubmission = new Mock<IPollSubmission>(MockBehavior.Strict);
			pollSubmission.SetupGet(_ => _.IsSavable).Returns(true);
			pollSubmission.Setup(_ => _.SaveAsync())
				.Returns(Task.FromResult<object>(pollSubmission.Object));

			var pollSubmissionFactory = new Mock<IObjectFactory<IPollSubmission>>(MockBehavior.Strict);
			pollSubmissionFactory.Setup(_ => _.Create(It.IsAny<object>())).Returns(pollSubmission.Object);

			var auth = new Mock<IMyVoteAuthentication>(MockBehavior.Strict);
			auth.Setup(_ => _.GetCurrentUserID()).Returns(userId);

			var controller = new RespondController(
				pollSubmissionFactory.Object, auth.Object);

			var result = await controller.Put(response);
			result.Should().BeOfType<NoContentResult>();

			pollSubmissionFactory.VerifyAll();
			pollSubmission.VerifyAll();
			auth.VerifyAll();
		}
	}
}
