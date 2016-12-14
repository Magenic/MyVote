using Autofac;
using Csla;
using FluentAssertions;
using Moq;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;
using MyVote.Data.Entities;
using Spackle.Extensions;
using System.Collections.Generic;
using Xunit;

namespace MyVote.BusinessObjects.Net.Tests
{
	public sealed class PollSearchResultsByCategoryTests
	{
		[Fact]
		public void Fetch()
		{
			var data = new List<PollSearchResultsData> { EntityCreator.Create<PollSearchResultsData>() };

			var pollSearchResultsFactory = new Mock<IObjectFactory<ReadOnlySwitchList<IPollSearchResult>>>(MockBehavior.Strict);
			pollSearchResultsFactory.Setup(_ => _.FetchChild()).Returns(DataPortal.FetchChild<ReadOnlySwitchList<IPollSearchResult>>());

			var pollSearchResultFactory = new Mock<IObjectFactory<IPollSearchResult>>(MockBehavior.Strict);
			pollSearchResultFactory.Setup(_ => _.FetchChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.FetchChild<PollSearchResult>(_[0] as PollSearchResultsData));

			var builder = new ContainerBuilder();
			builder.RegisterInstance(Mock.Of<IEntities>()).As<IEntities>();
			builder.Register<IObjectFactory<ReadOnlySwitchList<IPollSearchResult>>>(_ => pollSearchResultsFactory.Object);
			builder.Register<IObjectFactory<IPollSearchResult>>(_ => pollSearchResultFactory.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var result = DataPortal.FetchChild<PollSearchResultsByCategory>(data);

				result.Category.Should().Be(data[0].Category);
				result.SearchResults.Count.Should().Be(1);
			}

			pollSearchResultsFactory.VerifyAll();
			pollSearchResultFactory.VerifyAll();
		}
	}
}
