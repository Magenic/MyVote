using Autofac;
using Csla;
using FluentAssertions;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;
using MyVote.Data.Entities;
using Rocks;
using Rocks.Options;
using Spackle.Extensions;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace MyVote.BusinessObjects.Tests
{
	public sealed class PollSearchResultsByCategoryTests
	{
		[Fact]
		public void Fetch()
		{
			var data = new List<PollSearchResultsData> { EntityCreator.Create<PollSearchResultsData>() };

			var pollSearchResultsFactory = Rock.Create<IObjectFactory<ReadOnlySwitchList<IPollSearchResult>>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollSearchResultsFactory.Handle(_ => _.FetchChild())
				.Returns(DataPortal.FetchChild<ReadOnlySwitchList<IPollSearchResult>>());

			var pollSearchResultFactory = Rock.Create<IObjectFactory<IPollSearchResult>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollSearchResultFactory.Handle<object[], IPollSearchResult>(_ => _.FetchChild(Arg.IsAny<object[]>()),
				arg => DataPortal.FetchChild<PollSearchResult>(arg[0] as PollSearchResultsData));

			var builder = new ContainerBuilder();
			builder.RegisterInstance(Rock.Make<IEntitiesContext>(
				new RockOptions(allowWarnings: AllowWarnings.Yes))).As<IEntitiesContext>();
			builder.Register<IObjectFactory<ReadOnlySwitchList<IPollSearchResult>>>(_ => pollSearchResultsFactory.Make());
			builder.Register<IObjectFactory<IPollSearchResult>>(_ => pollSearchResultFactory.Make());

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var result = DataPortal.FetchChild<PollSearchResultsByCategory>(data);

				result.Category.Should().Be(data[0].Category);
				(result.SearchResults as ICollection).Count.Should().Be(1);
			}

			pollSearchResultsFactory.Verify();
			pollSearchResultFactory.Verify();
		}
	}
}