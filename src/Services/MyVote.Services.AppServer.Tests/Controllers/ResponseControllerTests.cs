using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MyVote.BusinessObjects.Contracts;
using MyVote.Services.AppServer.Auth;
using MyVote.Services.AppServer.Controllers;
using MyVote.Services.AppServer.Models;
using Rocks;
using Rocks.Options;
using Spackle;
using System;
using System.Threading.Tasks;
using Xunit;

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

			var pollSubmissionResponse = Rock.Create<IPollSubmissionResponse>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollSubmissionResponse.Handle(nameof(IPollSubmissionResponse.IsOptionSelected), () => isOptionSelected);
			pollSubmissionResponse.Handle(nameof(IPollSubmissionResponse.OptionPosition), () => optionPosition);
			pollSubmissionResponse.Handle(nameof(IPollSubmissionResponse.OptionText), () => optionText);
			pollSubmissionResponse.Handle(nameof(IPollSubmissionResponse.PollOptionID), () => pollOptionId);
			pollSubmissionResponse.Handle(nameof(IPollSubmissionResponse.PollResponseID), () => pollResponseId as int?);

			var pollSubmissionResponses = Rock.Create<IPollSubmissionResponseCollection>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollSubmissionResponses.Handle(nameof(IPollSubmissionResponseCollection.Count), () => 1);
			pollSubmissionResponses.Handle(() => 0, _ => pollSubmissionResponse.Make());

			var pollSubmission = Rock.Create<IPollSubmission>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollSubmission.Handle(nameof(IPollSubmission.PollID), () => pollId);
			pollSubmission.Handle(nameof(IPollSubmission.PollDescription), () => pollDescription);
			pollSubmission.Handle(nameof(IPollSubmission.PollMaxAnswers), () => pollMaxAnswers);
			pollSubmission.Handle(nameof(IPollSubmission.PollMinAnswers), () => pollMinAnswers);
			pollSubmission.Handle(nameof(IPollSubmission.Comment), () => comment);
			pollSubmission.Handle(nameof(IPollSubmission.PollQuestion), () => pollQuestion);
			pollSubmission.Handle(nameof(IPollSubmission.PollSubmissionID), () => pollSubmissionId as int?);
			pollSubmission.Handle(nameof(IPollSubmission.SubmissionDate), () => submissionDate as DateTime?);
			pollSubmission.Handle(nameof(IPollSubmission.UserID), () => userId);
			pollSubmission.Handle(nameof(IPollSubmission.Responses), () => pollSubmissionResponses.Make());

			var pollSubmissionFactory = Rock.Create<IObjectFactory<IPollSubmission>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollSubmissionFactory.Handle(_ => _.Fetch(Arg.IsAny<object>())).Returns(pollSubmission.Make());

			var auth = Rock.Create<IMyVoteAuthentication>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			auth.Handle(_ => _.GetCurrentUserID()).Returns(userId);

			var controller = new RespondController(
				pollSubmissionFactory.Make(), auth.Make());

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

			pollSubmissionFactory.Verify();
			pollSubmission.Verify();
			pollSubmissionResponses.Verify();
			pollSubmissionResponse.Verify();
		}

		[Fact]
		public async Task Respond()
		{
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();
			var response = EntityCreator.Create<PollResponse>();

			var pollSubmission = Rock.Create<IPollSubmission>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollSubmission.Handle(nameof(IPollSubmission.IsSavable), () => true);
			pollSubmission.Handle(_ => _.SaveAsync())
				.Returns(Task.FromResult<object>(pollSubmission.Make()));

			var pollSubmissionFactory = Rock.Create<IObjectFactory<IPollSubmission>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollSubmissionFactory.Handle(_ => _.Create(Arg.IsAny<object>())).Returns(pollSubmission.Make());

			var auth = Rock.Create<IMyVoteAuthentication>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			auth.Handle(_ => _.GetCurrentUserID()).Returns(userId);

			var controller = new RespondController(
				pollSubmissionFactory.Make(), auth.Make());

			var result = await controller.Put(response);
			result.Should().BeOfType<NoContentResult>();

			pollSubmissionFactory.Verify();
			pollSubmission.Verify();
			auth.Verify();
		}
	}
}
