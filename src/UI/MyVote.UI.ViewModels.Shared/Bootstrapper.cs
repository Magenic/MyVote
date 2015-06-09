using Autofac;
using Csla.Security;
using MyVote.BusinessObjects;
using MyVote.UI.Helpers;
using MyVote.UI.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyVote.UI
{
    public class Bootstrapper
    {
		private static string CloudPortalUrl = "http://myapp.cloudapp.net/MobilePortal.svc";
		private static string LocalPortalUrl = "http://localhost:55130/MobilePortal.svc";

		public IContainer Bootstrap()
		{
#if MOBILE
			Csla.ApplicationContext.DataPortalProxy = typeof(Csla.DataPortalClient.WcfProxy).AssemblyQualifiedName;
			Csla.DataPortal.ProxyTypeName = typeof(Csla.DataPortalClient.WcfProxy).AssemblyQualifiedName;
#else
			Csla.ApplicationContext.DataPortalProxy = typeof(Csla.DataPortalClient.HttpProxy).AssemblyQualifiedName;
      CloudPortalUrl = "http://myapp.cloudapp.net/DataPortal/PostAsync"; // MVC 4
      LocalPortalUrl = "http://localhost:55130/DataPortal/PostAsync"; // MVC 4
      //Csla.DataPortal.ProxyTypeName = typeof(Csla.DataPortalClient.HttpProxy).AssemblyQualifiedName;
#endif // MOBILE

#if DEBUG && !MOBILE
			Csla.ApplicationContext.DataPortalUrlString = Bootstrapper.LocalPortalUrl;
			//Csla.DataPortalClient.HttpProxy.DefaultUrl = Bootstrapper.LocalPortalUrl;
#else

            Csla.ApplicationContext.DataPortalUrlString = Bootstrapper.CloudPortalUrl;

	#if MOBILE
            Csla.DataPortalClient.WcfProxy.DefaultUrl = Bootstrapper.CloudPortalUrl;
	#else
			//Csla.DataPortalClient.HttpProxy.DefaultUrl = Bootstrapper.CloudPortalUrl;
	#endif // MOBILE
#endif // DEBUG

			Csla.ApplicationContext.User = new UnauthenticatedPrincipal();
			var container = new ContainerBuilder();

			container.RegisterModule(new UiModule());
			container.RegisterModule(new BusinessObjectsModule());
			
			//container.RegisterType<PhotoChooser>().AsImplementedInterfaces();

			var returnValue = container.Build();
			Csla.ApplicationContext.DataPortalActivator = new ObjectActivator(returnValue);

			return returnValue;
		}
    }
}
