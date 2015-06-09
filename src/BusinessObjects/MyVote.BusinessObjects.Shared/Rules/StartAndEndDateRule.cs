using System;
using System.Collections.Generic;
using Csla.Core;
using Csla.Rules;

namespace MyVote.BusinessObjects.Rules
{
	internal sealed class StartAndEndDateRule
		: BusinessRule
	{
		internal StartAndEndDateRule(IPropertyInfo primaryProperty, IPropertyInfo startDate, IPropertyInfo endDate)
			: base(primaryProperty)
		{
			this.InputProperties = new List<IPropertyInfo> { startDate, endDate };
			this.AffectedProperties.AddRange(this.InputProperties);
		}

		protected override void Execute(RuleContext context)
		{
			var startDateProperty = this.InputProperties[0];
			var endDateProperty = this.InputProperties[1];

			var startDate = context.InputPropertyValues[startDateProperty] as DateTime?;
			var endDate = context.InputPropertyValues[endDateProperty] as DateTime?;

			if (startDate != null && endDate != null)
			{
				if (startDate > endDate)
				{
					context.AddErrorResult(this.PrimaryProperty, "The start date cannot be after the end date.");
				}
			}
		}
	}
}
