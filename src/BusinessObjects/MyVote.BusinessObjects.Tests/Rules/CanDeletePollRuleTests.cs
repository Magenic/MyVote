using Csla;
using Csla.Rules;
using FluentAssertions;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Rules;
using Rocks;
using Rocks.Options;
using Spackle;
using Spackle.Extensions;
using System.Security.Principal;
using Xunit;

namespace MyVote.BusinessObjects.Tests.Rules
{
	public sealed class CanDeletePollRuleTests
	{
		[Fact]
		public void ExecuteWhenApplicationContextUserIsNotIUserIdentity()
		{
			var principal = Rock.Create<IPrincipal>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			principal.Handle(nameof(IPrincipal.Identity), () => Rock.Make<IIdentity>(
				new RockOptions(allowWarnings: AllowWarnings.Yes)));

			using (principal.Make().Bind(() => ApplicationContext.User))
			{
				var rule = new CanDeletePollRule();
				var context = new AuthorizationContext(rule, Rock.Make<IPoll>(
					new RockOptions(allowWarnings: AllowWarnings.Yes)), typeof(IPoll));
				(rule as IAuthorizationRule).Execute(context);

				context.HasPermission.Should().BeFalse();
			}

			principal.Verify();
		}

		[Fact]
		public void ExecuteWhenCurrentUserDidNotCreatePoll()
		{
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();
			var pollUserId = generator.Generate<int>();

			var poll = Rock.Create<IPoll>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			poll.Handle(nameof(IPoll.UserID), () => pollUserId);

			var identity = Rock.Create<IUserIdentity>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			identity.Handle(nameof(IUserIdentity.UserID), () => userId as int?);
			identity.Handle(_ => _.IsInRole(UserRoles.Admin)).Returns(false);

			var principal = Rock.Create<IPrincipal>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			principal.Handle(nameof(IPrincipal.Identity), () => identity.Make());

			using (principal.Make().Bind(() => ApplicationContext.User))
			{
				var rule = new CanDeletePollRule();
				var context = new AuthorizationContext(rule, poll.Make(), typeof(IPoll));
				(rule as IAuthorizationRule).Execute(context);

				context.HasPermission.Should().BeFalse();
			}

			principal.Verify();
			identity.Verify();
			poll.Verify();
		}

		[Fact]
		public void ExecuteWhenCurrentUserCreatedPoll()
		{
			var generator = new RandomObjectGenerator();
			var userId = generator.Generate<int>();

			var poll = Rock.Create<IPoll>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			poll.Handle(nameof(IPoll.UserID), () => userId);

			var identity = Rock.Create<IUserIdentity>(
				new RockOptions(allowWarnings: AllowWarnings.Yes,
				level: OptimizationSetting.Debug, codeFile: CodeFileOptions.Create));
			identity.Handle(nameof(IUserIdentity.UserID), () => userId as int?);
			identity.Handle(_ => _.IsInRole(UserRoles.Admin)).Returns(false);

			var principal = Rock.Create<IPrincipal>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			principal.Handle(nameof(IPrincipal.Identity), () => identity.Make());

			using (principal.Make().Bind(() => ApplicationContext.User))
			{
				var rule = new CanDeletePollRule();
				var context = new AuthorizationContext(rule, poll.Make(), typeof(IPoll));
				(rule as IAuthorizationRule).Execute(context);

				context.HasPermission.Should().BeTrue();
			}

			principal.Verify();
			identity.Verify();
			poll.Verify();
		}

		[Fact]
		public void ExecuteWhenCurrentUserIsInAdminRole()
		{
			var identity = Rock.Create<IUserIdentity>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			identity.Handle(_ => _.IsInRole(UserRoles.Admin)).Returns(true);

			var principal = Rock.Create<IPrincipal>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			principal.Handle(nameof(IPrincipal.Identity), () => identity.Make());

			using (principal.Make().Bind(() => ApplicationContext.User))
			{
				var rule = new CanDeletePollRule();
				var context = new AuthorizationContext(rule, Rock.Make<IPoll>(
					new RockOptions(allowWarnings: AllowWarnings.Yes)), typeof(IPoll));
				(rule as IAuthorizationRule).Execute(context);

				context.HasPermission.Should().BeTrue();
			}

			principal.Verify();
			identity.Verify();
		}
	}
}
