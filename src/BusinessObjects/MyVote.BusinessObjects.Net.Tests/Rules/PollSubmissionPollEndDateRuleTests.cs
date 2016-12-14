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
	public sealed class PollSubmissionPollEndDateRuleTests
	{
		[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flag")]
		[Fact]
		public void ExecuteWhenEndDateIsBeforeNow()
		{
			var pollEndDateProperty = new Mock<IPropertyInfo>(MockBehavior.Strict);
			pollEndDateProperty.Setup(_ => _.Name).Returns("PollEndDate");

			var isActiveProperty = Mock.Of<IPropertyInfo>();

			var rule = new PollSubmissionPollEndDateRule(
				pollEndDateProperty.Object, isActiveProperty);

			var context = new RuleContext(null, rule, null,
				new Dictionary<IPropertyInfo, object>
				{
					{ pollEndDateProperty.Object, DateTime.UtcNow.AddDays(-2) },
					{ isActiveProperty, true }
				});
			(rule as IBusinessRule).Execute(context);

			context.Results.Count.Should().Be(1);
			((bool)context.OutputPropertyValues[isActiveProperty]).Should().BeFalse();

			pollEndDateProperty.VerifyAll();
		}

		[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flag")]
		[Fact]
		public void ExecuteWhenEndDateIsAfterNow()
		{
			var pollEndDateProperty = new Mock<IPropertyInfo>(MockBehavior.Strict);
			pollEndDateProperty.Setup(_ => _.Name).Returns("PollEndDate");

			var isActiveProperty = Mock.Of<IPropertyInfo>();

			var rule = new PollSubmissionPollEndDateRule(
				pollEndDateProperty.Object, isActiveProperty);

			var context = new RuleContext(null, rule, null,
				new Dictionary<IPropertyInfo, object>
				{
					{ pollEndDateProperty.Object, DateTime.UtcNow.AddDays(2) },
					{ isActiveProperty, true }
				});
			(rule as IBusinessRule).Execute(context);

			context.Results.Count.Should().Be(0);
			context.OutputPropertyValues.Should().BeNull();

			pollEndDateProperty.VerifyAll();
		}
	}
}
