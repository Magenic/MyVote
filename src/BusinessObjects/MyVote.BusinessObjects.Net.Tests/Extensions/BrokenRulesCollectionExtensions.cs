using System.ComponentModel.DataAnnotations;
using System.Linq;
using Csla.Core;
using Csla.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MyVote.BusinessObjects.Net.Tests.Extensions
{
	public static class BrokenRulesCollectionExtensions
	{
		public static void AssertRuleCount(this BrokenRulesCollection @this, int expectedRuleCount)
		{
			Assert.AreEqual(expectedRuleCount, @this.Count);
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
				string.Format(
					"Rule count was unexpected for property {0}", property.FriendlyName) :
				string.Format(
					"Rule count was unexpected for property {0} : {1}", property.FriendlyName, message);
			Assert.AreEqual(expectedRuleCount, ruleCount, assertMessage);
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
				Assert.IsNotNull(foundRule, string.Format(
					"Property {0} does not contain rule {1}", property.Name, ruleTypeName));
			}
			else
			{
				Assert.IsNull(foundRule, string.Format(
					"Property {0} contains rule {1}", property.Name, ruleTypeName));
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
				Assert.IsNotNull(foundRule, string.Format(
					"Property {0} does not contain rule {1}", property.FriendlyName, ruleTypeName));
			}
			else
			{
				Assert.IsNull(foundRule, string.Format(
					"Property {0} contains rule {1}", property.FriendlyName, ruleTypeName));
			}
		}
	}
}
