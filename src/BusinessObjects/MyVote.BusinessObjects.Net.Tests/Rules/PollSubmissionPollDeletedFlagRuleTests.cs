using Csla.Core;
using Csla.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyVote.BusinessObjects.Rules;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MyVote.BusinessObjects.Net.Tests.Rules
{
	[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flag")]
	[TestClass]
	public sealed class PollSubmissionPollDeletedFlagRuleTests
	{
		[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flag")]
		[TestMethod]
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

			Assert.AreEqual(0, context.Results.Count, nameof(context.Results));
			Assert.IsNull(context.OutputPropertyValues);

			pollDeletedFlagProperty.VerifyAll();
		}

		[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flag")]
		[TestMethod]
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

			Assert.AreEqual(0, context.Results.Count, nameof(context.Results));
			Assert.IsNull(context.OutputPropertyValues);

			pollDeletedFlagProperty.VerifyAll();
		}

		[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flag")]
		[TestMethod]
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

			Assert.AreEqual(1, context.Results.Count, nameof(context.Results));
			Assert.IsFalse((bool)context.OutputPropertyValues[isActiveProperty]);

			pollDeletedFlagProperty.VerifyAll();
		}
	}
}
