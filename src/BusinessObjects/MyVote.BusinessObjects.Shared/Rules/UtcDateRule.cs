using System;
using System.Collections.Generic;
using System.Reflection;
using Csla.Core;
using Csla.Rules;

namespace MyVote.BusinessObjects.Rules
{
	internal sealed class UtcDateRule
		: BusinessRule
	{
		internal UtcDateRule(IPropertyInfo dateProperty)
			: base(dateProperty)
		{
			this.InputProperties = new List<IPropertyInfo> { dateProperty };
		}

		protected override void Execute(RuleContext context)
		{
			var dateProperty = this.InputProperties[0];

			var value = context.InputPropertyValues[dateProperty];

			if (value != null)
			{
				if (typeof(DateTime?).GetTypeInfo().IsAssignableFrom(value.GetType().GetTypeInfo()))
				{
					var nullableDate = value as DateTime?;

					if (nullableDate != null)
					{
						context.AddOutValue(dateProperty, nullableDate.Value.ToUniversalTime());
					}
				}
				else if (typeof(DateTime).GetTypeInfo().IsAssignableFrom(value.GetType().GetTypeInfo()))
				{
					var date = (DateTime)context.InputPropertyValues[dateProperty];
					context.AddOutValue(dateProperty, date.ToUniversalTime());
				}
			}
		}
	}
}
