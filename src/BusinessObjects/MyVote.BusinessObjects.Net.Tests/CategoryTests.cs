using Autofac;
using Csla;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyVote.Data.Entities;
using Spackle.Extensions;

namespace MyVote.BusinessObjects.Net.Tests
{
	[TestClass]
	public sealed class CategoryTests
	{
		[TestMethod]
		public void Fetch()
		{
			var entity = EntityCreator.Create<MVCategory>();

			var container = new ContainerBuilder();
			container.RegisterInstance(Mock.Of<IEntities>()).As<IEntities>();

			using (new ObjectActivator(container.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var category = DataPortal.FetchChild<Category>(entity);

				Assert.AreEqual(entity.CategoryID, category.ID, nameof(category.ID));
				Assert.AreEqual(entity.CategoryName, category.Name, nameof(category.Name));
			}
		}
	}
}
