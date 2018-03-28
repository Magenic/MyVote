using Autofac;
using Csla.Serialization.Mobile;
using FluentAssertions;
using MyVote.BusinessObjects.Contracts;
using Xunit;

namespace MyVote.BusinessObjects.Tests
{
	public sealed class BusinessObjectsModuleTests
	{
		[Fact]
		public void Load()
		{
			var module = new BusinessObjectsModule();
			var builder = new ContainerBuilder();
			builder.RegisterModule(module);

			var container = builder.Build();
			var factory = container.Resolve<IObjectFactory<IMobileObject>>();

			factory.Should().BeOfType<ObjectFactory<IMobileObject>>();
		}
	}
}
