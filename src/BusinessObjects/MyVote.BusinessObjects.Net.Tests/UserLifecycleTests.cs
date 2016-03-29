using Autofac;
using Csla;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Net.Tests.Extensions;
using MyVote.Data.Entities;
using Spackle;
using Spackle.Extensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace MyVote.BusinessObjects.Net.Tests
{
	[TestClass]
	public sealed class UserLifecycleTests
	{
		[TestMethod]
		public void Create()
		{
			var generator = new RandomObjectGenerator();
			var profileId = generator.Generate<string>();

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var user = new DataPortal<User>().Create(profileId);

				Assert.IsNull(user.BirthDate, nameof(user.BirthDate));
				Assert.AreEqual(string.Empty, user.EmailAddress, nameof(user.EmailAddress));
				Assert.AreEqual(string.Empty, user.FirstName, nameof(user.FirstName));
				Assert.AreEqual(string.Empty, user.Gender, nameof(user.Gender));
				Assert.AreEqual(string.Empty, user.LastName, nameof(user.LastName));
				Assert.AreEqual(string.Empty, user.PostalCode, nameof(user.PostalCode));
				Assert.AreEqual(string.Empty, user.ProfileAuthToken, nameof(user.ProfileAuthToken));
				Assert.AreEqual(profileId, user.ProfileID, nameof(user.ProfileID));
				Assert.IsNull(user.UserID, nameof(user.UserID));
				Assert.AreEqual(string.Empty, user.UserName, nameof(user.UserName));
				Assert.IsNull(user.UserRoleID, nameof(user.UserRoleID));

				user.BrokenRulesCollection.AssertRuleCount(2);
				user.BrokenRulesCollection.AssertRuleCount(User.EmailAddressProperty, 1);
				user.BrokenRulesCollection.AssertValidationRuleExists<RequiredAttribute>(
					User.EmailAddressProperty, true);
				user.BrokenRulesCollection.AssertRuleCount(User.UserNameProperty, 1);
				user.BrokenRulesCollection.AssertValidationRuleExists<RequiredAttribute>(
					User.UserNameProperty, true);
			}
		}

		[TestMethod]
		public void Insert()
		{
			var generator = new RandomObjectGenerator();
			var birthDate = generator.Generate<DateTime>();
			var emailAddress = generator.Generate<string>();
			var firstName = generator.Generate<string>();
			var gender = generator.Generate<string>();
			var lastName = generator.Generate<string>();
			var postalCode = generator.Generate<string>();
			var profileAuthToken = generator.Generate<string>();
			var profileId = generator.Generate<string>();
			var userId = generator.Generate<int>();
			var userName = generator.Generate<string>();
			var userRoleId = generator.Generate<int>();

			var users = new InMemoryDbSet<MVUser>();
			var entities = new Mock<IEntities>(MockBehavior.Strict);
			entities.Setup(_ => _.MVUsers)
				.Returns(users);
			entities.Setup(_ => _.SaveChanges()).Callback(() => users.Local[0].UserID = userId)
				.Returns(1);
			entities.Setup(_ => _.Dispose());

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => entities.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var user = new DataPortal<User>().Create(profileId);
				user.BirthDate = birthDate;
				user.EmailAddress = emailAddress;
				user.FirstName = firstName;
				user.Gender = gender;
				user.LastName = lastName;
				user.PostalCode = postalCode;
				user.ProfileAuthToken = profileAuthToken;
				user.UserName = userName;
				user.UserRoleID = userRoleId;
				user = user.Save();

				Assert.AreEqual(birthDate, user.BirthDate, nameof(user.BirthDate));
				Assert.AreEqual(emailAddress, user.EmailAddress, nameof(user.EmailAddress));
				Assert.AreEqual(firstName, user.FirstName, nameof(user.FirstName));
				Assert.AreEqual(gender, user.Gender, nameof(user.Gender));
				Assert.AreEqual(lastName, user.LastName, nameof(user.LastName));
				Assert.AreEqual(postalCode, user.PostalCode, nameof(user.PostalCode));
				Assert.AreEqual(profileAuthToken, user.ProfileAuthToken, nameof(user.ProfileAuthToken));
				Assert.AreEqual(userId, user.UserID, nameof(user.UserID));
				Assert.AreEqual(userName, user.UserName, nameof(user.UserName));
				Assert.AreEqual(userRoleId, user.UserRoleID, nameof(user.UserRoleID));
			}

			entities.VerifyAll();
		}
	}
}
