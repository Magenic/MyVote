using Autofac;
using Csla;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;
using MyVote.Data.Entities;
using Spackle.Extensions;
using System.Collections.Generic;

namespace MyVote.BusinessObjects.Net.Tests
{
	[TestClass]
	public sealed class PollSearchResultsByCategoryTests
	{
		[TestMethod]
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

				Assert.AreEqual(data[0].Category, result.Category, nameof(result.Category));
				Assert.AreEqual(1, result.SearchResults.Count, nameof(result.SearchResults));
			}

			pollSearchResultsFactory.VerifyAll();
			pollSearchResultFactory.VerifyAll();
		}
	}
}
