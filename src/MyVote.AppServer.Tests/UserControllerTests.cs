using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyVote.AppServer.Auth;
using MyVote.AppServer.Controllers;
using MyVote.BusinessObjects.Contracts;
using Spackle;

namespace MyVote.AppServer.Tests
{
	[TestClass]
	public class UserControllerTests
	{
		[TestMethod]
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

			var controller = new UserController();
			controller.UserFactory = userFactory.Object;
			controller.MyVoteAuthentication = auth.Object;

			var result = controller.Get(profileId);

			Assert.IsFalse(string.IsNullOrWhiteSpace(result.UserName));
			Assert.AreEqual(userId, result.UserID);
			Assert.AreEqual(profileId, result.ProfileID);
			Assert.AreEqual(emailAddress, result.EmailAddress);
			Assert.AreEqual(firstName, result.FirstName);
			Assert.AreEqual(lastName, result.LastName);
			Assert.AreEqual(gender, result.Gender);
			Assert.AreEqual(birthDate, result.BirthDate);
			Assert.AreEqual(postalCode, result.PostalCode);
			Assert.AreEqual(userName, result.UserName);

			userFactory.VerifyAll();
			user.VerifyAll();
		}

		[TestMethod]
		public void CreateUser()
		{
			var generator = new RandomObjectGenerator();
			var profileId = generator.Generate<string>();
			var entity = EntityCreator.Create<Models.User>(_ => _.ProfileID = profileId);

			var user = new Mock<IUser>();
			user.Setup(_ => _.Save());

			var userFactory = new Mock<IObjectFactory<IUser>>(MockBehavior.Strict);
			userFactory.Setup(_ => _.Create(profileId)).Returns(user.Object);

			var controller = new UserController();
			controller.UserFactory = userFactory.Object;
			controller.Put(entity);

			userFactory.VerifyAll();
			user.VerifyAll(); 
		}

		[TestMethod]
		public void UpdateUser()
		{
			var generator = new RandomObjectGenerator();
			var profileId = generator.Generate<string>();
			var userId = generator.Generate<int>();

			var user = new Mock<IUser>();
			user.SetupGet(_ => _.UserID).Returns(userId);
			user.Setup(_ => _.Save());

			var userFactory = new Mock<IObjectFactory<IUser>>(MockBehavior.Strict);
			userFactory.Setup(_ => _.Fetch(profileId)).Returns(user.Object);

			var auth = new Mock<IMyVoteAuthentication>(MockBehavior.Strict);
			auth.Setup(_ => _.GetCurrentUserID()).Returns(userId);

			var controller = new UserController();
			controller.UserFactory = userFactory.Object;
			controller.MyVoteAuthentication = auth.Object;

			controller.Put(profileId, new Models.User());

			userFactory.VerifyAll();
			user.VerifyAll();
		}
	}
}
