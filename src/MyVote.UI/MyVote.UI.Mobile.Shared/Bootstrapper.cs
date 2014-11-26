using Autofac;
using MyVote.BusinessObjects;
using MyVote.UI.Helpers;
using MyVote.UI.Services;
using MyVote.UI.ViewModels;

namespace MyVote.UI
{
    class Bootstrapper
    {
        public IContainer Bootstrap()
        {
            Csla.ApplicationContext.DataPortalProxy = typeof(Csla.DataPortalClient.WcfProxy).AssemblyQualifiedName;
			Csla.DataPortal.ProxyTypeName = typeof(Csla.DataPortalClient.WcfProxy).AssemblyQualifiedName;
#if DEBUG
			Csla.ApplicationContext.DataPortalUrlString = "http://084dd66925e34ddca11a8277ceb0ded4.cloudapp.net/MobilePortal.svc";
            Csla.DataPortalClient.WcfProxy.DefaultUrl = "http://084dd66925e34ddca11a8277ceb0ded4.cloudapp.net/MobilePortal.svc";
            //Csla.ApplicationContext.DataPortalUrlString = "http://myvote.cloudapp.net/MobilePortal.svc";
            //Csla.DataPortalClient.WcfProxy.DefaultUrl = "http://myvote.cloudapp.net/MobilePortal.svc";
#else
            Csla.ApplicationContext.DataPortalUrlString = "http://084dd66925e34ddca11a8277ceb0ded4.cloudapp.net/MobilePortal.svc";
            Csla.DataPortalClient.WcfProxy.DefaultUrl = "http://084dd66925e34ddca11a8277ceb0ded4.cloudapp.net/MobilePortal.svc";
#endif // DEBUG
			var container = new ContainerBuilder();

			new UiContainerBuilderComposition().Compose(container);
            new BusinessObjectsContainerBuilderComposition().Compose(container);
            container.RegisterType<UIContext>().AsImplementedInterfaces();
            container.RegisterType<AppSettings>().AsImplementedInterfaces();
            container.RegisterType<VMPageMappings>().AsImplementedInterfaces();
            container.RegisterType<PhotoChooser>().AsImplementedInterfaces();

            var returnValue = container.Build();
            Csla.ApplicationContext.DataPortalActivator = new ObjectActivator(returnValue);

            return returnValue;
        }
    }
}