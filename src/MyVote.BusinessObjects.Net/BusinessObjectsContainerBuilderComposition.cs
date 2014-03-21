using Autofac;
using MyVote.BusinessObjects.Contracts;
using MyVote.Core;

namespace MyVote.BusinessObjects
{
	public sealed class BusinessObjectsContainerBuilderComposition
		: IContainerBuilderComposition
	{
		public void Compose(ContainerBuilder builder)
		{
			builder.RegisterGeneric(typeof(ObjectFactory<>)).As(typeof(IObjectFactory<>));
		}
	}
}
