using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyVote.Services.AppServer.Auth;
using MyVote.Services.AppServer.Controllers;
using MyVote.BusinessObjects.Contracts;
using Spackle;

namespace MyVote.Services.AppServer.Tests
{
	[TestClass]
	public sealed class RespondControllerTests
	{
		[TestMethod]
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
			Assert.AreEqual(pollId, result.PollID);
			Assert.AreEqual(pollDescription, result.PollDescription);
			Assert.AreEqual(pollMaxAnswers, result.MaxAnswers);
			Assert.AreEqual(pollMinAnswers, result.MinAnswers);
			Assert.AreEqual(comment, result.Comment);
			Assert.AreEqual(pollQuestion, result.PollQuestion);
			Assert.AreEqual(pollSubmissionId, result.PollSubmissionID);
			Assert.AreEqual(submissionDate, result.SubmissionDate);
			Assert.AreEqual(userId, result.UserID);
			Assert.AreEqual(1, result.PollOptions.Count);
			var option = result.PollOptions[0];
			Assert.AreEqual(isOptionSelected, option.IsOptionSelected);
			Assert.AreEqual(optionPosition, option.OptionPosition);
			Assert.AreEqual(optionText, option.OptionText);
			Assert.AreEqual(pollOptionId, option.PollOptionID);
			Assert.AreEqual(pollResponseId, option.PollResponseID);

			pollSubmissionFactory.VerifyAll();
			pollSubmission.VerifyAll();
			pollSubmissionResponses.VerifyAll();
			pollSubmissionResponse.VerifyAll();
		}

		[TestMethod]
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
