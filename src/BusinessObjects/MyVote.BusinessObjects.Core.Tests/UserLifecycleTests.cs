using Autofac;
using Csla;
using FluentAssertions;
using Moq;
using MyVote.BusinessObjects.Tests.Extensions;
using MyVote.Data.Entities;
using Spackle;
using Spackle.Extensions;
using System;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace MyVote.BusinessObjects.Tests
{
	public sealed class UserLifecycleTests
	{
		[Fact]
		public void Create()
		{
			var generator = new RandomObjectGenerator();
			var profileId = generator.Generate<string>();

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => Mock.Of<IEntitiesContext>());

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var user = new DataPortal<User>().Create(profileId);

				user.BirthDate.Should().NotHaveValue();
				user.EmailAddress.Should().BeEmpty();
				user.FirstName.Should().BeEmpty();
				user.Gender.Should().BeEmpty();
				user.LastName.Should().BeEmpty();
				user.PostalCode.Should().BeEmpty();
				user.ProfileAuthToken.Should().BeEmpty();
				user.ProfileID.Should().Be(profileId);
				user.UserID.Should().NotHaveValue();
				user.UserName.Should().BeEmpty();
				user.UserRoleID.Should().NotHaveValue();

				user.BrokenRulesCollection.AssertRuleCount(2);
				user.BrokenRulesCollection.AssertRuleCount(User.EmailAddressProperty, 1);
				user.BrokenRulesCollection.AssertValidationRuleExists<RequiredAttribute>(
					User.EmailAddressProperty, true);
				user.BrokenRulesCollection.AssertRuleCount(User.UserNameProperty, 1);
				user.BrokenRulesCollection.AssertValidationRuleExists<RequiredAttribute>(
					User.UserNameProperty, true);
			}
		}

		[Fact]
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

			var users = new InMemoryDbSet<Mvuser>();
			var entities = new Mock<IEntitiesContext>(MockBehavior.Strict);
			entities.Setup(_ => _.Mvuser)
				.Returns(users);
			entities.Setup(_ => _.SaveChanges()).Callback(() => users.Local[0].UserId = userId)
				.Returns(1);
			entities.Setup(_ => _.Dispose());

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => entities.Object);

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

				user.BirthDate.Should().Be(birthDate);
				user.EmailAddress.Should().Be(emailAddress);
				user.FirstName.Should().Be(firstName);
				user.Gender.Should().Be(gender);
				user.LastName.Should().Be(lastName);
				user.PostalCode.Should().Be(postalCode);
				user.ProfileAuthToken.Should().Be(profileAuthToken);
				user.UserID.Should().Be(userId);
				user.UserName.Should().Be(userName);
				user.UserRoleID.Should().Be(userRoleId);
			}

			entities.VerifyAll();
		}
	}
}
