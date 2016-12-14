using Autofac;
using Csla;
using FluentAssertions;
using Moq;
using MyVote.Data.Entities;
using Spackle.Extensions;
using Xunit;

namespace MyVote.BusinessObjects.Tests
{
	public sealed class CategoryTests
	{
		[Fact]
		public void Fetch()
		{
			var entity = EntityCreator.Create<Mvcategory>();

			var container = new ContainerBuilder();
			container.RegisterInstance(Mock.Of<IEntitiesContext>()).As<IEntitiesContext>();

			using (new ObjectActivator(container.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var category = DataPortal.FetchChild<Category>(entity);

				category.ID.Should().Be(entity.CategoryId);
				category.Name.Should().Be(entity.CategoryName);
			}
		}
	}
}
