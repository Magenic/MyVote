using Csla.Core;
using Csla.Rules;
using FluentAssertions;
using MyVote.BusinessObjects.Rules;
using Rocks;
using Rocks.Options;
using System;
using System.Collections.Generic;
using Xunit;

namespace MyVote.BusinessObjects.Tests.Rules
{
	public sealed class PollSubmissionPollStartDateRuleTests
	{
		[Fact]
		public void ExecuteWhenStartDateIsBeforeNow()
		{
			var pollStartDateProperty = Rock.Create<IPropertyInfo>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollStartDateProperty.Handle(nameof(IPropertyInfo.Name), () => "PollStartDate", 3);
			var pollStartDatePropertyChunk = pollStartDateProperty.Make();

			var isActiveProperty = Rock.Make<IPropertyInfo>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));

			var rule = new PollSubmissionPollStartDateRule(
				pollStartDatePropertyChunk, isActiveProperty);

			var context = new RuleContext(null, rule, null,
				new Dictionary<IPropertyInfo, object>
				{
					{ pollStartDatePropertyChunk, DateTime.UtcNow.AddDays(-2) },
					{ isActiveProperty, true }
				});
			(rule as IBusinessRule).Execute(context);

			context.Results.Count.Should().Be(0);
			context.OutputPropertyValues.Should().BeNull();

			pollStartDateProperty.Verify();
		}

		[Fact]
		public void ExecuteWhenStartDateIsAfterNow()
		{
			var pollStartDateProperty = Rock.Create<IPropertyInfo>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollStartDateProperty.Handle(nameof(IPropertyInfo.Name), () => "PollStartDate", 3);
			var pollStartDatePropertyChunk = pollStartDateProperty.Make();

			var isActiveProperty = Rock.Make<IPropertyInfo>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));

			var rule = new PollSubmissionPollStartDateRule(
				pollStartDatePropertyChunk, isActiveProperty);

			var context = new RuleContext(null, rule, null,
				new Dictionary<IPropertyInfo, object>
				{
					{ pollStartDatePropertyChunk, DateTime.UtcNow.AddDays(2) },
					{ isActiveProperty, true }
				});
			(rule as IBusinessRule).Execute(context);

			context.Results.Count.Should().Be(1);
			((bool)context.OutputPropertyValues[isActiveProperty]).Should().BeFalse();

			pollStartDateProperty.Verify();
		}
	}
}
