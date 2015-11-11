using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Csla.Data;
using MyVote.Services.AppServer.Auth;
using MyVote.Services.AppServer.Models;
using MyVote.BusinessObjects;
using MyVote.BusinessObjects.Contracts;

namespace MyVote.Services.AppServer.Controllers
{
	public class PollCommentController : ApiController
	{
		public IObjectFactory<IPollResults> PollResultsFactory { get; set; }
		public IObjectFactory<IPollComment> PollCommentFactory { get; set; }
		public IMyVoteAuthentication MyVoteAuthentication { get; set; }

		[Authorize]
		public HttpResponseMessage Put([FromBody]PollResultComment input)
		{
			IPollComment comment = null;
			
			try
			{
				var userID = MyVoteAuthentication.GetCurrentUserID();
				var pollResults = PollResultsFactory.Fetch(new PollResultsCriteria(userID.Value, input.PollID));

				comment = PollCommentFactory.CreateChild(input.UserID, input.UserName);
				comment.CommentText = input.CommentText;

				if (input.ParentCommentID.HasValue)
				{
					var parentComment = pollResults.PollComments.Comments.Single(c => c.PollCommentID == input.ParentCommentID);
					parentComment.Comments.Add(comment);
				}
				else
				{
					pollResults.PollComments.Comments.Add(comment);
				}

				var newResults = pollResults.Save() as IPollResults;
				var targetCollection = input.ParentCommentID.HasValue
					? newResults.PollComments.Comments.Single(c => c.PollCommentID == input.ParentCommentID).Comments
					: newResults.PollComments.Comments;

				return this.Request.CreateResponse(
					HttpStatusCode.OK,
					new {PollCommentID = targetCollection.Max(c => c.PollCommentID).Value});
			}
			catch (Csla.Rules.ValidationException ex)
			{
				var brokenRules = comment.GetBrokenRules().ToString();
				throw new HttpResponseException(
				  new HttpResponseMessage
				  {
					  StatusCode = HttpStatusCode.BadRequest,
					  ReasonPhrase = ex.Message.Replace(Environment.NewLine, " "),
					  Content = new StringContent(brokenRules),
					  RequestMessage = Request
				  });
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
	}
}