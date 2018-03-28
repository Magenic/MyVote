using Autofac;
using Csla.Security;
using MyVote.BusinessObjects;

namespace MyVote.UI
{
	public class Bootstrapper
	{
#if DEBUG_OFF && !__MOBILE__
		private static string DataPortalUrl = "http://localhost:55130/api/DataPortal/PostAsync";
#else
        //private static string DataPortalUrl =  "http://myapi.azurewebsites.net/api/DataPortal";
        private static string DataPortalUrl = "http://myapi-stage.azurewebsites.net/api/DataPortal";
#endif

		public IContainer Bootstrap()
		{
			Csla.DataPortal.ProxyTypeName = typeof(Csla.DataPortalClient.HttpProxy).AssemblyQualifiedName;
			Csla.ApplicationContext.DataPortalUrlString = Bootstrapper.DataPortalUrl;

			Csla.ApplicationContext.User = new UnauthenticatedPrincipal();
			var containerBuilder = new ContainerBuilder();

			containerBuilder.RegisterModule(new UiModule());
			containerBuilder.RegisterModule(new BusinessObjectsModule());

			//containerBuilder.RegisterType<PhotoChooser>().AsImplementedInterfaces();

			var container = containerBuilder.Build();
			Csla.ApplicationContext.DataPortalActivator = new ObjectActivator(
				container, new ExecutionCallContext());

			return container;
		}
	}
}
