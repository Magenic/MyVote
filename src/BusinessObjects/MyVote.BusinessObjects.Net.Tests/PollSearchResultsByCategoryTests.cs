using System.Collections.Generic;
using Autofac;
using Csla;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyVote.Core.Extensions;
using Spackle.Extensions;

namespace MyVote.BusinessObjects.Net.Tests
{
	[TestClass]
	public sealed class PollSearchResultsByCategoryTests
	{
		[TestMethod]
		public void Fetch()
		{
			var data = new List<PollSearchResultsData> { EntityCreator.Create<PollSearchResultsData>() };

			using (new ObjectActivator(new ContainerBuilder().Build()).Bind(() => ApplicationContext.DataPortalActivator))
			{
				var result = DataPortal.FetchChild<PollSearchResultsByCategory>(data);

				Assert.AreEqual(data[0].Category, result.Category, result.GetPropertyName(_ => _.Category));
				Assert.AreEqual(1, result.SearchResults.Count, result.GetPropertyName(_ => _.SearchResults));
			}
		}
	}
}
