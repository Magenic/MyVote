using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MyVote.BusinessObjects;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core.Contracts;
using MyVote.Services.AppServer.Auth;
using MyVote.Services.AppServer.Controllers;
using MyVote.Services.AppServer.Models;
using Rocks;
using Rocks.Options;
using Spackle;
using System;
using System.Collections.Generic;
using Xunit;

namespace MyVote.Services.AppServer.Tests
{
	public sealed class PollResultControllerTests
	{
		[Fact]
		public void GetResult()
		{
			var generator = new RandomObjectGenerator();
			var pollID = generator.Generate<int>();
			var question = generator.Generate<string>();
			var optionText = generator.Generate<string>();
			var pollOptionID = generator.Generate<int>();
			var responseCount = generator.Generate<int>();
			var userID = generator.Generate<int>();
			var userName = generator.Generate<string>();
			var imageLink = generator.Generate<Uri>().ToString();

			var pollDataResult = Rock.Create<IPollDataResult>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollDataResult.Handle(nameof(IPollDataResult.OptionText), () => optionText, 2);
			pollDataResult.Handle(nameof(IPollDataResult.PollOptionID), () => pollOptionID);
			pollDataResult.Handle(nameof(IPollDataResult.ResponseCount), () => responseCount);

			var results = Rock.Create<IReadOnlyListBaseCore<IPollDataResult>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			results.Handle(_ => _.GetEnumerator()).Returns(new List<IPollDataResult> { pollDataResult.Make() }.GetEnumerator());
			results.Handle(() => 0, _ => pollDataResult.Make());
			results.Handle(nameof(IReadOnlyListBaseCore<IPollDataResult>.Count), () => 1);

			var pollDataResults = Rock.Create<IPollDataResults>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollDataResults.Handle(nameof(IPollDataResults.Question), () => question);
			pollDataResults.Handle(nameof(IPollDataResults.Results), () => results.Make());

			var childComment = this.BuildMockPollComment(generator, null, 1);
			var parentComment = this.BuildMockPollComment(generator, childComment.Make(), 2);
			var parentCommentChunk = parentComment.Make();

			var parentCommentCollection = Rock.Create<IPollCommentCollection>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			parentCommentCollection.Handle(_ => _.GetEnumerator()).Returns(new List<IPollComment> { parentCommentChunk }.GetEnumerator());
			parentCommentCollection.Handle(() => 0, _ => parentCommentChunk);
			parentCommentCollection.Handle(nameof(IPollCommentCollection.Count), () => 1);

			var pollComments = Rock.Create<IPollComments>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollComments.Handle(nameof(IPollComments.Comments), () => parentCommentCollection.Make());

			var pollResults = Rock.Create<IPollResults>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollResults.Handle(nameof(IPollResults.PollID), () => pollID);
			pollResults.Handle(nameof(IPollResults.IsPollOwnedByUser), () => true);
			pollResults.Handle(nameof(IPollResults.IsActive), () => true);
			pollResults.Handle(nameof(IPollResults.PollImageLink), () => imageLink);
			pollResults.Handle(nameof(IPollResults.PollDataResults), () => pollDataResults.Make());
			pollResults.Handle(nameof(IPollResults.PollComments), () => pollComments.Make());

			var pollResultsFactory = Rock.Create<IObjectFactory<IPollResults>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollResultsFactory
				.Handle(_ => _.Fetch(Arg.IsAny<PollResultsCriteria>()))
				.Returns(pollResults.Make());

			var user = Rock.Create<IUser>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			user.Handle(nameof(IUser.UserID), () => userID as int?);
			var userFactory = Rock.Create<IObjectFactory<IUser>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			userFactory.Handle(_ => _.Fetch(userName)).Returns(user.Make());

			var myVoteAuth = Rock.Create<IMyVoteAuthentication>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			myVoteAuth.Handle(nameof(IMyVoteAuthentication.CurrentPrincipal), () => new MyVotePrincipal(userName));
			myVoteAuth.Handle(_ => _.GetCurrentUserID()).Returns(userID);

			var controller = new PollResultController(
				myVoteAuth.Make(), pollResultsFactory.Make());

			var pollResult = (controller.Get(pollID) as OkObjectResult).Value as PollResult;
			pollResult.PollID.Should().Be(pollID);
			pollResult.Question.Should().Be(question);
			pollResult.Results[0].OptionText.Should().Be(pollDataResult.Make().OptionText);
			pollResult.Comments[0].CommentText.Should().Be(parentComment.Make().CommentText);
			pollResult.Comments[0].Comments[0].CommentText.Should().Be(childComment.Make().CommentText);

			pollResultsFactory.Verify();
			pollComments.Verify();
			parentComment.Verify();
			childComment.Verify();
			pollDataResults.Verify();
			pollDataResult.Verify();
		}

		private IRock<IPollComment> BuildMockPollComment(RandomObjectGenerator generator, IPollComment childComment, uint expectationCount)
		{
			var commentID = generator.Generate<int>();
			var commentDate = generator.Generate<DateTime>();
			var commentText = generator.Generate<string>();
			var userName = generator.Generate<string>();

			var pollCommentList = new List<IPollComment>();

			if (childComment != null)
			{
				pollCommentList.Add(childComment);
			}

			var pollCommentCollection = Rock.Create<IPollCommentCollection>();
			pollCommentCollection.Handle(_ => _.GetEnumerator()).Returns(pollCommentList.GetEnumerator());
			pollCommentCollection.Handle(() => 0, _ => childComment);
			pollCommentCollection.Handle(nameof(IPollCommentCollection.Count), () => pollCommentList.Count);

			var pollComment = Rock.Create<IPollComment>();
			pollComment.Handle(nameof(IPollComment.PollCommentID), () => commentID as int?, expectationCount);
			pollComment.Handle(nameof(IPollComment.CommentDate), () => commentDate as DateTime?);
			pollComment.Handle(nameof(IPollComment.Comments), () => pollCommentCollection.Make());
			pollComment.Handle(nameof(IPollComment.CommentText), () => commentText, 2);
			pollComment.Handle(nameof(IPollComment.UserName), () => userName);

			return pollComment;
		}
	}
}
