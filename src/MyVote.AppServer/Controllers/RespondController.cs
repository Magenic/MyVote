using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Csla.Rules;
using Microsoft.Ajax.Utilities;
using MyVote.AppServer.Auth;
using MyVote.AppServer.Models;
using MyVote.BusinessObjects;
using MyVote.BusinessObjects.Contracts;

namespace MyVote.AppServer.Controllers
{
	public class RespondController : ApiController
	{
		public IObjectFactory<IPollSubmission> PollSubmissionFactory { get; set; }
		public IMyVoteAuthentication MyVoteAuthentication { get; set; }

		// GET api/respond
		[Authorize]
		public PollInfo Get(int pollID, int userID)
		{
			try
			{
				var authUserID = MyVoteAuthentication.GetCurrentUserID().Value;
				var criteria = new PollSubmissionCriteria(pollID, authUserID);
				var submission = this.PollSubmissionFactory.Fetch(criteria);
				var result = new PollInfo
				{
					PollID = submission.PollID,
					PollDescription = submission.PollDescription,
					MaxAnswers = submission.PollMaxAnswers,
					MinAnswers = submission.PollMinAnswers,
					Comment = submission.Comment,
					PollQuestion = submission.PollQuestion,
					PollSubmissionID = submission.PollSubmissionID,
					SubmissionDate = submission.SubmissionDate,
					UserID = submission.UserID,
					PollOptions = submission.Responses.Select(_ => new Models.PollResponseOption
					{
						IsOptionSelected = _.IsOptionSelected,
						OptionPosition = _.OptionPosition,
						OptionText = _.OptionText,
						PollOptionID = _.PollOptionID,
						PollResponseID = _.PollResponseID
					}).ToList()
				};
				return result;
			}
			catch (Exception ex)
			{
				throw ex.ToHttpResponseException(Request);
			}
		}

		// PUT api/respond
		[Authorize]
		public void Put([FromBody]Models.PollResponse value)
		{
			IPollSubmission submission = null;

			try
			{
			    var authUserID = MyVoteAuthentication.GetCurrentUserID().Value;
				var criteria = new PollSubmissionCriteria(value.PollID, authUserID);
				submission = this.PollSubmissionFactory.Create(criteria);

				foreach (var item in value.ResponseItems)
				{
					var response = submission.Responses.Where(_ => _.PollOptionID == item.PollOptionID).Single();
					response.IsOptionSelected = item.IsOptionSelected;
				}

				submission.Save();
			}
			catch (ValidationException ex)
			{
				string brokenRules = string.Empty;
				if (submission != null)
				{
					brokenRules = submission.GetBrokenRules().ToString();
				}

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
				throw ex.ToHttpResponseException(Request);
			}
		}
	}
}
