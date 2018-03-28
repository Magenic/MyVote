using Autofac;
using Csla;
using FluentAssertions;
using MyVote.Data.Entities;
using Rocks;
using Rocks.Options;
using Spackle;
using Spackle.Extensions;
using Xunit;

namespace MyVote.BusinessObjects.Tests
{
	public sealed class UserIdentityTests
	{
		[Fact]
		public void Fetch()
		{
			var generator = new RandomObjectGenerator();
			var profileId = generator.Generate<string>();
			var userId = generator.Generate<int>();
			var userName = generator.Generate<string>();
			var userRole = EntityCreator.Create<MvuserRole>();

			var entity = EntityCreator.Create<Mvuser>(_ =>
				{
					_.ProfileId = profileId;
					_.UserId = userId;
					_.UserName = userName;
					_.UserRole = userRole;
					_.UserRoleId = userRole.UserRoleId;
				});

			var entities = Rock.Create<IEntitiesContext>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			entities.Handle(nameof(IEntitiesContext.Mvuser), () => new InMemoryDbSet<Mvuser> { entity });
			entities.Handle(_ => _.Dispose());

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => entities.Make());

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var identity = DataPortal.Fetch<UserIdentity>(profileId);

				identity.ProfileID.Should().Be(profileId);
				identity.UserID.Should().Be(userId);
				identity.UserName.Should().Be(userName);
				identity.IsAuthenticated.Should().BeTrue();
			}

			entities.Verify();
		}

		[Fact]
		public void FetchWhenUserDoesNotExist()
		{
			var generator = new RandomObjectGenerator();
			var profileId = generator.Generate<string>();

			var entities = Rock.Create<IEntitiesContext>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			entities.Handle(nameof(IEntitiesContext.Mvuser), () => new InMemoryDbSet<Mvuser>());
			entities.Handle(_ => _.Dispose());

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => entities.Make());

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var identity = DataPortal.Fetch<UserIdentity>(profileId);

				identity.ProfileID.Should().BeEmpty();
				identity.UserID.Should().NotHaveValue();
				identity.UserName.Should().BeEmpty();
				identity.IsAuthenticated.Should().BeFalse();
			}

			entities.Verify();
		}
	}
}
