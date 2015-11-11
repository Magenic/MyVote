using System.Collections.Generic;
using Csla.Core;
using Csla.Rules;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;

namespace MyVote.BusinessObjects.Rules
{
	internal sealed class PollMaxAnswersRule
		: BusinessRule
	{
		internal PollMaxAnswersRule(IPropertyInfo pollMaxAnswersRule, IPropertyInfo pollOptions)
			: base(pollMaxAnswersRule)
		{
			this.InputProperties = new List<IPropertyInfo> { pollMaxAnswersRule, pollOptions };
			this.AffectedProperties.Add(pollOptions);
		}

		protected override void Execute(RuleContext context)
		{
			var pollMaxAnswersProperty = this.InputProperties[0];
			var pollOptionsProperty = this.InputProperties[1];

			var pollMaxAnswers = context.InputPropertyValues[pollMaxAnswersProperty] as short?;
			var pollOptions = context.InputPropertyValues[pollOptionsProperty] as BusinessList<IPollOption>;

			if (pollMaxAnswers != null)
			{
				if (pollMaxAnswers <= 0)
				{
					context.AddErrorResult(pollMaxAnswersProperty, "The maximum answer count must be greater than zero.");
				}
				else if (pollOptions != null && pollMaxAnswers > pollOptions.Count)
				{
					context.AddErrorResult(pollMaxAnswersProperty, "The maximum answer count must be less than or equal to the poll option count.");
				}
			}
		}
	}
}
