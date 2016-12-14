using Autofac;
using Csla;
using Moq;
using MyVote.BusinessObjects.Tests.Extensions;
using MyVote.Data.Entities;
using Spackle;
using Spackle.Extensions;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace MyVote.BusinessObjects.Tests
{
	public sealed class PollOptionRuleTests
	{
		[Fact]
		public void ChangeOptionPositionToValidValue()
		{
			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => Mock.Of<IEntitiesContext>());

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var generator = new RandomObjectGenerator();
				var optionPosition = generator.Generate<short>();

				var pollOption = DataPortal.CreateChild<PollOption>();
				pollOption.OptionPosition = optionPosition;

				pollOption.BrokenRulesCollection.AssertRuleCount(PollOption.OptionPositionProperty, 0);
			}
		}

		[Fact]
		public void ChangeOptionTextToInvalidValue()
		{
			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => Mock.Of<IEntitiesContext>());

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
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

		[Fact]
		public void ChangeOptionTextToValidValue()
		{
			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => Mock.Of<IEntitiesContext>());

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
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
