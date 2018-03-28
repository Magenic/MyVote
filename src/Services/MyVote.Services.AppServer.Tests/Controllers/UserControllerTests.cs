using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MyVote.BusinessObjects.Contracts;
using MyVote.Services.AppServer.Auth;
using MyVote.Services.AppServer.Controllers;
using MyVote.Services.AppServer.Models;
using Rocks;
using Rocks.Options;
using Spackle;
using System;
using System.Threading.Tasks;
using Xunit;

namespace MyVote.Services.AppServer.Tests
{
	public sealed class UserControllerTests
	{
		[Fact]
		public void GetUser()
		{
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();
			var profileId = generator.Generate<string>();
			var emailAddress = generator.Generate<string>();
			var firstName = generator.Generate<string>();
			var lastName = generator.Generate<string>();
			var gender = generator.Generate<string>();
			var birthDate = generator.Generate<DateTime>();
			var postalCode = generator.Generate<string>();
			var userName = generator.Generate<string>();

			var user = Rock.Create<IUser>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			user.Handle(nameof(IUser.UserID), () => userId as int?);
			user.Handle(nameof(IUser.ProfileID), () => profileId);
			user.Handle(nameof(IUser.EmailAddress), () => emailAddress);
			user.Handle(nameof(IUser.FirstName), () => firstName);
			user.Handle(nameof(IUser.LastName), () => lastName);
			user.Handle(nameof(IUser.Gender), () => gender);
			user.Handle(nameof(IUser.BirthDate), () => birthDate as DateTime?);
			user.Handle(nameof(IUser.PostalCode), () => postalCode);
			user.Handle(nameof(IUser.UserName), () => userName);

			var userFactory = Rock.Create<IObjectFactory<IUser>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			userFactory.Handle(_ => _.Fetch(profileId)).Returns(user.Make());

			var auth = Rock.Create<IMyVoteAuthentication>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			auth.Handle(_ => _.GetCurrentUserID()).Returns(userId);

			var controller = new UserController(
				userFactory.Make(), auth.Make());

			var result = (controller.Get(profileId) as OkObjectResult).Value as User;

			result.UserName.Should().NotBeNullOrWhiteSpace();
			result.UserID.Should().Be(userId);
			result.ProfileID.Should().Be(profileId);
			result.EmailAddress.Should().Be(emailAddress);
			result.FirstName.Should().Be(firstName);
			result.LastName.Should().Be(lastName);
			result.Gender.Should().Be(gender);
			result.BirthDate.Should().Be(birthDate);
			result.PostalCode.Should().Be(postalCode);
			result.UserName.Should().Be(userName);

			userFactory.Verify();
			user.Verify();
		}

		[Fact(Skip = "debugging CreatedAtRoute 500 issue")]
		public async Task CreateUser()
		{
			var generator = new RandomObjectGenerator();
			var profileId = generator.Generate<string>();
			var entity = EntityCreator.Create<User>(_ => _.ProfileID = profileId);

			var user = Rock.Create<IUser>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			user.Handle<DateTime?>(nameof(IUser.BirthDate), _ => { });
			user.Handle<string>(nameof(IUser.EmailAddress), _ => { });
			user.Handle<string>(nameof(IUser.FirstName), _ => { });
			user.Handle<string>(nameof(IUser.Gender), _ => { });
			user.Handle<string>(nameof(IUser.LastName), _ => { });
			user.Handle<string>(nameof(IUser.PostalCode), _ => { });
			user.Handle<string>(nameof(IUser.UserName), _ => { });
			user.Handle(nameof(IUser.UserID), () => entity.UserID);
			user.Handle(nameof(IUser.IsSavable), () => true);
			user.Handle(_ => _.SaveAsync()).Returns(Task.FromResult<object>(user.Make()));

			var userFactory = Rock.Create<IObjectFactory<IUser>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			userFactory.Handle(_ => _.Create(profileId)).Returns(user.Make());

			var controller = new UserController(
				userFactory.Make(), Rock.Make<IMyVoteAuthentication>(
					new RockOptions(allowWarnings: AllowWarnings.Yes)));

			var result = (await controller.Post(entity)) as CreatedAtRouteResult;
			result.RouteName.Should().Be(UserController.GetByIdUri);
			result.RouteValues.Count.Should().Be(1);
			(result.RouteValues["id"] as int?).Should().Be(entity.UserID);
			result.Value.Should().Be(entity);

			userFactory.Verify();
			user.Verify();
		}

		[Fact]
		public async Task UpdateUser()
		{
			var generator = new RandomObjectGenerator();
			var profileId = generator.Generate<string>();
			var userId = generator.Generate<int>();

			var value = new User();
			var user = Rock.Create<IUser>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			user.Handle<DateTime?>(nameof(IUser.BirthDate), _ => { });
			user.Handle<string>(nameof(IUser.EmailAddress), _ => { });
			user.Handle<string>(nameof(IUser.FirstName), _ => { });
			user.Handle<string>(nameof(IUser.Gender), _ => { });
			user.Handle<string>(nameof(IUser.LastName), _ => { });
			user.Handle<string>(nameof(IUser.PostalCode), _ => { });
			user.Handle<string>(nameof(IUser.UserName), _ => { });
			user.Handle(nameof(IUser.UserID), () => userId as int?);
			user.Handle(nameof(IUser.UserName), () => default(string));
			user.Handle(nameof(IUser.BirthDate), () => default(DateTime?));
			user.Handle(nameof(IUser.EmailAddress), () => default(string));
			user.Handle(nameof(IUser.FirstName), () => default(string));
			user.Handle(nameof(IUser.Gender), () => default(string));
			user.Handle(nameof(IUser.LastName), () => default(string));
			user.Handle(nameof(IUser.PostalCode), () => default(string));
			user.Handle(nameof(IUser.IsSavable), () => true);
			user.Handle(_ => _.SaveAsync()).Returns(Task.FromResult<object>(user.Make()));

			var userFactory = Rock.Create<IObjectFactory<IUser>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			userFactory.Handle(_ => _.Fetch(profileId)).Returns(user.Make());

			var auth = Rock.Create<IMyVoteAuthentication>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			auth.Handle(_ => _.GetCurrentUserID()).Returns(userId);

			var controller = new UserController(
				userFactory.Make(), auth.Make());

			await controller.Put(profileId, value);

			auth.Verify();
			userFactory.Verify();
			user.Verify();
		}
	}
}
