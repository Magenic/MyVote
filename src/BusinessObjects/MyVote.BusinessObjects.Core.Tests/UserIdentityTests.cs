using Autofac;
using Csla;
using FluentAssertions;
using Moq;
using MyVote.Data.Entities;
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

			var entities = new Mock<IEntitiesContext>(MockBehavior.Strict);
			entities.Setup(_ => _.Mvuser)
				.Returns(new InMemoryDbSet<Mvuser> { entity });
			entities.Setup(_ => _.Dispose());

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => entities.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var identity = DataPortal.Fetch<UserIdentity>(profileId);

				identity.ProfileID.Should().Be(profileId);
				identity.UserID.Should().Be(userId);
				identity.UserName.Should().Be(userName);
				identity.IsAuthenticated.Should().BeTrue();
			}

			entities.VerifyAll();
		}

		[Fact]
		public void FetchWhenUserDoesNotExist()
		{
			var generator = new RandomObjectGenerator();
			var profileId = generator.Generate<string>();

			var entities = new Mock<IEntitiesContext>(MockBehavior.Strict);
			entities.Setup(_ => _.Mvuser)
				.Returns(new InMemoryDbSet<Mvuser>());
			entities.Setup(_ => _.Dispose());

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => entities.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var identity = DataPortal.Fetch<UserIdentity>(profileId);

				identity.ProfileID.Should().BeEmpty();
				identity.UserID.Should().NotHaveValue();
				identity.UserName.Should().BeEmpty();
				identity.IsAuthenticated.Should().BeFalse();
			}

			entities.VerifyAll();
		}
	}
}
