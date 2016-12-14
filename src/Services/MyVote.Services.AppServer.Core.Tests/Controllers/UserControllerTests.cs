using System;
using Moq;
using MyVote.BusinessObjects.Contracts;
using Spackle;
using Xunit;
using FluentAssertions;
using MyVote.Services.AppServer.Auth;
using MyVote.Services.AppServer.Models;
using System.Threading.Tasks;
using MyVote.Services.AppServer.Controllers;
using Microsoft.AspNetCore.Mvc;

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

			var user = new Mock<IUser>(MockBehavior.Strict);
			user.SetupGet(_ => _.UserID).Returns(userId);
			user.SetupGet(_ => _.ProfileID).Returns(profileId);
			user.SetupGet(_ => _.EmailAddress).Returns(emailAddress);
			user.SetupGet(_ => _.FirstName).Returns(firstName);
			user.SetupGet(_ => _.LastName).Returns(lastName);
			user.SetupGet(_ => _.Gender).Returns(gender);
			user.SetupGet(_ => _.BirthDate).Returns(birthDate);
			user.SetupGet(_ => _.PostalCode).Returns(postalCode);
			user.SetupGet(_ => _.UserName).Returns(userName);

			var userFactory = new Mock<IObjectFactory<IUser>>(MockBehavior.Strict);
			userFactory.Setup(_ => _.Fetch(profileId)).Returns(user.Object);

			var auth = new Mock<IMyVoteAuthentication>(MockBehavior.Strict);
			auth.Setup(_ => _.GetCurrentUserID()).Returns(userId);

			var controller = new UserController(
				userFactory.Object, auth.Object);

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

			userFactory.VerifyAll();
			user.VerifyAll();
		}

        [Fact(Skip = "debugging CreatedAtRoute 500 issue")]
        public async Task CreateUser()
		{
			var generator = new RandomObjectGenerator();
			var profileId = generator.Generate<string>();
			var entity = EntityCreator.Create<User>(_ => _.ProfileID = profileId);

			var user = new Mock<IUser>(MockBehavior.Strict);
			user.SetupSet(_ => _.BirthDate = entity.BirthDate);
			user.SetupSet(_ => _.EmailAddress = entity.EmailAddress);
			user.SetupSet(_ => _.FirstName = entity.FirstName);
			user.SetupSet(_ => _.Gender = entity.Gender);
			user.SetupSet(_ => _.LastName = entity.LastName);
			user.SetupSet(_ => _.PostalCode = entity.PostalCode);
			user.SetupSet(_ => _.UserName = entity.UserName);
			user.SetupGet(_ => _.UserID).Returns(entity.UserID);
			user.SetupGet(_ => _.IsSavable).Returns(true);
			user.Setup(_ => _.SaveAsync()).Returns(Task.FromResult<object>(user.Object));

			var userFactory = new Mock<IObjectFactory<IUser>>(MockBehavior.Strict);
			userFactory.Setup(_ => _.Create(profileId)).Returns(user.Object);

			var controller = new UserController(
				userFactory.Object, Mock.Of<IMyVoteAuthentication>());

			var result = (await controller.Post(entity)) as CreatedAtRouteResult;
			result.RouteName.Should().Be(UserController.GetByIdUri);
			result.RouteValues.Count.Should().Be(1);
			(result.RouteValues["id"] as int?).Should().Be(entity.UserID);
			result.Value.Should().Be(entity);

			userFactory.VerifyAll();
			user.VerifyAll();
		}

		[Fact]
		public async Task UpdateUser()
		{
			var generator = new RandomObjectGenerator();
			var profileId = generator.Generate<string>();
			var userId = generator.Generate<int>();

			var value = new User();
			var user = new Mock<IUser>(MockBehavior.Strict);
			user.SetupSet(_ => _.BirthDate = value.BirthDate);
			user.SetupSet(_ => _.EmailAddress = value.EmailAddress);
			user.SetupSet(_ => _.FirstName = value.FirstName);
			user.SetupSet(_ => _.Gender = value.Gender);
			user.SetupSet(_ => _.LastName = value.LastName);
			user.SetupSet(_ => _.PostalCode = value.PostalCode);
			user.SetupSet(_ => _.UserName = value.UserName);
			user.SetupGet(_ => _.UserID).Returns(userId);
			user.SetupGet(_ => _.UserName).Returns(default(string));
			user.SetupGet(_ => _.BirthDate).Returns(default(DateTime));
			user.SetupGet(_ => _.EmailAddress).Returns(default(string));
			user.SetupGet(_ => _.FirstName).Returns(default(string));
			user.SetupGet(_ => _.Gender).Returns(default(string));
			user.SetupGet(_ => _.LastName).Returns(default(string));
			user.SetupGet(_ => _.PostalCode).Returns(default(string));
			user.SetupGet(_ => _.IsSavable).Returns(true);
			user.Setup(_ => _.SaveAsync()).Returns(Task.FromResult<object>(user.Object));

			var userFactory = new Mock<IObjectFactory<IUser>>(MockBehavior.Strict);
			userFactory.Setup(_ => _.Fetch(profileId)).Returns(user.Object);

			var auth = new Mock<IMyVoteAuthentication>(MockBehavior.Strict);
			auth.Setup(_ => _.GetCurrentUserID()).Returns(userId);

			var controller = new UserController(
				userFactory.Object, auth.Object);

			await controller.Put(profileId, value);

			auth.VerifyAll();
			userFactory.VerifyAll();
			user.VerifyAll();
		}
	}
}
