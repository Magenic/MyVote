using Autofac;
using Csla;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyVote.Data.Entities;
using Spackle.Extensions;

namespace MyVote.BusinessObjects.Net.Tests
{
	[TestClass]
	public sealed class PollSearchResultTests
	{
		[TestMethod]
		public void Fetch()
		{
			var container = new ContainerBuilder();
			container.RegisterInstance(Mock.Of<IEntities>()).As<IEntities>();

			var data = EntityCreator.Create<PollSearchResultsData>();

			using (new ObjectActivator(container.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var result = DataPortal.FetchChild<PollSearchResult>(data);

				Assert.AreEqual(data.Id, result.Id, nameof(result.Id));
				Assert.AreEqual(data.ImageLink, result.ImageLink, nameof(result.ImageLink));
				Assert.AreEqual(data.Question, result.Question, nameof(result.Question));
			}
		}
	}
}
