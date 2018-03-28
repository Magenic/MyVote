using System.Collections.Generic;
using System.Linq;
using Csla.Core;
using Csla.Rules;
using MyVote.BusinessObjects.Contracts;

namespace MyVote.BusinessObjects.Rules
{
	internal sealed class MinimumAndMaximumPollSubmissionResponsesRule
		: BusinessRule
	{
		internal MinimumAndMaximumPollSubmissionResponsesRule(
			IPropertyInfo primaryProperty, IPropertyInfo pollMinAnswers, IPropertyInfo pollMaxAnswers)
			: base(primaryProperty)
		{
			this.InputProperties = new List<IPropertyInfo> { primaryProperty, pollMinAnswers, pollMaxAnswers };
		}

		protected override void Execute(RuleContext context)
		{
			var responsesProperty = this.InputProperties[0];
			var pollMinAnswersProperty = this.InputProperties[1];
			var pollMaxAnswersProperty = this.InputProperties[2];

			var pollMinAnswers = (short)context.InputPropertyValues[pollMinAnswersProperty];
			var pollMaxAnswers = (short)context.InputPropertyValues[pollMaxAnswersProperty];
			var responses = context.InputPropertyValues[responsesProperty] as IPollSubmissionResponseCollection;

			var responseCount = (from r in responses
										where r.IsOptionSelected
										select r).Count();

			if (responseCount < pollMinAnswers || responseCount > pollMaxAnswers)
			{
				context.AddErrorResult(this.PrimaryProperty,
					"The number of selections must be greater than or equal to the minimum answer count and less than or equal to the maximum count.");
			}
		}
	}
}
