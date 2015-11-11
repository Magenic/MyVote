using System;
using Autofac;
using MyVote.Core;

namespace MyVote.Services.AppServer.Auth
{
	public sealed class AuthModule
		:  Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder
				.RegisterType<MyVoteAuthentication>()
				.As<IMyVoteAuthentication>()
				.PropertiesAutowired();
		}
	}
}