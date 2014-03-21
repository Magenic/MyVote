using System;
using System.ComponentModel.DataAnnotations;
using Autofac;
using Csla;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyVote.BusinessObjects.Net.Tests.Extensions;
using MyVote.Core.Extensions;
using MyVote.Repository;
using Spackle;
using Spackle.Extensions;

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

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
			{
				var user = new DataPortal<User>().Create(profileId);

				Assert.IsNull(user.BirthDate, user.GetPropertyName(_ => _.BirthDate));
				Assert.AreEqual(string.Empty, user.EmailAddress, user.GetPropertyName(_ => _.EmailAddress));
				Assert.AreEqual(string.Empty, user.FirstName, user.GetPropertyName(_ => _.FirstName));
				Assert.AreEqual(string.Empty, user.Gender, user.GetPropertyName(_ => _.Gender));
				Assert.AreEqual(string.Empty, user.LastName, user.GetPropertyName(_ => _.LastName));
				Assert.AreEqual(string.Empty, user.PostalCode, user.GetPropertyName(_ => _.PostalCode));
				Assert.AreEqual(string.Empty, user.ProfileAuthToken, user.GetPropertyName(_ => _.ProfileAuthToken));
				Assert.AreEqual(profileId, user.ProfileID, user.GetPropertyName(_ => _.ProfileID));
				Assert.IsNull(user.UserID, user.GetPropertyName(_ => _.UserID));
				Assert.AreEqual(string.Empty, user.UserName, user.GetPropertyName(_ => _.UserName));
				Assert.IsNull(user.UserRoleID, user.GetPropertyName(_ => _.UserRoleID));

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
			var entities = new Mock<IEntities>();
			entities.Setup(_ => _.MVUsers)
				.Returns(users);
			entities.Setup(_ => _.SaveChanges()).Callback(() => users.Local[0].UserID = userId);
			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => entities.Object);

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
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

				Assert.AreEqual(birthDate, user.BirthDate, user.GetPropertyName(_ => _.BirthDate));
				Assert.AreEqual(emailAddress, user.EmailAddress, user.GetPropertyName(_ => _.EmailAddress));
				Assert.AreEqual(firstName, user.FirstName, user.GetPropertyName(_ => _.FirstName));
				Assert.AreEqual(gender, user.Gender, user.GetPropertyName(_ => _.Gender));
				Assert.AreEqual(lastName, user.LastName, user.GetPropertyName(_ => _.LastName));
				Assert.AreEqual(postalCode, user.PostalCode, user.GetPropertyName(_ => _.PostalCode));
				Assert.AreEqual(profileAuthToken, user.ProfileAuthToken, user.GetPropertyName(_ => _.ProfileAuthToken));
				Assert.AreEqual(userId, user.UserID, user.GetPropertyName(_ => _.UserID));
				Assert.AreEqual(userName, user.UserName, user.GetPropertyName(_ => _.UserName));
				Assert.AreEqual(userRoleId, user.UserRoleID, user.GetPropertyName(_ => _.UserRoleID));
			}
		}
	}
}
