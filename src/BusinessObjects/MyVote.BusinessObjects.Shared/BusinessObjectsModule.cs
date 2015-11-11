using Autofac;
using MyVote.BusinessObjects.Contracts;

namespace MyVote.BusinessObjects
{
	public sealed class BusinessObjectsModule
		: Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterGeneric(typeof(ObjectFactory<>)).As(typeof(IObjectFactory<>));
		}
	}
}
