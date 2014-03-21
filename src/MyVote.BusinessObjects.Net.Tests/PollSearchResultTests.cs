using Autofac;
using Csla;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyVote.Core.Extensions;
using Spackle.Extensions;

namespace MyVote.BusinessObjects.Net.Tests
{
	[TestClass]
	public sealed class PollSearchResultTests
	{
		[TestMethod]
		public void Fetch()
		{
			var data = EntityCreator.Create<PollSearchResultsData>();

			using (new ObjectActivator(new ContainerBuilder().Build()).Bind(() => ApplicationContext.DataPortalActivator))
			{
				var result = DataPortal.FetchChild<PollSearchResult>(data);

				Assert.AreEqual(data.Id, result.Id, result.GetPropertyName(_ => _.Id));
				Assert.AreEqual(data.ImageLink, result.ImageLink, result.GetPropertyName(_ => _.ImageLink));
				Assert.AreEqual(data.Question, result.Question, result.GetPropertyName(_ => _.Question));
			}
		}
	}
}
