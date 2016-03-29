using Csla.Core;
using Csla.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyVote.BusinessObjects.Rules;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MyVote.BusinessObjects.Net.Tests.Rules
{
	[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flag")]
	[TestClass]
	public sealed class PollSubmissionPollStartDateRuleTests
	{
		[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flag")]
		[TestMethod]
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

			Assert.AreEqual(0, context.Results.Count, nameof(context.Results));
			Assert.IsNull(context.OutputPropertyValues, nameof(context.OutputPropertyValues));

			pollStartDateProperty.VerifyAll();
		}

		[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flag")]
		[TestMethod]
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

			Assert.AreEqual(1, context.Results.Count, nameof(context.Results));
			Assert.IsFalse((bool)context.OutputPropertyValues[isActiveProperty]);

			pollStartDateProperty.VerifyAll();
		}
	}
}
