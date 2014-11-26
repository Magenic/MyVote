using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Csla.Core;
using Csla.Rules;

namespace MyVote.BusinessObjects.Rules
{
	internal sealed class PollSubmissionPollEndDateRule
		: BusinessRule
	{
		internal PollSubmissionPollEndDateRule(IPropertyInfo pollEndDateProperty, IPropertyInfo isActiveProperty)
			: base(pollEndDateProperty)
		{
			this.InputProperties = new List<IPropertyInfo> { pollEndDateProperty, isActiveProperty };
			this.AffectedProperties.Add(isActiveProperty);
		}

		[SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
		protected override void Execute(RuleContext context)
		{
			var pollEndDateProperty = this.InputProperties[0];
			var isActiveProperty = this.InputProperties[1];

			var pollEndDate = (DateTime)context.InputPropertyValues[pollEndDateProperty];
			var isActive = (bool)context.InputPropertyValues[isActiveProperty];

			var now = DateTime.UtcNow;

			if (pollEndDate < now)
			{
				context.AddErrorResult(pollEndDateProperty, "The poll has ended.");
				context.AddOutValue(isActiveProperty, false);
			}
		}
	}
}
