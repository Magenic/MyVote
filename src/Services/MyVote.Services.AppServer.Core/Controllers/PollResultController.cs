using System;
using System.Linq;
using MyVote.Services.AppServer.Auth;
using MyVote.Services.AppServer.Models;
using MyVote.BusinessObjects;
using MyVote.BusinessObjects.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace MyVote.Services.AppServer.Controllers
{
	[Route("api/[controller]")]
	public sealed class PollResultController
		: Controller
	{
		private readonly IMyVoteAuthentication authentication;
		private readonly IObjectFactory<IPollResults> pollDataResultsFactory;

		public PollResultController(IMyVoteAuthentication authentication,
			IObjectFactory<IPollResults> pollDataResultsFactory)
		{
			if (authentication == null)
			{
				throw new ArgumentNullException(nameof(authentication));
			}

			if (pollDataResultsFactory == null)
			{
				throw new ArgumentNullException(nameof(pollDataResultsFactory));
			}

			this.authentication = authentication;
			this.pollDataResultsFactory = pollDataResultsFactory;
		}

		// GET api/pollresult?pollId=5
		[HttpGet("{pollId}")]
		public IActionResult Get(int pollId)
		{
			var userID = this.authentication.GetCurrentUserID();
			var data = this.pollDataResultsFactory.Fetch(new PollResultsCriteria(userID, pollId));
			var result = new PollResult
			{
				PollID = data.PollID,
				IsPollOwnedByUser = data.IsPollOwnedByUser,
				IsActive = data.IsActive,
				PollImageLink = data.PollImageLink,
				Question = data.PollDataResults.Question,
				Results = data.PollDataResults.Results.Select(_ => new PollResultItem
				{
					OptionText = _.OptionText,
					PollOptionID = _.PollOptionID,
					ResponseCount = _.ResponseCount
				}).ToList(),
				Comments = data.PollComments.Comments.Select(
					_ => this.MapPollComments(data.PollID, _, null)).ToList()
			};

			return new OkObjectResult(result);
		}

		private PollResultComment MapPollComments(int pollID, IPollComment pollComment, int? parentCommentID)
		{
			return new PollResultComment
			{
				PollID = pollID,
				ParentCommentID = parentCommentID,
				PollCommentID = pollComment.PollCommentID.Value,
				UserName = pollComment.UserName,
				CommentDate = pollComment.CommentDate,
				CommentText = pollComment.CommentText,
				Comments = pollComment.Comments.Select(c => this.MapPollComments(pollID, c, pollComment.PollCommentID)).ToList()
			};
		}
	}
}
