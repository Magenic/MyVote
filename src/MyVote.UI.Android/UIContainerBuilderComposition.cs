using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Autofac;
using MyVote.Core;
using MyVote.UI.Services;
using MyVote.UI.Helpers;

namespace MyVote.UI.Droid
{
	public sealed class UiContainerBuilderComposition : IContainerBuilderComposition
	{
		public void Compose(ContainerBuilder builder)
		{

			builder.RegisterType<MobileService>().AsImplementedInterfaces();
			builder.RegisterType<MessageBox>().AsImplementedInterfaces();
            builder.RegisterType<AzureStorageService>().AsImplementedInterfaces();
			/*
			builder.RegisterAssemblyTypes(typeof(PageViewModelBase).GetTypeInfo().Assembly);


			builder.RegisterType<AppSettings>().AsImplementedInterfaces();
			builder.RegisterType<PhotoChooser>().AsImplementedInterfaces();
			builder.RegisterType<AzureStorageService>().AsImplementedInterfaces();
			builder.RegisterType<Navigation>().AsImplementedInterfaces();
			*/
			#if NETFX_CORE
			builder.RegisterType<ShareManager>().AsImplementedInterfaces();
			builder.RegisterType<SecondaryPinner>().AsImplementedInterfaces();
			#endif
		}
	}
}

