using Csla;
using Csla.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Rules;
using Spackle;
using Spackle.Extensions;
using System.Security.Principal;

namespace MyVote.BusinessObjects.Net.Tests.Rules
{
	[TestClass]
	public sealed class CanDeletePollRuleTests
	{
		[TestMethod]
		public void ExecuteWhenApplicationContextUserIsNotIUserIdentity()
		{
			var principal = new Mock<IPrincipal>(MockBehavior.Strict);
			principal.Setup(_ => _.Identity).Returns(Mock.Of<IIdentity>());

			using (principal.Object.Bind(() => ApplicationContext.User))
			{
				var rule = new CanDeletePollRule();
				var context = new AuthorizationContext(rule, Mock.Of<IPoll>(), typeof(IPoll));
				(rule as IAuthorizationRule).Execute(context);

				Assert.IsFalse(context.HasPermission, nameof(context.HasPermission));
			}

			principal.VerifyAll();
		}

		[TestMethod]
		public void ExecuteWhenCurrentUserDidNotCreatePoll()
		{
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();
			var pollUserId = generator.Generate<int>();

			var poll = new Mock<IPoll>(MockBehavior.Strict);
			poll.Setup(_ => _.UserID).Returns(pollUserId);

			var identity = new Mock<IUserIdentity>(MockBehavior.Strict);
			identity.Setup(_ => _.UserID).Returns(userId);
			identity.Setup(_ => _.IsInRole(UserRoles.Admin)).Returns(false);

			var principal = new Mock<IPrincipal>(MockBehavior.Strict);
			principal.Setup(_ => _.Identity).Returns(identity.Object);

			using (principal.Object.Bind(() => ApplicationContext.User))
			{
				var rule = new CanDeletePollRule();
				var context = new AuthorizationContext(rule, poll.Object, typeof(IPoll));
				(rule as IAuthorizationRule).Execute(context);

				Assert.IsFalse(context.HasPermission, nameof(context.HasPermission));
			}

			principal.VerifyAll();
			identity.VerifyAll();
			poll.VerifyAll();
		}

		[TestMethod]
		public void ExecuteWhenCurrentUserCreatedPoll()
		{
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();

			var poll = new Mock<IPoll>(MockBehavior.Strict);
			poll.Setup(_ => _.UserID).Returns(userId);

			var identity = new Mock<IUserIdentity>(MockBehavior.Strict);
			identity.Setup(_ => _.UserID).Returns(userId);
			identity.Setup(_ => _.IsInRole(UserRoles.Admin)).Returns(false);

			var principal = new Mock<IPrincipal>(MockBehavior.Strict);
			principal.Setup(_ => _.Identity).Returns(identity.Object);

			using (principal.Object.Bind(() => ApplicationContext.User))
			{
				var rule = new CanDeletePollRule();
				var context = new AuthorizationContext(rule, poll.Object, typeof(IPoll));
				(rule as IAuthorizationRule).Execute(context);

				Assert.IsTrue(context.HasPermission, nameof(context.HasPermission));
			}

			principal.VerifyAll();
			identity.VerifyAll();
			poll.VerifyAll();
		}

		[TestMethod]
		public void ExecuteWhenCurrentUserIsInAdminRole()
		{
			var identity = new Mock<IUserIdentity>(MockBehavior.Strict);
			identity.Setup(_ => _.IsInRole(UserRoles.Admin)).Returns(true);

			var principal = new Mock<IPrincipal>(MockBehavior.Strict);
			principal.Setup(_ => _.Identity).Returns(identity.Object);

			using (principal.Object.Bind(() => ApplicationContext.User))
			{
				var rule = new CanDeletePollRule();
				var context = new AuthorizationContext(rule, Mock.Of<IPoll>(), typeof(IPoll));
				(rule as IAuthorizationRule).Execute(context);

				Assert.IsTrue(context.HasPermission, nameof(context.HasPermission));
			}

			principal.VerifyAll();
			identity.VerifyAll();
		}
	}
}
