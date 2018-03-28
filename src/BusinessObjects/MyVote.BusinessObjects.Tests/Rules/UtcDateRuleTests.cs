using System;
using System.Collections.Generic;
using Csla.Core;
using Csla.Rules;
using MyVote.BusinessObjects.Rules;
using Xunit;
using FluentAssertions;
using Rocks;
using Rocks.Options;

namespace MyVote.BusinessObjects.Tests.Rules
{
	public sealed class UtcDateRuleTests
	{
		[Fact]
		public void ExecuteWithDateTime()
		{
			var property = Rock.Create<IPropertyInfo>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			property.Handle(nameof(IPropertyInfo.Name), () => "Name", 3);
			var propertyChunk = property.Make();

			var rule = new UtcDateRule(propertyChunk);

			var currentDate = DateTime.Now;

			var context = new RuleContext(null, rule, null,
				new Dictionary<IPropertyInfo, object> { { propertyChunk, currentDate } });
			(rule as IBusinessRule).Execute(context);

			((DateTime)context.OutputPropertyValues[propertyChunk])
				.Should().Be(currentDate.ToUniversalTime());
			property.Verify();
		}

		[Fact]
		public void ExecuteWithNullableDateTime()
		{
			var property = Rock.Create<IPropertyInfo>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			property.Handle(nameof(IPropertyInfo.Name), () => "Name", 3);
			var propertyChunk = property.Make();

			var rule = new UtcDateRule(propertyChunk);

			var currentDate = new DateTime?(DateTime.Now);

			var context = new RuleContext(null, rule, null,
				new Dictionary<IPropertyInfo, object> { { propertyChunk, currentDate } });
			(rule as IBusinessRule).Execute(context);

			((DateTime)context.OutputPropertyValues[propertyChunk])
				.Should().Be(currentDate.Value.ToUniversalTime());

			property.Verify();
		}
	}
}
