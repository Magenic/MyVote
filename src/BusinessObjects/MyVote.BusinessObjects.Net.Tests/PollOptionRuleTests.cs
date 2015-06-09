using System.ComponentModel.DataAnnotations;
using Autofac;
using Csla;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyVote.BusinessObjects.Net.Tests.Extensions;
using MyVote.Data.Entities;
using Spackle;
using Spackle.Extensions;

namespace MyVote.BusinessObjects.Net.Tests
{
	[TestClass]
	public sealed class PollOptionRuleTests
	{
		[TestMethod]
		public void ChangeOptionPositionToValidValue()
		{
			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
			{
				var generator = new RandomObjectGenerator();
				var optionPosition = generator.Generate<short>();

				var pollOption = DataPortal.CreateChild<PollOption>();
				pollOption.OptionPosition = optionPosition;

				pollOption.BrokenRulesCollection.AssertRuleCount(PollOption.OptionPositionProperty, 0);
			}
		}

		[TestMethod]
		public void ChangeOptionTextToInvalidValue()
		{
			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
			{
				var generator = new RandomObjectGenerator();
				var optionPosition = new string(generator.Generate<string>()[0], 1001);

				var pollOption = DataPortal.CreateChild<PollOption>();
				pollOption.OptionText = optionPosition;

				pollOption.BrokenRulesCollection.AssertRuleCount(PollOption.OptionTextProperty, 1);
				pollOption.BrokenRulesCollection.AssertValidationRuleExists<StringLengthAttribute>(
					PollOption.OptionTextProperty, true);
			}
		}

		[TestMethod]
		public void ChangeOptionTextToValidValue()
		{
			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
			{
				var generator = new RandomObjectGenerator();
				var optionPosition = new string(generator.Generate<string>()[0], 1);

				var pollOption = DataPortal.CreateChild<PollOption>();
				pollOption.OptionText = optionPosition;

				pollOption.BrokenRulesCollection.AssertRuleCount(PollOption.OptionTextProperty, 0);
			}
		}
	}
}
