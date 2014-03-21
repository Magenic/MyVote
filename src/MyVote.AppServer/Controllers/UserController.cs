using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Csla.Data;
using Csla.Rules;
using MyVote.AppServer.Auth;
using MyVote.AppServer.Models;
using MyVote.BusinessObjects.Contracts;

namespace MyVote.AppServer.Controllers
{
	public class UserController : ApiController
	{
		public IObjectFactory<IUser> UserFactory { get; set; }
		public IMyVoteAuthentication MyVoteAuthentication { get; set; }

		// GET api/user/5
		[Authorize]
		public User Get(string userProfileId)
		{
			try
			{
				var user = this.UserFactory.Fetch(userProfileId);
				var authUserID = MyVoteAuthentication.GetCurrentUserID().Value;
				if (user.UserID != authUserID)
				{
					throw new HttpResponseException(new HttpResponseMessage
					{
						StatusCode = HttpStatusCode.Unauthorized,
						ReasonPhrase = "Only the authorized user's profile may be retrieved.",
						RequestMessage = Request
					});
				}
				return new User
				{
					UserID = user.UserID,
					ProfileID = user.ProfileID,
					EmailAddress = user.EmailAddress,
					FirstName = user.FirstName,
					LastName = user.LastName,
					Gender = user.Gender,
					BirthDate = user.BirthDate,
					PostalCode = user.PostalCode,
					UserName = user.UserName
				};
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
					  ReasonPhrase = string.Format("No resource matching {0} found", userProfileId),
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
					  ReasonPhrase = ex.Message,
					  Content = new StringContent(ex.ToString()),
					  RequestMessage = Request
				  });
			}
		}

		// PUT api/user
		[Authorize]
		public void Put([FromBody]User value)
		{
			IUser user = null;

			try
			{
				user = this.UserFactory.Create(value.ProfileID);

				user.EmailAddress = value.EmailAddress;
				user.FirstName = value.FirstName;
				user.LastName = value.LastName;
				user.Gender = value.Gender;
				user.BirthDate = value.BirthDate;
				user.PostalCode = value.PostalCode;
				user.UserName = value.UserName;

				//DataMapper.Map(value, user, "UserID");
				user.Save();
			}
			catch (ValidationException ex)
			{
				var brokenRules = user.GetBrokenRules().ToString();
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
					  ReasonPhrase = ex.Message,
					  Content = new StringContent(ex.ToString()),
					  RequestMessage = Request
				  });
			}
		}

		// PUT api/user/5
		[Authorize]
		public void Put(string userProfileId, [FromBody]User value)
		{
			IUser user = null;

			try
			{
				user = this.UserFactory.Fetch(userProfileId);
				var authUserID = MyVoteAuthentication.GetCurrentUserID();
				if (user.UserID != authUserID)
				{
					throw new HttpResponseException(new HttpResponseMessage
					{
						StatusCode = HttpStatusCode.Unauthorized,
						ReasonPhrase = "Only the authorized user may be updated",
						RequestMessage = Request
					});
				}
				DataMapper.Map(value, user, "ProfileID", "UserID");
				user.Save();
			}
			catch (ValidationException ex)
			{
				var brokenRules = user.GetBrokenRules().ToString();
				throw new HttpResponseException(
				  new HttpResponseMessage
				  {
					  StatusCode = HttpStatusCode.BadRequest,
					  ReasonPhrase = ex.Message.Replace(Environment.NewLine, " "),
					  Content = new StringContent(brokenRules),
					  RequestMessage = Request
				  });
			}
			catch (NullReferenceException)
			{
				throw new HttpResponseException(
				  new HttpResponseMessage
				  {
					  StatusCode = HttpStatusCode.NotFound,
					  Content = new StringContent(string.Format("No resource matching {0} found", userProfileId)),
					  RequestMessage = Request
				  });
			}
			catch (Exception ex)
			{
				throw new HttpResponseException(
				  new HttpResponseMessage
				  {
					  StatusCode = HttpStatusCode.BadRequest,
					  ReasonPhrase = ex.Message,
					  Content = new StringContent(ex.ToString()),
					  RequestMessage = Request
				  });
			}
		}
	}
}
