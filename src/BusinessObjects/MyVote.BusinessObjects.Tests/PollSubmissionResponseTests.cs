using Autofac;
using Csla;
using FluentAssertions;
using MyVote.BusinessObjects.Contracts;
using MyVote.Data.Entities;
using Rocks;
using Rocks.Options;
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

			var option = Rock.Create<IPollOption>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			option.Handle(nameof(IPollOption.PollOptionID), () => optionId as int?);
			option.Handle(nameof(IPollOption.OptionPosition), () => optionPosition as short?);
			option.Handle(nameof(IPollOption.OptionText), () => optionText);

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => Rock.Make<IEntitiesContext>(
				new RockOptions(allowWarnings: AllowWarnings.Yes)));

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var response = DataPortal.CreateChild<PollSubmissionResponse>(option.Make());

				response.PollOptionID.Should().Be(optionId);
				response.OptionPosition.Should().Be(optionPosition);
				response.OptionText.Should().Be(optionText);
				response.IsOptionSelected.Should().BeFalse();
				response.PollResponseID.Should().NotHaveValue();
			}

			option.Verify();
		}
	}
}
