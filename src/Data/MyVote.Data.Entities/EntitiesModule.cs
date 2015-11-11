using Autofac;

namespace MyVote.Data.Entities
{
	public sealed class EntitiesModule
		: Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.Register(c => Entities.GetContext()).As<IEntities>();
			builder.RegisterInstance(new SearchWhereClause()).As<ISearchWhereClause>();
		}
	}
}
