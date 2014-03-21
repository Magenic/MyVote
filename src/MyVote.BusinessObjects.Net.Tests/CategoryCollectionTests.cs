using Autofac;
using Csla;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyVote.Core.Extensions;
using MyVote.Repository;
using Spackle.Extensions;

namespace MyVote.BusinessObjects.Net.Tests
{
	[TestClass]
	public sealed class CategoryCollectionTests
	{
		[TestMethod]
		public void Fetch()
		{
			var entity = EntityCreator.Create<MVCategory>();

			var context = new Mock<IEntities>();
			context.Setup(_ => _.MVCategories).Returns(new InMemoryDbSet<MVCategory> { entity });

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => context.Object);

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
			{
				var categories = DataPortal.Fetch<CategoryCollection>();
				Assert.AreEqual(1, categories.Count, categories.GetPropertyName(_ => _.Count));
			}
		}
	}
}
