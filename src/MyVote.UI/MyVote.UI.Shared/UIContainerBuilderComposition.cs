using Autofac;
using MyVote.Core;
using MyVote.UI.Services;
using MyVote.UI.Helpers;
using MyVote.UI.ViewModels;

namespace MyVote.UI
{
    public sealed class UiContainerBuilderComposition : IContainerBuilderComposition
	{
		public void Compose(ContainerBuilder builder)
		{
            //builder.RegisterAssemblyTypes(typeof(BusinessObjects.UserRoles).GetTypeInfo().Assembly);

            builder.RegisterType<MobileService>().AsImplementedInterfaces();
			builder.RegisterType<MessageBox>().AsImplementedInterfaces();
            builder.RegisterType<AzureStorageService>().AsImplementedInterfaces();
		    builder.RegisterType<PollImageViewModel>().AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(typeof(PageViewModelBase).Assembly)
				.Where(t => t.IsSubclassOf(typeof(PageViewModelBase)));

			/*
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

