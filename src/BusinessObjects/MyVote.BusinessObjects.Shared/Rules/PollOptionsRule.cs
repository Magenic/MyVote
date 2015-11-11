using System.Collections.Generic;
using Csla.Core;
using Csla.Rules;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;

namespace MyVote.BusinessObjects.Rules
{
	internal sealed class PollOptionsRule
		: BusinessRule
	{
		internal PollOptionsRule(IPropertyInfo pollOptionsProperty)
			: base(pollOptionsProperty)
		{
			this.InputProperties = new List<IPropertyInfo> { pollOptionsProperty };
		}

		protected override void Execute(RuleContext context)
		{
			var pollOptionsProperty = this.InputProperties[0];
			var pollOptions = context.InputPropertyValues[pollOptionsProperty] as BusinessList<IPollOption>;

			if (pollOptions == null || pollOptions.Count < 2)
			{
				context.AddErrorResult(pollOptionsProperty, "There must be at least two poll answers specified.");
			}
		}
	}
}
