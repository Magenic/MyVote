using Autofac;
using Csla;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyVote.BusinessObjects.Contracts;
using MyVote.Data.Entities;
using Spackle;
using Spackle.Extensions;

namespace MyVote.BusinessObjects.Net.Tests
{
	[TestClass]
	public sealed class UserIdentityTests
	{
		[TestMethod]
		public void Fetch()
		{
			var generator = new RandomObjectGenerator();
			var profileId = generator.Generate<string>();
			var userId = generator.Generate<int>();
			var userName = generator.Generate<string>();

			var entity = EntityCreator.Create<MVUser>(_ =>
				{
					_.ProfileID = profileId;
					_.UserID = userId;
					_.UserName = userName;
					_.MVUserRole = EntityCreator.Create<MVUserRole>();
				});

			var entities = new Mock<IEntities>(MockBehavior.Strict);
			entities.Setup(_ => _.MVUsers)
				.Returns(new InMemoryDbSet<MVUser> { entity });
			entities.Setup(_ => _.Dispose());

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => entities.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var identity = DataPortal.Fetch<UserIdentity>(profileId);

				Assert.AreEqual(profileId, identity.ProfileID, nameof(identity.ProfileID));
				Assert.AreEqual(userId, identity.UserID, nameof(identity.UserID));
				Assert.AreEqual(userName, identity.UserName, nameof(identity.UserName));
				Assert.IsTrue(identity.IsAuthenticated, nameof(identity.IsAuthenticated));
			}

			entities.VerifyAll();
		}

		[TestMethod]
		public void FetchWhenUserDoesNotExist()
		{
			var generator = new RandomObjectGenerator();
			var profileId = generator.Generate<string>();

			var entities = new Mock<IEntities>(MockBehavior.Strict);
			entities.Setup(_ => _.MVUsers)
				.Returns(new InMemoryDbSet<MVUser>());
			entities.Setup(_ => _.Dispose());

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => entities.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var identity = DataPortal.Fetch<UserIdentity>(profileId);

				Assert.AreEqual(string.Empty, identity.ProfileID, nameof(identity.ProfileID));
				Assert.IsNull(identity.UserID, nameof(identity.UserID));
				Assert.AreEqual(string.Empty, identity.UserName, nameof(identity.UserName));
				Assert.IsFalse(identity.IsAuthenticated, nameof(identity.IsAuthenticated));
			}

			entities.VerifyAll();
		}
	}
}
