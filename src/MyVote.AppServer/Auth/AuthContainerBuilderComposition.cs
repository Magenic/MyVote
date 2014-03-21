using System;
using Autofac;
using MyVote.Core;

namespace MyVote.AppServer.Auth
{
	public class AuthContainerBuilderComposition
		: IContainerBuilderComposition
	{
		public void Compose(ContainerBuilder builder)
		{
			builder
				.RegisterType<MyVoteAuthentication>()
				.As<IMyVoteAuthentication>()
				.PropertiesAutowired();
		}
	}
}