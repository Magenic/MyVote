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
	public sealed class PollSubmissionPollEndDateRuleTests
	{
		[Fact]
		public void ExecuteWhenEndDateIsBeforeNow()
		{
			var pollEndDateProperty = Rock.Create<IPropertyInfo>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollEndDateProperty.Handle(nameof(IPropertyInfo.Name), () => "PollEndDate", 3);
			var pollEndDatePropertyChunk = pollEndDateProperty.Make();

			var isActiveProperty = Rock.Make<IPropertyInfo>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));

			var rule = new PollSubmissionPollEndDateRule(
				pollEndDatePropertyChunk, isActiveProperty);

			var context = new RuleContext(null, rule, null,
				new Dictionary<IPropertyInfo, object>
				{
					{ pollEndDatePropertyChunk, DateTime.UtcNow.AddDays(-2) },
					{ isActiveProperty, true }
				});
			(rule as IBusinessRule).Execute(context);

			context.Results.Count.Should().Be(1);
			((bool)context.OutputPropertyValues[isActiveProperty]).Should().BeFalse();

			pollEndDateProperty.Verify();
		}

		[Fact]
		public void ExecuteWhenEndDateIsAfterNow()
		{
			var pollEndDateProperty = Rock.Create<IPropertyInfo>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollEndDateProperty.Handle(nameof(IPropertyInfo.Name), () => "PollEndDate", 3);
			var pollEndDatePropertyChunk = pollEndDateProperty.Make();

			var isActiveProperty = Rock.Make<IPropertyInfo>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));

			var rule = new PollSubmissionPollEndDateRule(
				pollEndDatePropertyChunk, isActiveProperty);

			var context = new RuleContext(null, rule, null,
				new Dictionary<IPropertyInfo, object>
				{
					{ pollEndDatePropertyChunk, DateTime.UtcNow.AddDays(2) },
					{ isActiveProperty, true }
				});
			(rule as IBusinessRule).Execute(context);

			context.Results.Count.Should().Be(0);
			context.OutputPropertyValues.Should().BeNull();

			pollEndDateProperty.Verify();
		}
	}
}
