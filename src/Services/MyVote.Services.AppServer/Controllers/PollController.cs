using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Csla.Rules;
using MyVote.Services.AppServer.Auth;
using MyVote.Services.AppServer.Models;
using MyVote.BusinessObjects;
using MyVote.BusinessObjects.Contracts;

namespace MyVote.Services.AppServer.Controllers
{
	public class PollController : ApiController
	{
		public Lazy<IObjectFactory<IPoll>> PollFactory { get; set; }
		public Lazy<IObjectFactory<IPollOption>> PollOptionFactory { get; set; }
		public Lazy<IObjectFactory<IPollSearchResults>> PollSearchResultsFactory { get; set; }
		public IMyVoteAuthentication MyVoteAuthentication { get; set; }

		// GET api/poll
		/// <summary>
		/// Gets a list of the most popular polls
		/// </summary>
		/// <returns>List of poll summaries</returns>
		[Authorize]
		public IEnumerable<PollSummary> Get()
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
		public IEnumerable<PollSummary> Get(string filterBy)
		{
			try
			{
				var queryType = (PollSearchResultsQueryType)Enum.Parse(typeof(PollSearchResultsQueryType), filterBy, true);
				var data = this.PollSearchResultsFactory.Value.Fetch(queryType);
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

		// GET api/poll/5
		public Poll Get(int id)
		{
			try
			{
				var poll = this.PollFactory.Value.Fetch(id);
				return PollController.MapPollToModel(poll);
			}
			catch (NullReferenceException ex)
			{
				throw new HttpResponseException(
				  new HttpResponseMessage
				  {
					  StatusCode = HttpStatusCode.NotFound,
					  ReasonPhrase = string.Format("No resource matching {0} found", id),
					  Content = new StringContent(ex.ToString()),
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

		// PUT api/poll
		[Authorize]
		public Poll Put([FromBody]Poll input)
		{
			IPoll poll = null;

			try
			{
				var userID = MyVoteAuthentication.GetCurrentUserID();
				poll = this.PollFactory.Value.Create(userID.Value);
				var newPoll = this.SavePoll(input, poll);
				return PollController.MapPollToModel(newPoll);
			}
			catch (ValidationException ex)
			{
				var brokenRules = poll.GetBrokenRules().ToString();
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

		// PUT api/poll/5
		[Authorize]
		public Poll Put(int id, [FromBody]Poll input)
		{
			IPoll poll = null;

			try
			{
				poll = this.PollFactory.Value.Fetch(id);
				var updatedPoll = this.SavePoll(input, poll);
				return PollController.MapPollToModel(updatedPoll);
			}
			catch (ValidationException ex)
			{
				var brokenRules = poll.GetBrokenRules().ToString();
				throw new HttpResponseException(
				  new HttpResponseMessage
				  {
					  StatusCode = HttpStatusCode.BadRequest,
					  ReasonPhrase = ex.Message.Replace(Environment.NewLine, " "),
					  Content = new StringContent(brokenRules),
					  RequestMessage = Request
				  });
			}
			catch (NullReferenceException ex)
			{
				throw new HttpResponseException(
				  new HttpResponseMessage
				  {
					  StatusCode = HttpStatusCode.NotFound,
					  ReasonPhrase = string.Format("No resource matching {0} found", id),
					  Content = new StringContent(ex.ToString()),
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

		private IPoll SavePoll(Poll input, IPoll poll)
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
						var newOption = this.PollOptionFactory.Value.CreateChild();
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

			return poll.Save() as IPoll;
		}

		// DELETE api/poll/5
		[Authorize]
		public void Delete(int id)
		{
			try
			{
				var poll = this.PollFactory.Value.Fetch(id);
				var userID = MyVoteAuthentication.GetCurrentUserID();
				if (poll.UserID != userID.Value)
				{
					throw new HttpResponseException(new HttpResponseMessage
					{
						StatusCode = HttpStatusCode.Unauthorized,
						ReasonPhrase = "Only the user who created the poll can delete it.",
						RequestMessage = Request
					});
				}
				poll.PollDeletedDate = DateTime.Today;
				poll.PollDeletedFlag = true;
				poll.Save();
			}
			catch (HttpResponseException)
			{
				throw;
			}
			catch (NullReferenceException ex)
			{
				throw new HttpResponseException(
				  new HttpResponseMessage
				  {
					  StatusCode = HttpStatusCode.NotFound,
					  ReasonPhrase = string.Format("No resource matching {0} found", id),
					  Content = new StringContent(ex.ToString()),
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
	}
}
