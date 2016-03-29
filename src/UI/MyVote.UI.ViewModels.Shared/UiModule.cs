using Autofac;
using MyVote.UI.Helpers;
using MyVote.UI.Services;
using MyVote.UI.ViewModels;
using MvvmCross.Plugins.Json;
using MvvmCross.Core.Platform;

#if NETFX_CORE
using System.Reflection;
using MvvmCross.Core.ViewModels;
#endif // NETFX_CORE

namespace MyVote.UI
{
    public sealed class UiModule : Autofac.Module
    {
		protected override void Load(ContainerBuilder builder)
		{
#if MOBILE
		    builder.RegisterType<UiContext>().AsImplementedInterfaces();
            builder.RegisterInstance(new MobileService(new UiContext())).AsImplementedInterfaces();
			builder.RegisterType<VmPageMappings>().AsImplementedInterfaces();
#else
			builder.RegisterInstance(new MobileService()).AsImplementedInterfaces();
#endif
            builder.RegisterType<MessageBox>().AsImplementedInterfaces();
			builder.RegisterType<AzureStorageService>().AsImplementedInterfaces();

			builder.RegisterType<AppSettings>().AsImplementedInterfaces();

			builder.RegisterType<Logger>().AsImplementedInterfaces();
			builder.RegisterType<PhotoChooser>().AsImplementedInterfaces();
            builder.RegisterType<MvxJsonConverter>().AsImplementedInterfaces();

#if NETFX_CORE
			builder.RegisterType<ShareManager>().AsImplementedInterfaces();
			builder.RegisterType<SecondaryPinner>().AsImplementedInterfaces();
			builder.RegisterAssemblyTypes(typeof(ViewModelBase).GetTypeInfo().Assembly)
				.Where(t => t.GetTypeInfo().IsSubclassOf(typeof(ViewModelBase)));
			builder.RegisterAssemblyTypes(typeof(PollImageViewModelBase).GetTypeInfo().Assembly)
				.Where(t => t.GetTypeInfo().IsSubclassOf(typeof(PollImageViewModelBase)));
#else
			builder.RegisterType<UiContext>().AsImplementedInterfaces();
			builder.RegisterAssemblyTypes(typeof(ViewModelBase).Assembly)
				.Where(t => t.IsSubclassOf(typeof(ViewModelBase)));
			builder.RegisterAssemblyTypes(typeof(PollImageViewModelBase).Assembly)
				.Where(t => t.IsSubclassOf(typeof(PollImageViewModelBase)));
#endif
		}
    }
}
