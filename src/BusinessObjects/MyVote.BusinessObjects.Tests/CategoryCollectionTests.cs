using Autofac;
using Csla;
using FluentAssertions;
using MyVote.BusinessObjects.Contracts;
using MyVote.Data.Entities;
using Rocks;
using Rocks.Options;
using Spackle.Extensions;
using Xunit;

namespace MyVote.BusinessObjects.Tests
{
	public sealed class CategoryCollectionTests
	{
		[Fact]
		public void Fetch()
		{
			var entity = EntityCreator.Create<Mvcategory>();

			var context = Rock.Create<IEntitiesContext>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			context.Handle(nameof(IEntitiesContext.Mvcategory), () => new InMemoryDbSet<Mvcategory> { entity });
			context.Handle(_ => _.Dispose());

			var categoryFactory = Rock.Create<IObjectFactory<ICategory>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			categoryFactory.Handle<object[], ICategory>(_ => _.FetchChild(Arg.IsAny<object[]>()),
				data => DataPortal.FetchChild<Category>(data[0] as Mvcategory));

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => context.Make());
			builder.Register<IObjectFactory<ICategory>>(_ => categoryFactory.Make());

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var categories = DataPortal.Fetch<CategoryCollection>();
				categories.Count.Should().Be(1);
			}

			context.Verify();
			categoryFactory.Verify();
		}
	}
}
