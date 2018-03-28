using System.Collections.Generic;
using Csla.Core;
using Csla.Rules;

namespace MyVote.BusinessObjects.Rules
{
	internal sealed class MinimumAndMaximumPollOptionRule
		: BusinessRule
	{
		internal MinimumAndMaximumPollOptionRule(IPropertyInfo primaryProperty, IPropertyInfo pollMinAnswers, IPropertyInfo pollMaxAnswers)
			: base(primaryProperty)
		{
			this.InputProperties = new List<IPropertyInfo> { pollMinAnswers, pollMaxAnswers };
			this.AffectedProperties.AddRange(this.InputProperties);
		}

		protected override void Execute(RuleContext context)
		{
			var pollMinAnswersProperty = this.InputProperties[0];
			var pollMaxAnswersProperty = this.InputProperties[1];

			var pollMinAnswers = context.InputPropertyValues[pollMinAnswersProperty] as short?;
			var pollMaxAnswers = context.InputPropertyValues[pollMaxAnswersProperty] as short?;

			if (pollMinAnswers != null && pollMaxAnswers != null)
			{
				if (pollMinAnswers > pollMaxAnswers)
				{
					context.AddErrorResult(this.PrimaryProperty, "The minimum answer count must be less than or equal to the maximum count.");
				}
			}
		}
	}
}
