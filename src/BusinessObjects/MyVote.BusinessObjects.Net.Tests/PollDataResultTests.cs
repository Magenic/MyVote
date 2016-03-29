using Autofac;
using Csla;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyVote.Data.Entities;
using Spackle.Extensions;

namespace MyVote.BusinessObjects.Net.Tests
{
	[TestClass]
	public sealed class PollDataResultTests
	{
		[TestMethod]
		public void Fetch()
		{
			var data = EntityCreator.Create<PollData>();

			var container = new ContainerBuilder();
			container.RegisterInstance(Mock.Of<IEntities>()).As<IEntities>();

			using (new ObjectActivator(container.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var result = DataPortal.FetchChild<PollDataResult>(data);

				Assert.AreEqual(data.PollOptionID, result.PollOptionID, nameof(result.PollOptionID));
				Assert.AreEqual(data.ResponseCount, result.ResponseCount, nameof(result.ResponseCount));
			}
		}
	}
}
