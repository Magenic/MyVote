using System.Diagnostics.CodeAnalysis;
using Csla.Core;
using Csla.Rules;

namespace MyVote.BusinessObjects.Core
{
	internal abstract class AuthorizationRuleCore
		: AuthorizationRule
	{
		protected AuthorizationRuleCore(AuthorizationActions action)
			: base(action) { }

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		protected AuthorizationRuleCore(AuthorizationActions action, IMemberInfo element)
			: base(action, element) { }
	}
}
