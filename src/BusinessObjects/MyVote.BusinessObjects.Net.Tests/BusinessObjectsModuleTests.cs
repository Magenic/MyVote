using Autofac;
using Csla.Serialization.Mobile;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyVote.BusinessObjects.Contracts;

namespace MyVote.BusinessObjects.Net.Tests
{
	[TestClass]
	public sealed class BusinessObjectsModuleTests
	{
		[TestMethod]
		public void Load()
		{
			var module = new BusinessObjectsModule();
			var builder = new ContainerBuilder();
			builder.RegisterModule(module);

			var container = builder.Build();
			var factory = container.Resolve<IObjectFactory<IMobileObject>>();

			Assert.AreEqual(typeof(ObjectFactory<IMobileObject>), factory.GetType());
		}
	}
}
