using Autofac;
using MyVote.Core;

namespace MyVote.Repository
{
	public sealed class RepositoryContainerBuilderComposition
		: IContainerBuilderComposition
	{
		public void Compose(ContainerBuilder builder)
		{
			builder.Register<IEntities>(c => Entities.GetContext()).As<IEntities>();
		}
	}
}
