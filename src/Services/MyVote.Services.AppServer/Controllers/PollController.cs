using System;
using System.Collections.Generic;
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
	public sealed class PollController
		: Controller
	{
		private readonly Lazy<IObjectFactory<IPoll>> pollFactory;
		private readonly Lazy<IObjectFactory<IPollOption>> pollOptionFactory;
		private readonly Lazy<IObjectFactory<IPollSearchResults>> pollSearchResultsFactory;
		private readonly IMyVoteAuthentication authentication;

		public PollController(Lazy<IObjectFactory<IPoll>> pollFactory,
			Lazy<IObjectFactory<IPollOption>> pollOptionFactory,
			Lazy<IObjectFactory<IPollSearchResults>> pollSearchResultsFactory,
			IMyVoteAuthentication authentication)
		{
			if (pollFactory == null)
			{
				throw new ArgumentNullException(nameof(pollFactory));
			}

			if (pollOptionFactory == null)
			{
				throw new ArgumentNullException(nameof(pollOptionFactory));
			}

			if (pollSearchResultsFactory == null)
			{
				throw new ArgumentNullException(nameof(pollSearchResultsFactory));
			}

			if (authentication == null)
			{
				throw new ArgumentNullException(nameof(authentication));
			}

			this.pollFactory = pollFactory;
			this.pollOptionFactory = pollOptionFactory;
			this.pollSearchResultsFactory = pollSearchResultsFactory;
			this.authentication = authentication;
		}

		// GET api/poll
		/// <summary>
		/// Gets a list of the most popular polls
		/// </summary>
		/// <returns>List of poll summaries</returns>
		[Authorize]
		[HttpGet]
		public IActionResult Get()
		{
			return this.Get(PollSearchResultsQueryType.MostPopular.ToString());
		}

		// GET api/poll?filterBy=xyz
		/// <summary>
		/// Gets a list of polls
		/// </summary>
		/// <param name="filterBy">Newest, MostPopular, Reported</param>
		/// <returns>List of poll summaries</returns>
		[Authorize]
		[HttpGet("filter/{filterBy}")]
		public IActionResult Get(string filterBy)
		{
			var queryType = (PollSearchResultsQueryType)Enum.Parse(typeof(PollSearchResultsQueryType), filterBy, true);
			var data = this.pollSearchResultsFactory.Value.Fetch(queryType);
			var result = new List<PollSummary>();

			foreach (var item in data.SearchResultsByCategory)
			{
				foreach (var info in item.SearchResults)
				{
					result.Add(new PollSummary
					{
						Category = item.Category,
						Id = info.Id,
						ImageLink = info.ImageLink,  //ImageLink uses Azure Storage REST API to retrieve the image on the client
						Question = info.Question,
						SubmissionCount = info.SubmissionCount
					});
				}
			}

			return new OkObjectResult(result);
		}

		// GET api/poll/5
		[HttpGet("{id}")]
		public IActionResult Get(int id)
		{
			return new OkObjectResult(PollController.MapPollToModel(
				this.pollFactory.Value.Fetch(id)));
		}

		// PUT api/poll
		[Authorize]
		[HttpPut]
		public async Task<IActionResult> Put([FromBody]Poll input)
		{
            var userID = authentication.GetCurrentUserID();
            var poll = this.pollFactory.Value.Create(userID.Value);
            return await this.BuildPoll(input, poll).PersistAsync<IPoll>(
                (newPoll) =>
                {
                    return Get((int)newPoll.PollID);
                    //return this.CreatedAtRoute("GetById",
                    //    new { id = newPoll.PollID }, input);

                });
        }

		// PUT api/poll/5
		[Authorize]
		[HttpPut("{id}")]
		public async Task<IActionResult> Put(int id, [FromBody]Poll input)
		{
			var poll = this.pollFactory.Value.Fetch(id);
			return await this.BuildPoll(input, poll).PersistAsync();
		}

		// DELETE api/poll/5
		[Authorize]
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var poll = this.pollFactory.Value.Fetch(id);
			var userID = authentication.GetCurrentUserID();

			if (poll.UserID != userID.Value)
			{
				return new UnauthorizedResult();
			}

			poll.PollDeletedDate = DateTime.UtcNow;
			poll.PollDeletedFlag = true;
			return await poll.PersistAsync();
		}

		private static Poll MapPollToModel(IPoll poll)
		{
			var result = new Poll();

			result.PollID = poll.PollID.Value;
			result.UserID = poll.UserID;
			result.PollCategoryID = poll.PollCategoryID.Value;
			result.PollQuestion = poll.PollQuestion;
			result.PollImageLink = poll.PollImageLink;
			result.PollMaxAnswers = poll.PollMaxAnswers.Value;
			result.PollMinAnswers = poll.PollMinAnswers.Value;
			result.PollStartDate = poll.PollStartDate;
			result.PollEndDate = poll.PollEndDate;
			result.PollAdminRemovedFlag = poll.PollAdminRemovedFlag.GetValueOrDefault(false);
			result.PollDateRemoved = poll.PollDateRemoved;
			result.PollDeletedFlag = poll.PollDeletedFlag.GetValueOrDefault(false);
			result.PollDeletedDate = poll.PollDeletedDate;
			result.PollDescription = poll.PollDescription;

			result.PollOptions = poll.PollOptions.Select(_ => new PollOption
			{
				PollOptionID = _.PollOptionID,
				PollID = _.PollID,
				OptionPosition = _.OptionPosition,
				OptionText = _.OptionText
			}).ToList();

			return result;
		}

		private IPoll BuildPoll(Poll input, IPoll poll)
		{
			poll.PollCategoryID = input.PollCategoryID;
			poll.PollQuestion = input.PollQuestion;
			poll.PollImageLink = input.PollImageLink;
			poll.PollMaxAnswers = input.PollMaxAnswers;
			poll.PollMinAnswers = input.PollMinAnswers;
			poll.PollStartDate = input.PollStartDate;
			poll.PollEndDate = input.PollEndDate;
			poll.PollAdminRemovedFlag = input.PollAdminRemovedFlag;
			poll.PollDateRemoved = input.PollDateRemoved;
			poll.PollDeletedFlag = input.PollDeletedFlag;
			poll.PollDeletedDate = input.PollDeletedDate;
			poll.PollDescription = input.PollDescription;

			// remove items from the real poll if they aren't in the input
			if (input.PollOptions == null)
			{
				poll.PollOptions.Clear();
			}
			else
			{
				var toRemove = new List<IPollOption>();
				toRemove.AddRange(
					poll.PollOptions.Where(item => input.PollOptions.All(_ => _.PollOptionID != item.PollOptionID)));
				foreach (var item in toRemove)
				{
					poll.PollOptions.Remove(item);
				}
			}

			// add or update according to new options list
			if (input.PollOptions != null && input.PollOptions.Count > 0)
			{
				var toAdd = new List<IPollOption>();

				foreach (var item in input.PollOptions)
				{
					var existing = poll.PollOptions.FirstOrDefault(_ => _.PollOptionID == item.PollOptionID);
					if (existing == null)
					{
						var newOption = this.pollOptionFactory.Value.CreateChild();
						newOption.OptionPosition = item.OptionPosition;
						newOption.OptionText = item.OptionText;
						toAdd.Add(newOption);
					}
					else
					{
						// updating existing item
						existing.OptionPosition = item.OptionPosition;
						existing.OptionText = item.OptionText;
					}
				}

				poll.PollOptions.AddRange(toAdd);
			}

			return poll;
		}
	}
}