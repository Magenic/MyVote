using Autofac;
using Csla;
using MyVote.BusinessObjects.Tests.Extensions;
using MyVote.Data.Entities;
using Rocks;
using Rocks.Options;
using Spackle;
using Spackle.Extensions;
using Xunit;

namespace MyVote.BusinessObjects.Tests
{
	public sealed class UserRuleTests
	{
		[Fact]
		public void ChangeEmailAddressToValidValue()
		{
			var generator = new RandomObjectGenerator();
			var profileId = generator.Generate<string>();
			var emailAddress = generator.Generate<string>();

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => Rock.Make<IEntitiesContext>(
				new RockOptions(allowWarnings: AllowWarnings.Yes)));

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var user = DataPortal.Create<User>(profileId);
				user.EmailAddress = emailAddress;
				user.BrokenRulesCollection.AssertRuleCount(User.EmailAddressProperty, 0);
			}
		}

		[Fact]
		public void ChangeUserNameToValidValue()
		{
			var generator = new RandomObjectGenerator();
			var profileId = generator.Generate<string>();
			var userName = generator.Generate<string>();

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => Rock.Make<IEntitiesContext>(
				new RockOptions(allowWarnings: AllowWarnings.Yes)));

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
