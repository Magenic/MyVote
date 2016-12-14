using Csla.Core;
using Csla.Rules;
using FluentAssertions;
using Moq;
using MyVote.BusinessObjects.Rules;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace MyVote.BusinessObjects.Tests.Rules
{
	[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flag")]
	public sealed class PollSubmissionPollDeletedFlagRuleTests
	{
		[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flag")]
		[Fact]
		public void ExecuteWhenFlagIsNull()
		{
			var pollDeletedFlagProperty = new Mock<IPropertyInfo>(MockBehavior.Strict);
			pollDeletedFlagProperty.SetupGet(_ => _.Name).Returns("PollDeletedFlag");

			var isActiveProperty = Mock.Of<IPropertyInfo>();

			var rule = new PollSubmissionPollDeletedFlagRule(
				pollDeletedFlagProperty.Object, isActiveProperty);

			var context = new RuleContext(null, rule, null,
				new Dictionary<IPropertyInfo, object>
				{
					{ pollDeletedFlagProperty.Object, (null as int?) },
					{ isActiveProperty, true }
				});
			(rule as IBusinessRule).Execute(context);

			context.Results.Count.Should().Be(0);
			context.OutputPropertyValues.Should().BeNull();

			pollDeletedFlagProperty.VerifyAll();
		}

		[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flag")]
		[Fact]
		public void ExecuteWhenFlagIsNotNullAndFalse()
		{
			var pollDeletedFlagProperty = new Mock<IPropertyInfo>(MockBehavior.Strict);
			pollDeletedFlagProperty.Setup(_ => _.Name).Returns("PollDeletedFlag");

			var isActiveProperty = Mock.Of<IPropertyInfo>();

			var rule = new PollSubmissionPollDeletedFlagRule(
				pollDeletedFlagProperty.Object, isActiveProperty);

			var context = new RuleContext(null, rule, null,
				new Dictionary<IPropertyInfo, object>
				{
					{ pollDeletedFlagProperty.Object, false },
					{ isActiveProperty, true }
				});
			(rule as IBusinessRule).Execute(context);

			context.Results.Count.Should().Be(0);
			context.OutputPropertyValues.Should().BeNull();

			pollDeletedFlagProperty.VerifyAll();
		}

		[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flag")]
		[Fact]
		public void ExecuteWhenFlagIsNotNullAndTrue()
		{
			var pollDeletedFlagProperty = new Mock<IPropertyInfo>(MockBehavior.Strict);
			pollDeletedFlagProperty.Setup(_ => _.Name).Returns("PollDeletedFlag");

			var isActiveProperty = Mock.Of<IPropertyInfo>();

			var rule = new PollSubmissionPollDeletedFlagRule(
				pollDeletedFlagProperty.Object, isActiveProperty);

			var context = new RuleContext(null, rule, null,
				new Dictionary<IPropertyInfo, object>
				{
					{ pollDeletedFlagProperty.Object, true },
					{ isActiveProperty, true }
				});
			(rule as IBusinessRule).Execute(context);

			context.Results.Count.Should().Be(1);
			((bool)context.OutputPropertyValues[isActiveProperty]).Should().BeFalse();

			pollDeletedFlagProperty.VerifyAll();
		}
	}
}
