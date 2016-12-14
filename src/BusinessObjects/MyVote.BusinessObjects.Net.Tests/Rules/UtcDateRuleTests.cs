using System;
using System.Collections.Generic;
using Csla.Core;
using Csla.Rules;
using Moq;
using MyVote.BusinessObjects.Rules;
using Xunit;
using FluentAssertions;

namespace MyVote.BusinessObjects.Net.Tests.Rules
{
	public sealed class UtcDateRuleTests
	{
		[Fact]
		public void ExecuteWithDateTime()
		{
			var property = new Mock<IPropertyInfo>(MockBehavior.Strict);
			property.Setup(_ => _.Name).Returns("Name");

			var rule = new UtcDateRule(property.Object);

			var currentDate = DateTime.Now;

			var context = new RuleContext(null, rule, null,
				new Dictionary<IPropertyInfo, object> { { property.Object, currentDate } });
			(rule as IBusinessRule).Execute(context);

			((DateTime)context.OutputPropertyValues[property.Object])
				.Should().Be(currentDate.ToUniversalTime());
			property.VerifyAll();
		}

		[Fact]
		public void ExecuteWithNullableDateTime()
		{
			var property = new Mock<IPropertyInfo>(MockBehavior.Strict);
			property.Setup(_ => _.Name).Returns("Name");

			var rule = new UtcDateRule(property.Object);

			var currentDate = new DateTime?(DateTime.Now);

			var context = new RuleContext(null, rule, null,
				new Dictionary<IPropertyInfo, object> { { property.Object, currentDate } });
			(rule as IBusinessRule).Execute(context);

			((DateTime)context.OutputPropertyValues[property.Object])
				.Should().Be(currentDate.Value.ToUniversalTime());

			property.VerifyAll();
		}
	}
}
