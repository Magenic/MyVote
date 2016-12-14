using Csla.Core;
using Csla.Rules;
using FluentAssertions;
using Moq;
using MyVote.BusinessObjects.Rules;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace MyVote.BusinessObjects.Net.Tests.Rules
{
	[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flag")]
	public sealed class PollSubmissionPollStartDateRuleTests
	{
		[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flag")]
		[Fact]
		public void ExecuteWhenStartDateIsBeforeNow()
		{
			var pollStartDateProperty = new Mock<IPropertyInfo>(MockBehavior.Strict);
			pollStartDateProperty.Setup(_ => _.Name).Returns("PollStartDate");

			var isActiveProperty = Mock.Of<IPropertyInfo>();

			var rule = new PollSubmissionPollStartDateRule(
				pollStartDateProperty.Object, isActiveProperty);

			var context = new RuleContext(null, rule, null,
				new Dictionary<IPropertyInfo, object>
				{
					{ pollStartDateProperty.Object, DateTime.UtcNow.AddDays(-2) },
					{ isActiveProperty, true }
				});
			(rule as IBusinessRule).Execute(context);

			context.Results.Count.Should().Be(0);
			context.OutputPropertyValues.Should().BeNull();

			pollStartDateProperty.VerifyAll();
		}

		[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flag")]
		[Fact]
		public void ExecuteWhenStartDateIsAfterNow()
		{
			var pollStartDateProperty = new Mock<IPropertyInfo>(MockBehavior.Strict);
			pollStartDateProperty.Setup(_ => _.Name).Returns("PollStartDate");

			var isActiveProperty = Mock.Of<IPropertyInfo>();

			var rule = new PollSubmissionPollStartDateRule(
				pollStartDateProperty.Object, isActiveProperty);

			var context = new RuleContext(null, rule, null,
				new Dictionary<IPropertyInfo, object>
				{
					{ pollStartDateProperty.Object, DateTime.UtcNow.AddDays(2) },
					{ isActiveProperty, true }
				});
			(rule as IBusinessRule).Execute(context);

			context.Results.Count.Should().Be(1);
			((bool)context.OutputPropertyValues[isActiveProperty]).Should().BeFalse();

			pollStartDateProperty.VerifyAll();
		}
	}
}
