using Autofac;
using Csla;
using FluentAssertions;
using Moq;
using MyVote.BusinessObjects.Contracts;
using MyVote.Data.Entities;
using Spackle.Extensions;
using Xunit;

namespace MyVote.BusinessObjects.Net.Tests
{
	public sealed class CategoryCollectionTests
	{
		[Fact]
		public void Fetch()
		{
			var entity = EntityCreator.Create<MVCategory>();

			var context = new Mock<IEntities>(MockBehavior.Strict);
			context.Setup(_ => _.MVCategories).Returns(new InMemoryDbSet<MVCategory> { entity });
			context.Setup(_ => _.Dispose());

			var categoryFactory = new Mock<IObjectFactory<ICategory>>(MockBehavior.Strict);
			categoryFactory.Setup(_ => _.FetchChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.FetchChild<Category>(_[0] as MVCategory));

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => context.Object);
			builder.Register<IObjectFactory<ICategory>>(_ => categoryFactory.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var categories = DataPortal.Fetch<CategoryCollection>();
				categories.Count.Should().Be(1);
			}

			context.VerifyAll();
			categoryFactory.VerifyAll();
		}
	}
}
