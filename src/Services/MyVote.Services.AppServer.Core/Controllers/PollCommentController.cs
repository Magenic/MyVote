using System;
using System.Linq;
using MyVote.Services.AppServer.Models;
using MyVote.BusinessObjects;
using MyVote.BusinessObjects.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using MyVote.Services.AppServer.Auth;
using static MyVote.Services.AppServer.Extensions.IBusinessBaseExtensions;

namespace MyVote.Services.AppServer.Controllers
{
	[Route("api/[controller]")]
	public sealed class PollCommentController :
		Controller
	{
		private readonly IObjectFactory<IPollResults> pollResultsFactory;
		private readonly IObjectFactory<IPollComment> pollCommentFactory;
		private readonly IMyVoteAuthentication authentication;

		public PollCommentController(IObjectFactory<IPollResults> pollResultsFactory,
			IObjectFactory<IPollComment> pollCommentFactory,
			IMyVoteAuthentication authentication)
		{
			if (pollResultsFactory == null)
			{
				throw new ArgumentNullException(nameof(pollResultsFactory));
			}

			if (pollCommentFactory == null)
			{
				throw new ArgumentNullException(nameof(pollCommentFactory));
			}

			if (authentication == null)
			{
				throw new ArgumentNullException(nameof(authentication));
			}

			this.pollResultsFactory = pollResultsFactory;
			this.pollCommentFactory = pollCommentFactory;
			this.authentication = authentication;
		}

		[Authorize]
		[HttpPut]
		public async Task<IActionResult> Put([FromBody]PollResultComment input)
		{
			var userID = this.authentication.GetCurrentUserID();
			var pollResults = this.pollResultsFactory.Fetch(new PollResultsCriteria(userID.Value, input.PollID));

			var comment = this.pollCommentFactory.CreateChild(input.UserID, input.UserName);
			comment.CommentText = input.CommentText;

			if (input.ParentCommentID.HasValue)
			{
				var parentComment = pollResults.PollComments.Comments.Single(
					c => c.PollCommentID == input.ParentCommentID);
				parentComment.Comments.Add(comment);
			}
			else
			{
				pollResults.PollComments.Comments.Add(comment);
			}

			return await pollResults.PersistAsync();
		}
	}
}