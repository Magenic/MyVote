using System.Collections.Generic;
using Csla.Core;
using Csla.Rules;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;

namespace MyVote.BusinessObjects.Rules
{
	internal sealed class PollMinAnswersRule
		: BusinessRule
	{
		internal PollMinAnswersRule(IPropertyInfo pollMinAnswersProperty, IPropertyInfo pollOptionsProperty)
			: base(pollMinAnswersProperty)
		{
			this.InputProperties = new List<IPropertyInfo> { pollMinAnswersProperty, pollOptionsProperty };
			this.AffectedProperties.Add(pollOptionsProperty);
		}

		protected override void Execute(RuleContext context)
		{
			var pollMinAnswersProperty = this.InputProperties[0];
			var pollOptionsProperty = this.InputProperties[1];

			var pollMinAnswers = context.InputPropertyValues[pollMinAnswersProperty] as short?;
			var pollOptions = context.InputPropertyValues[pollOptionsProperty] as BusinessList<IPollOption>;

			if (pollMinAnswers != null)
			{
				if (pollMinAnswers <= 0)
				{
					context.AddErrorResult(pollMinAnswersProperty, "The minimum answer count must be greater than zero.");
				}
				else if (pollOptions != null && pollMinAnswers > pollOptions.Count)
				{
					context.AddErrorResult(pollMinAnswersProperty, "The minimum answer count must be less than or equal to the poll option count.");
				}
			}
		}
	}
}
