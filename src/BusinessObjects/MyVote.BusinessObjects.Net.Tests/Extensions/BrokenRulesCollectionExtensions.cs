using System.ComponentModel.DataAnnotations;
using System.Linq;
using Csla.Core;
using Csla.Rules;
using FluentAssertions;

namespace MyVote.BusinessObjects.Net.Tests.Extensions
{
	public static class BrokenRulesCollectionExtensions
	{
		public static void AssertRuleCount(this BrokenRulesCollection @this, int expectedRuleCount)
		{
			@this.Count.Should().Be(expectedRuleCount);
		}

		public static void AssertRuleCount(this BrokenRulesCollection @this, IPropertyInfo property, int expectedRuleCount)
		{
			@this.AssertRuleCount(property, expectedRuleCount, string.Empty);
		}

		public static void AssertRuleCount(this BrokenRulesCollection @this, IPropertyInfo property, int expectedRuleCount, string message)
		{
			var ruleCount =
				(from brokenRule in @this
				 where brokenRule.Property == property.FriendlyName
				 select brokenRule).Count();
			var assertMessage = string.IsNullOrWhiteSpace(message) ?
				$"rule count was unexpected for property {property.FriendlyName}" :
				$"rule count was unexpected for property {property.FriendlyName} : {message}";
			ruleCount.Should().Be(expectedRuleCount, assertMessage);
		}

		public static void AssertValidationRuleExists<T>(this BrokenRulesCollection @this, IPropertyInfo property, bool shouldExist)
			where T : ValidationAttribute
		{
			var ruleTypeName = typeof(T).Name;

			var foundRule =
				(from brokenRule in @this
				 where (brokenRule.Property == property.Name &&
				 brokenRule.RuleName.Replace("/", string.Empty).ToLower().Contains(ruleTypeName.ToLower()))
				 select brokenRule).SingleOrDefault();

			if (shouldExist)
			{
				foundRule.Should().NotBeNull($"property {property.Name} does not contain rule {ruleTypeName}");
			}
			else
			{
				foundRule.Should().BeNull($"property {property.Name} contains rule {ruleTypeName}");
			}
		}

		public static void AssertBusinessRuleExists<T>(this BrokenRulesCollection @this, IPropertyInfo property, bool shouldExist)
			where T : IBusinessRule
		{
			var ruleTypeName = typeof(T).FullName;

			var foundRule =
				(from brokenRule in @this
				 where (brokenRule.Property == property.FriendlyName &&
				 brokenRule.RuleName == new RuleUri(ruleTypeName, property.Name).ToString())
				 select brokenRule).SingleOrDefault();

			if (shouldExist)
			{
				foundRule.Should().NotBeNull($"property {property.FriendlyName} does not contain rule {ruleTypeName}");
			}
			else
			{
				foundRule.Should().BeNull($"property {property.FriendlyName} contains rule {ruleTypeName}");
			}
		}
	}
}
