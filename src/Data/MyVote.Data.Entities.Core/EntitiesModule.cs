using Autofac;
using Microsoft.Extensions.Configuration;

namespace MyVote.Data.Entities
{
	public sealed class EntitiesModule
		: Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<EntitiesContext>().As<IEntitiesContext>();
			builder.RegisterInstance(new SearchWhereClause()).As<ISearchWhereClause>();

			var configuration = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json");
			var root = configuration.Build();

			builder.RegisterInstance(root).As<IConfigurationRoot>();
		}
	}
}
