using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Csla.Core;
using Csla.Rules;

namespace MyVote.BusinessObjects.Rules
{
	internal sealed class PollSubmissionPollDeletedFlagRule
		: BusinessRule
	{
		internal PollSubmissionPollDeletedFlagRule(IPropertyInfo pollDeletedFlagProperty, IPropertyInfo isActiveProperty)
			: base(pollDeletedFlagProperty)
		{
			this.InputProperties = new List<IPropertyInfo> { pollDeletedFlagProperty, isActiveProperty };
			this.AffectedProperties.Add(isActiveProperty);
		}

		[SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
		protected override void Execute(RuleContext context)
		{
			var pollDeletedFlagProperty = this.InputProperties[0];
			var isActiveProperty = this.InputProperties[1];

			var pollDeletedFlag = context.InputPropertyValues[pollDeletedFlagProperty] as bool?;
			var isActive = (bool)context.InputPropertyValues[isActiveProperty];

			if (pollDeletedFlag != null && pollDeletedFlag.Value)
			{
				context.AddErrorResult(pollDeletedFlagProperty, "The poll is deleted.");
				context.AddOutValue(isActiveProperty, false);
			}
		}
	}
}
