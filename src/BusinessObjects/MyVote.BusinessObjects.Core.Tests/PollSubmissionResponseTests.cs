using Autofac;
using Csla;
using FluentAssertions;
using Moq;
using MyVote.BusinessObjects.Contracts;
using MyVote.Data.Entities;
using Spackle;
using Spackle.Extensions;
using Xunit;

namespace MyVote.BusinessObjects.Tests
{
	public sealed class PollSubmissionResponseTests
	{
		[Fact]
		public void Create()
		{
			var generator = new RandomObjectGenerator();
			var optionId = generator.Generate<int>();
			var optionPosition = generator.Generate<short>();
			var optionText = generator.Generate<string>();

			var option = new Mock<IPollOption>(MockBehavior.Strict);
			option.Setup(_ => _.PollOptionID).Returns(optionId);
			option.Setup(_ => _.OptionPosition).Returns(optionPosition);
			option.Setup(_ => _.OptionText).Returns(optionText);

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => Mock.Of<IEntitiesContext>());

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var response = DataPortal.CreateChild<PollSubmissionResponse>(option.Object);

				response.PollOptionID.Should().Be(optionId);
				response.OptionPosition.Should().Be(optionPosition);
				response.OptionText.Should().Be(optionText);
				response.IsOptionSelected.Should().BeFalse();
				response.PollResponseID.Should().NotHaveValue();
			}

			option.VerifyAll();
		}
	}
}
