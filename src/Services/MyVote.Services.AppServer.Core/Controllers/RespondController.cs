using System;
using System.Linq;
using MyVote.Services.AppServer.Auth;
using MyVote.Services.AppServer.Models;
using MyVote.BusinessObjects;
using MyVote.BusinessObjects.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using static MyVote.Services.AppServer.Extensions.IBusinessBaseExtensions;

namespace MyVote.Services.AppServer.Controllers
{
	[Route("api/[controller]")]
	public sealed class RespondController
		: Controller
	{
		public IObjectFactory<IPollSubmission> pollSubmissionFactory { get; set; }
		public IMyVoteAuthentication authentication { get; set; }

		public RespondController(IObjectFactory<IPollSubmission> pollSubmissionFactory,
			IMyVoteAuthentication authentication)
		{
			if (pollSubmissionFactory == null)
			{
				throw new ArgumentNullException(nameof(pollSubmissionFactory));
			}

			if (authentication == null)
			{
				throw new ArgumentNullException(nameof(authentication));
			}

			this.pollSubmissionFactory = pollSubmissionFactory;
			this.authentication = authentication;
		}

		// GET api/respond
		[Authorize]
		[HttpGet("{pollID}/{userID}")]
		public IActionResult Get(int pollID, int userID)
		{
			var authUserID = this.authentication.GetCurrentUserID().Value;
			var criteria = new PollSubmissionCriteria(pollID, authUserID);
			var submission = this.pollSubmissionFactory.Fetch(criteria);
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
				PollOptions = submission.Responses.Select(_ => new PollResponseOption
				{
					IsOptionSelected = _.IsOptionSelected,
					OptionPosition = _.OptionPosition,
					OptionText = _.OptionText,
					PollOptionID = _.PollOptionID,
					PollResponseID = _.PollResponseID
				}).ToList()
			};

			return new OkObjectResult(result);
		}

		// PUT api/respond
		[Authorize]
		public async Task<IActionResult> Put([FromBody]PollResponse value)
		{
			var authUserID = authentication.GetCurrentUserID().Value;
			var criteria = new PollSubmissionCriteria(value.PollID, authUserID);
			var submission = this.pollSubmissionFactory.Create(criteria);

			foreach (var item in value.ResponseItems)
			{
				var response = submission.Responses.Where(_ => _.PollOptionID == item.PollOptionID).Single();
				response.IsOptionSelected = item.IsOptionSelected;
			}

			return await submission.PersistAsync();
		}
	}
}
