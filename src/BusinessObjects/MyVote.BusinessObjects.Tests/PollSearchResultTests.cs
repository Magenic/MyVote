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
	public sealed class PollSearchResultTests
	{
		[Fact]
		public void Fetch()
		{
			var container = new ContainerBuilder();
			container.RegisterInstance(Rock.Make<IEntitiesContext>(
				new RockOptions(allowWarnings: AllowWarnings.Yes))).As<IEntitiesContext>();

			var data = EntityCreator.Create<PollSearchResultsData>();

			using (new ObjectActivator(container.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var result = DataPortal.FetchChild<PollSearchResult>(data);

				result.Id.Should().Be(data.Id);
				result.ImageLink.Should().Be(data.ImageLink);
				result.Question.Should().Be(data.Question);
			}
		}
	}
}
