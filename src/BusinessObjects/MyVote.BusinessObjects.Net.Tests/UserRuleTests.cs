using Autofac;
using Csla;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Net.Tests.Extensions;
using MyVote.Data.Entities;
using Spackle;
using Spackle.Extensions;

namespace MyVote.BusinessObjects.Net.Tests
{
	[TestClass]
	public sealed class UserRuleTests
	{
		[TestMethod]
		public void ChangeEmailAddressToValidValue()
		{
			var generator = new RandomObjectGenerator();
			var profileId = generator.Generate<string>();
			var emailAddress = generator.Generate<string>();

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var user = DataPortal.Create<User>(profileId);
				user.EmailAddress = emailAddress;
				user.BrokenRulesCollection.AssertRuleCount(User.EmailAddressProperty, 0);
			}
		}

		[TestMethod]
		public void ChangeUserNameToValidValue()
		{
			var generator = new RandomObjectGenerator();
			var profileId = generator.Generate<string>();
			var userName = generator.Generate<string>();

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var user = DataPortal.Create<User>(profileId);
				user.UserName = userName;
				user.BrokenRulesCollection.AssertRuleCount(User.UserNameProperty, 0);
			}
		}
	}
}
