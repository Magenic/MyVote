using Csla;
using Csla.Rules;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;

namespace MyVote.BusinessObjects.Rules
{
	internal sealed class CanDeletePollRule
		: AuthorizationRuleCore
	{
		internal CanDeletePollRule()
			: base(AuthorizationActions.DeleteObject) { }

		protected override void Execute(AuthorizationContext context)
		{
			var pollUserId = (context.Target as IPoll).UserID;
			var currentUser = ApplicationContext.User.Identity as IUserIdentity;

			context.HasPermission = currentUser != null && (
				currentUser.IsInRole(UserRoles.Admin) || currentUser.UserID == pollUserId);
		}
	}
}
