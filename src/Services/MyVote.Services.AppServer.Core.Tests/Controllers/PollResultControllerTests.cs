using System;
using System.Collections.Generic;
using Moq;
using MyVote.Services.AppServer.Auth;
using MyVote.Services.AppServer.Controllers;
using MyVote.BusinessObjects;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core.Contracts;
using Spackle;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MyVote.Services.AppServer.Models;

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

			var pollDataResult = new Mock<IPollDataResult>(MockBehavior.Strict);
			pollDataResult.SetupGet(_ => _.OptionText).Returns(optionText);
			pollDataResult.SetupGet(_ => _.PollOptionID).Returns(pollOptionID);
			pollDataResult.SetupGet(_ => _.ResponseCount).Returns(responseCount);

			var results = new Mock<IReadOnlyListBaseCore<IPollDataResult>>(MockBehavior.Strict);
			results.Setup(_ => _.GetEnumerator()).Returns(new List<IPollDataResult> { pollDataResult.Object }.GetEnumerator());
			results.Setup(_ => _[0]).Returns(pollDataResult.Object);
			results.SetupGet(_ => _.Count).Returns(1);

			var pollDataResults = new Mock<IPollDataResults>(MockBehavior.Strict);
			pollDataResults.SetupGet(_ => _.Question).Returns(question);
			pollDataResults.SetupGet(_ => _.Results).Returns(results.Object);

			var childComment = this.BuildMockPollComment(generator, null);
			var parentComment = this.BuildMockPollComment(generator, childComment.Object);

			var parentCommentCollection = new Mock<IPollCommentCollection>(MockBehavior.Strict);
			parentCommentCollection.Setup(_ => _.GetEnumerator()).Returns(new List<IPollComment> { parentComment.Object }.GetEnumerator());
			parentCommentCollection.Setup(_ => _[0]).Returns(parentComment.Object);
			parentCommentCollection.SetupGet(_ => _.Count).Returns(1);

			var pollComments = new Mock<IPollComments>(MockBehavior.Strict);
			pollComments.SetupGet(_ => _.Comments).Returns(parentCommentCollection.Object);

			var pollResults = new Mock<IPollResults>(MockBehavior.Strict);
			pollResults.SetupGet(_ => _.PollID).Returns(pollID);
			pollResults.SetupGet(_ => _.IsPollOwnedByUser).Returns(true);
			pollResults.SetupGet(_ => _.IsActive).Returns(true);
			pollResults.SetupGet(_ => _.PollImageLink).Returns(imageLink);
			pollResults.SetupGet(_ => _.PollDataResults).Returns(pollDataResults.Object);
			pollResults.SetupGet(_ => _.PollComments).Returns(pollComments.Object);

			var pollResultsFactory = new Mock<IObjectFactory<IPollResults>>(MockBehavior.Strict);
			pollResultsFactory
				.Setup(_ => _.Fetch(It.Is<PollResultsCriteria>(
					criteria => criteria.PollID == pollID && criteria.UserID == userID)))
				.Returns(pollResults.Object);

			var user = new Mock<IUser>(MockBehavior.Strict);
			user.SetupGet(_ => _.UserID).Returns(userID);
			var userFactory = new Mock<IObjectFactory<IUser>>(MockBehavior.Strict);
			userFactory.Setup(_ => _.Fetch(userName)).Returns(user.Object);

			var myVoteAuth = new Mock<IMyVoteAuthentication>(MockBehavior.Strict);
			myVoteAuth.SetupGet(_ => _.CurrentPrincipal).Returns(new MyVotePrincipal(userName));
			myVoteAuth.Setup(_ => _.GetCurrentUserID()).Returns(userID);

			var controller = new PollResultController(
				myVoteAuth.Object, pollResultsFactory.Object);

			var pollResult = (controller.Get(pollID) as OkObjectResult).Value as PollResult;
			pollResult.PollID.Should().Be(pollID);
			pollResult.Question.Should().Be(question);
			pollResult.Results[0].OptionText.Should().Be(pollDataResult.Object.OptionText);
			pollResult.Comments[0].CommentText.Should().Be(parentComment.Object.CommentText);
			pollResult.Comments[0].Comments[0].CommentText.Should().Be(childComment.Object.CommentText);

			pollResultsFactory.VerifyAll();
			pollComments.VerifyAll();
			parentComment.VerifyAll();
			childComment.VerifyAll();
			pollDataResults.VerifyAll();
			pollDataResult.VerifyAll();
		}

		private Mock<IPollComment> BuildMockPollComment(RandomObjectGenerator generator, IPollComment childComment)
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

			var pollCommentCollection = new Mock<IPollCommentCollection>(MockBehavior.Strict);
			pollCommentCollection.Setup(_ => _.GetEnumerator()).Returns(pollCommentList.GetEnumerator());
			pollCommentCollection.Setup(_ => _[0]).Returns(childComment);
			pollCommentCollection.Setup(_ => _.Count).Returns(pollCommentList.Count);

			var pollComment = new Mock<IPollComment>(MockBehavior.Strict);
			pollComment.SetupGet(_ => _.PollCommentID).Returns(commentID);
			pollComment.SetupGet(_ => _.CommentDate).Returns(commentDate);
			pollComment.SetupGet(_ => _.Comments).Returns(pollCommentCollection.Object);
			pollComment.SetupGet(_ => _.CommentText).Returns(commentText);
			pollComment.SetupGet(_ => _.UserName).Returns(userName);

			return pollComment;
		}
	}
}
