using System;
using System.Collections.Generic;
using Csla.Core;
using Csla.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyVote.BusinessObjects.Rules;

namespace MyVote.BusinessObjects.Net.Tests.Rules
{
	[TestClass]
	public sealed class UtcDateRuleTests
	{
		[TestMethod]
		public void ExecuteWithDateTime()
		{
			var property = new Mock<IPropertyInfo>();
			property.Setup(_ => _.Name).Returns("Name");

			var rule = new UtcDateRule(property.Object);

			var currentDate = DateTime.Now;

			var context = new RuleContext(null, rule, null,
				new Dictionary<IPropertyInfo, object> { { property.Object, currentDate } });
			(rule as IBusinessRule).Execute(context);
			Assert.AreEqual(currentDate.ToUniversalTime(),
				(DateTime)context.OutputPropertyValues[property.Object]);
		}

		[TestMethod]
		public void ExecuteWithNullableDateTime()
		{
			var property = new Mock<IPropertyInfo>();
			property.Setup(_ => _.Name).Returns("Name");

			var rule = new UtcDateRule(property.Object);

			var currentDate = new DateTime?(DateTime.Now);

			var context = new RuleContext(null, rule, null,
				new Dictionary<IPropertyInfo, object> { { property.Object, currentDate } });
			(rule as IBusinessRule).Execute(context);
			Assert.AreEqual(currentDate.Value.ToUniversalTime(),
				(DateTime)context.OutputPropertyValues[property.Object]);
		}
	}
}
