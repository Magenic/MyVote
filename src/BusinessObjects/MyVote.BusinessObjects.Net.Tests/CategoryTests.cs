using Autofac;
using Csla;
using FluentAssertions;
using Moq;
using MyVote.Data.Entities;
using Spackle.Extensions;
using Xunit;

namespace MyVote.BusinessObjects.Net.Tests
{
	public sealed class CategoryTests
	{
		[Fact]
		public void Fetch()
		{
			var entity = EntityCreator.Create<MVCategory>();

			var container = new ContainerBuilder();
			container.RegisterInstance(Mock.Of<IEntities>()).As<IEntities>();

			using (new ObjectActivator(container.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var category = DataPortal.FetchChild<Category>(entity);

				category.ID.Should().Be(entity.CategoryID);
				category.Name.Should().Be(entity.CategoryName);
			}
		}
	}
}
