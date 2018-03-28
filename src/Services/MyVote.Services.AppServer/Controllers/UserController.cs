using System;
using Csla.Data;
using MyVote.Services.AppServer.Auth;
using MyVote.Services.AppServer.Models;
using MyVote.BusinessObjects.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using static MyVote.Services.AppServer.Extensions.IBusinessBaseExtensions;

namespace MyVote.Services.AppServer.Controllers
{
    [Route("api/[controller]")]
    public sealed class UserController
        : Controller
    {
        public const string GetByIdUri = "GetById";
        private readonly IObjectFactory<IUser> userFactory;
        private readonly IMyVoteAuthentication authentication;

        public UserController(IObjectFactory<IUser> userFactory, IMyVoteAuthentication authentication)
        {
            if (userFactory == null)
            {
                throw new ArgumentNullException(nameof(userFactory));
            }

            if (authentication == null)
            {
                throw new ArgumentNullException(nameof(authentication));
            }

            this.userFactory = userFactory;
            this.authentication = authentication;
        }

        // GET api/user/5
        [Authorize]
        [HttpGet("{userProfileId}", Name = UserController.GetByIdUri)]
        public IActionResult Get(string userProfileId)
        {

            IUser user;
            try
            {                
                user = this.userFactory.Fetch(userProfileId);
            }
            catch (Exception)
            {
                return new OkObjectResult(new User
                {
                    UserID = null,
                    UserName = null
                });

            }

			
			//var authUserID = this.authentication.GetCurrentUserID().Value;

			//if (user.UserID != authUserID)
			//{
			//	return new UnauthorizedResult();
			//}

			return new OkObjectResult(new User
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
			});
		}

		// POST api/user
		//[Authorize]
		[HttpPost]
		public async Task<IActionResult> Post([FromBody]User value)
		{
			var user = this.userFactory.Create(value.ProfileID);

			user.EmailAddress = value.EmailAddress;
			user.FirstName = value.FirstName;
			user.LastName = value.LastName;
			user.Gender = value.Gender;
			user.BirthDate = value.BirthDate;
			user.PostalCode = value.PostalCode;
			user.UserName = value.UserName;

		    //return Ok(user);

            return await user.PersistAsync(
                () =>
                {
                    return Ok(user);
                });
        }

		// PUT api/user/5
		//[Authorize]
		[HttpPut("{id}")]
		public async Task<IActionResult> Put(string userProfileId, [FromBody]User value)
		{
			var user = this.userFactory.Fetch(userProfileId);
			var authUserID = this.authentication.GetCurrentUserID();

			if (user.UserID != authUserID)
			{
				return new UnauthorizedResult();
			}

			DataMapper.Map(value, user, nameof(user.ProfileID), nameof(user.UserID));
			return await user.PersistAsync();
		}
	}
}