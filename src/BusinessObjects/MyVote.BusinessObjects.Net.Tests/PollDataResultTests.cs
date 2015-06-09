using Autofac;
using Csla;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyVote.Core.Extensions;
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

			using (new ObjectActivator(new ContainerBuilder().Build()).Bind(() => ApplicationContext.DataPortalActivator))
			{
				var result = DataPortal.FetchChild<PollDataResult>(data);

				Assert.AreEqual(data.PollOptionID, result.PollOptionID, result.GetPropertyName(_ => _.PollOptionID));
				Assert.AreEqual(data.ResponseCount, result.ResponseCount, result.GetPropertyName(_ => _.ResponseCount));
			}
		}
	}
}
