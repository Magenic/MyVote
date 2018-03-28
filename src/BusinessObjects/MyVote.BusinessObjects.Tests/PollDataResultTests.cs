using Autofac;
using Csla;
using FluentAssertions;
using MyVote.Data.Entities;
using Rocks;
using Rocks.Options;
using Spackle.Extensions;
using Xunit;

namespace MyVote.BusinessObjects.Tests
{
	public sealed class PollDataResultTests
	{
		[Fact]
		public void Fetch()
		{
			var data = EntityCreator.Create<PollData>();

			var container = new ContainerBuilder();
			container.RegisterInstance(Rock.Make<IEntitiesContext>(
				new RockOptions(allowWarnings: AllowWarnings.Yes))).As<IEntitiesContext>();

			using (new ObjectActivator(container.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var result = DataPortal.FetchChild<PollDataResult>(data);

				result.PollOptionID.Should().Be(data.PollOptionID);
				result.ResponseCount.Should().Be(data.ResponseCount);
			}
		}
	}
}
