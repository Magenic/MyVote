using Autofac;
using Csla;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyVote.Core.Extensions;
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

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
			{
				var identity = DataPortal.Fetch<UserIdentity>(profileId);

				Assert.AreEqual(profileId, identity.ProfileID, identity.GetPropertyName(_ => _.ProfileID));
				Assert.AreEqual(userId, identity.UserID, identity.GetPropertyName(_ => _.UserID));
				Assert.AreEqual(userName, identity.UserName, identity.GetPropertyName(_ => _.UserName));
				Assert.IsTrue(identity.IsAuthenticated, identity.GetPropertyName(_ => _.IsAuthenticated));
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

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
			{
				var identity = DataPortal.Fetch<UserIdentity>(profileId);

				Assert.AreEqual(string.Empty, identity.ProfileID, identity.GetPropertyName(_ => _.ProfileID));
				Assert.IsNull(identity.UserID, identity.GetPropertyName(_ => _.UserID));
				Assert.AreEqual(string.Empty, identity.UserName, identity.GetPropertyName(_ => _.UserName));
				Assert.IsFalse(identity.IsAuthenticated, identity.GetPropertyName(_ => _.IsAuthenticated));
			}

			entities.VerifyAll();
		}
	}
}
