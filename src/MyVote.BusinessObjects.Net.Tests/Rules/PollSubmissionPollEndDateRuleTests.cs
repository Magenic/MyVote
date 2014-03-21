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
	public sealed class PollSubmissionPollEndDateRuleTests
	{
		[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flag")]
		[TestMethod]
		public void ExecuteWhenEndDateIsBeforeNow()
		{
			var pollEndDateProperty = new Mock<IPropertyInfo>();
			pollEndDateProperty.Setup(_ => _.Name).Returns("PollEndDate");
			var isActiveProperty = new Mock<IPropertyInfo>();
			isActiveProperty.Setup(_ => _.Name).Returns("IsActive");

			var rule = new PollSubmissionPollEndDateRule(
				pollEndDateProperty.Object, isActiveProperty.Object);

			var context = new RuleContext(null, rule, null,
				new Dictionary<IPropertyInfo, object> 
				{ 
					{ pollEndDateProperty.Object, DateTime.UtcNow.AddDays(-2) },
					{ isActiveProperty.Object, true }
				});
			(rule as IBusinessRule).Execute(context);

			Assert.AreEqual(1, context.Results.Count, context.GetPropertyName(_ => _.Results));
			Assert.IsFalse((bool)context.OutputPropertyValues[isActiveProperty.Object]);
		}

		[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flag")]
		[TestMethod]
		public void ExecuteWhenEndDateIsAfterNow()
		{
			var pollEndDateProperty = new Mock<IPropertyInfo>();
			pollEndDateProperty.Setup(_ => _.Name).Returns("PollEndDate");
			var isActiveProperty = new Mock<IPropertyInfo>();
			isActiveProperty.Setup(_ => _.Name).Returns("IsActive");

			var rule = new PollSubmissionPollEndDateRule(
				pollEndDateProperty.Object, isActiveProperty.Object);

			var context = new RuleContext(null, rule, null,
				new Dictionary<IPropertyInfo, object> 
				{ 
					{ pollEndDateProperty.Object, DateTime.UtcNow.AddDays(2) },
					{ isActiveProperty.Object, true }
				});
			(rule as IBusinessRule).Execute(context);

			Assert.AreEqual(0, context.Results.Count, context.GetPropertyName(_ => _.Results));
			Assert.IsNull(context.OutputPropertyValues, context.GetPropertyName(_ => _.OutputPropertyValues));
		}
	}
}
