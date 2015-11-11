using System.Diagnostics.CodeAnalysis;
using Csla.Core;
using Csla.Rules;

namespace MyVote.BusinessObjects.Core
{
	internal abstract class BusinessRuleCore
		: BusinessRule
	{
		protected BusinessRuleCore()
			: base() { }

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		protected BusinessRuleCore(IPropertyInfo primaryProperty)
			: base(primaryProperty) { }
	}
}
