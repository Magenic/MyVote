using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MyVote.Services.AppServer.Auth;
using MyVote.Services.AppServer.Models;
using MyVote.BusinessObjects;
using MyVote.BusinessObjects.Contracts;

namespace MyVote.Services.AppServer.Controllers
{
	public class PollResultController : ApiController
	{
		public IMyVoteAuthentication MyVoteAuthentication { get; set; }
		public IObjectFactory<IPollResults> PollDataResultsFactory { get; set; }

		// GET api/pollresult?pollId=5
		public PollResult Get(int pollId)
		{
			try
			{
				var userID = MyVoteAuthentication.GetCurrentUserID();
				var data = PollDataResultsFactory.Fetch(new PollResultsCriteria(userID, pollId));
				var result = new PollResult
				{
					PollID = data.PollID,
					IsPollOwnedByUser = data.IsPollOwnedByUser,
					IsActive = data.IsActive,
					PollImageLink = data.PollImageLink,
					Question = data.PollDataResults.Question,
					Results = data.PollDataResults.Results.Select(pdr => new PollResultItem
					{
						OptionText = pdr.OptionText,
						PollOptionID = pdr.PollOptionID,
						ResponseCount = pdr.ResponseCount
					}).ToList(),
					Comments = data.PollComments.Comments.Select(c => MapPollComments(data.PollID, c, null)).ToList()
				};
				return result;
			}
			catch (Exception ex)
			{
				throw new HttpResponseException(
				  new HttpResponseMessage
				  {
					  StatusCode = HttpStatusCode.BadRequest,
					  ReasonPhrase = ex.Message.Replace(Environment.NewLine, " "),
					  Content = new StringContent(ex.ToString()),
					  RequestMessage = Request
				  });
			}
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
				Comments = pollComment.Comments.Select(c => MapPollComments(pollID, c, pollComment.PollCommentID)).ToList()
			};
		}
	}
}
