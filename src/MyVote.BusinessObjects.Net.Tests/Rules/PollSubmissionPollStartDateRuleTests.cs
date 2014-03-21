using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Csla.Core;
using Csla.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyVote.BusinessObjects.Rules;
using MyVote.Core.Extensions;

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
			var pollStartDateProperty = new Mock<IPropertyInfo>();
			pollStartDateProperty.Setup(_ => _.Name).Returns("PollStartDate");
			var isActiveProperty = new Mock<IPropertyInfo>();
			isActiveProperty.Setup(_ => _.Name).Returns("IsActive");

			var rule = new PollSubmissionPollStartDateRule(
				pollStartDateProperty.Object, isActiveProperty.Object);

			var context = new RuleContext(null, rule, null,
				new Dictionary<IPropertyInfo, object> 
				{ 
					{ pollStartDateProperty.Object, DateTime.UtcNow.AddDays(-2) },
					{ isActiveProperty.Object, true }
				});
			(rule as IBusinessRule).Execute(context);

			Assert.AreEqual(0, context.Results.Count, context.GetPropertyName(_ => _.Results));
			Assert.IsNull(context.OutputPropertyValues, context.GetPropertyName(_ => _.OutputPropertyValues));
		}

		[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flag")]
		[TestMethod]
		public void ExecuteWhenStartDateIsAfterNow()
		{
			var pollStartDateProperty = new Mock<IPropertyInfo>();
			pollStartDateProperty.Setup(_ => _.Name).Returns("PollStartDate");
			var isActiveProperty = new Mock<IPropertyInfo>();
			isActiveProperty.Setup(_ => _.Name).Returns("IsActive");

			var rule = new PollSubmissionPollStartDateRule(
				pollStartDateProperty.Object, isActiveProperty.Object);

			var context = new RuleContext(null, rule, null,
				new Dictionary<IPropertyInfo, object> 
				{ 
					{ pollStartDateProperty.Object, DateTime.UtcNow.AddDays(2) },
					{ isActiveProperty.Object, true }
				});
			(rule as IBusinessRule).Execute(context);

			Assert.AreEqual(1, context.Results.Count, context.GetPropertyName(_ => _.Results));
			Assert.IsFalse((bool)context.OutputPropertyValues[isActiveProperty.Object]);
		}
	}
}
