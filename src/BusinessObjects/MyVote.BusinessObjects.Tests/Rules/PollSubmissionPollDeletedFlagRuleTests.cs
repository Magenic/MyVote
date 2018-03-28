using Csla.Core;
using Csla.Rules;
using FluentAssertions;
using MyVote.BusinessObjects.Rules;
using Rocks;
using Rocks.Options;
using System.Collections.Generic;
using Xunit;

namespace MyVote.BusinessObjects.Tests.Rules
{
	public sealed class PollSubmissionPollDeletedFlagRuleTests
	{
		[Fact]
		public void ExecuteWhenFlagIsNull()
		{
			var pollDeletedFlagProperty = Rock.Create<IPropertyInfo>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollDeletedFlagProperty.Handle(nameof(IPropertyInfo.Name), () => "PollDeletedFlag", 3);
			var pollDeletedFlagPropertyChunk = pollDeletedFlagProperty.Make();

			var isActiveProperty = Rock.Make<IPropertyInfo>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));

			var rule = new PollSubmissionPollDeletedFlagRule(
				pollDeletedFlagPropertyChunk, isActiveProperty);

			var context = new RuleContext(null, rule, null,
				new Dictionary<IPropertyInfo, object>
				{
					{ pollDeletedFlagPropertyChunk, (null as int?) },
					{ isActiveProperty, true }
				});
			(rule as IBusinessRule).Execute(context);

			context.Results.Count.Should().Be(0);
			context.OutputPropertyValues.Should().BeNull();

			pollDeletedFlagProperty.Verify();
		}

		[Fact]
		public void ExecuteWhenFlagIsNotNullAndFalse()
		{
			var pollDeletedFlagProperty = Rock.Create<IPropertyInfo>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollDeletedFlagProperty.Handle(nameof(IPropertyInfo.Name), () => "PollDeletedFlag", 3);
			var pollDeletedFlagPropertyChunk = pollDeletedFlagProperty.Make();

			var isActiveProperty = Rock.Make<IPropertyInfo>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));

			var rule = new PollSubmissionPollDeletedFlagRule(
				pollDeletedFlagPropertyChunk, isActiveProperty);

			var context = new RuleContext(null, rule, null,
				new Dictionary<IPropertyInfo, object>
				{
					{ pollDeletedFlagPropertyChunk, false },
					{ isActiveProperty, true }
				});
			(rule as IBusinessRule).Execute(context);

			context.Results.Count.Should().Be(0);
			context.OutputPropertyValues.Should().BeNull();

			pollDeletedFlagProperty.Verify();
		}

		[Fact]
		public void ExecuteWhenFlagIsNotNullAndTrue()
		{
			var pollDeletedFlagProperty = Rock.Create<IPropertyInfo>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollDeletedFlagProperty.Handle(nameof(IPropertyInfo.Name), () => "PollDeletedFlag", 3);
			var pollDeletedFlagPropertyChunk = pollDeletedFlagProperty.Make();

			var isActiveProperty = Rock.Make<IPropertyInfo>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));

			var rule = new PollSubmissionPollDeletedFlagRule(
				pollDeletedFlagPropertyChunk, isActiveProperty);

			var context = new RuleContext(null, rule, null,
				new Dictionary<IPropertyInfo, object>
				{
					{ pollDeletedFlagPropertyChunk, true },
					{ isActiveProperty, true }
				});
			(rule as IBusinessRule).Execute(context);

			context.Results.Count.Should().Be(1);
			((bool)context.OutputPropertyValues[isActiveProperty]).Should().BeFalse();

			pollDeletedFlagProperty.Verify();
		}
	}
}