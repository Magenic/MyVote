using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Csla.Core;
using Csla.Rules;

namespace MyVote.BusinessObjects.Rules
{
	internal sealed class PollSubmissionPollStartDateRule
		: BusinessRule
	{
		internal PollSubmissionPollStartDateRule(IPropertyInfo pollStartDateProperty, IPropertyInfo isActiveProperty)
			: base(pollStartDateProperty)
		{
			this.InputProperties = new List<IPropertyInfo> { pollStartDateProperty, isActiveProperty };
			this.AffectedProperties.Add(isActiveProperty);
		}

		[SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
		protected override void Execute(RuleContext context)
		{
			var pollStartDateProperty = this.InputProperties[0];
			var isActiveProperty = this.InputProperties[1];

			var pollStartDate = (DateTime)context.InputPropertyValues[pollStartDateProperty];
			var isActive = (bool)context.InputPropertyValues[isActiveProperty];

			var now = DateTime.UtcNow;

			if (pollStartDate > now)
			{
				context.AddErrorResult(pollStartDateProperty, "The poll has not started.");
				context.AddOutValue(isActiveProperty, false);
			}
		}
	}
}
