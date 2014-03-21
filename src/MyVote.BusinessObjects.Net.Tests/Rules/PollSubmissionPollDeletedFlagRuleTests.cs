using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Principal;
using Csla;
using Csla.Core;
using Csla.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Rules;
using MyVote.Core.Extensions;
using Spackle;
using Spackle.Extensions;

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
			var pollDeletedFlagProperty = new Mock<IPropertyInfo>();
			pollDeletedFlagProperty.Setup(_ => _.Name).Returns("PollDeletedFlag");
			var isActiveProperty = new Mock<IPropertyInfo>();
			isActiveProperty.Setup(_ => _.Name).Returns("IsActive");

			var rule = new PollSubmissionPollDeletedFlagRule(
				pollDeletedFlagProperty.Object, isActiveProperty.Object);

			var context = new RuleContext(null, rule, null,
				new Dictionary<IPropertyInfo, object> 
				{ 
					{ pollDeletedFlagProperty.Object, (null as int?) },
					{ isActiveProperty.Object, true }
				});
			(rule as IBusinessRule).Execute(context);

			Assert.AreEqual(0, context.Results.Count, context.GetPropertyName(_ => _.Results));
			Assert.IsNull(context.OutputPropertyValues);
		}

		[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flag")]
		[TestMethod]
		public void ExecuteWhenFlagIsNotNullAndFalse()
		{
			var pollDeletedFlagProperty = new Mock<IPropertyInfo>();
			pollDeletedFlagProperty.Setup(_ => _.Name).Returns("PollDeletedFlag");
			var isActiveProperty = new Mock<IPropertyInfo>();
			isActiveProperty.Setup(_ => _.Name).Returns("IsActive");

			var rule = new PollSubmissionPollDeletedFlagRule(
				pollDeletedFlagProperty.Object, isActiveProperty.Object);

			var context = new RuleContext(null, rule, null,
				new Dictionary<IPropertyInfo, object> 
				{ 
					{ pollDeletedFlagProperty.Object, false },
					{ isActiveProperty.Object, true }
				});
			(rule as IBusinessRule).Execute(context);

			Assert.AreEqual(0, context.Results.Count, context.GetPropertyName(_ => _.Results));
			Assert.IsNull(context.OutputPropertyValues);
		}

		[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flag")]
		[TestMethod]
		public void ExecuteWhenFlagIsNotNullAndTrue()
		{
			var pollDeletedFlagProperty = new Mock<IPropertyInfo>();
			pollDeletedFlagProperty.Setup(_ => _.Name).Returns("PollDeletedFlag");
			var isActiveProperty = new Mock<IPropertyInfo>();
			isActiveProperty.Setup(_ => _.Name).Returns("IsActive");

			var rule = new PollSubmissionPollDeletedFlagRule(
				pollDeletedFlagProperty.Object, isActiveProperty.Object);

			var context = new RuleContext(null, rule, null,
				new Dictionary<IPropertyInfo, object> 
				{ 
					{ pollDeletedFlagProperty.Object, true },
					{ isActiveProperty.Object, true }
				});
			(rule as IBusinessRule).Execute(context);

			Assert.AreEqual(1, context.Results.Count, context.GetPropertyName(_ => _.Results));
			Assert.IsFalse((bool)context.OutputPropertyValues[isActiveProperty.Object]);
		}
	}
}
