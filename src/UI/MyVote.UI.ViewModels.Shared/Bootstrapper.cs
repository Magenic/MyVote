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
		private static string DataPortalUrl =  "http://yourapi.azurewebsites.net/api/DataPortal";
#endif

		public IContainer Bootstrap()
		{
			Csla.DataPortal.ProxyTypeName = typeof(Csla.DataPortalClient.HttpProxy).AssemblyQualifiedName;
			Csla.ApplicationContext.DataPortalUrlString = Bootstrapper.DataPortalUrl;

			Csla.ApplicationContext.User = new UnauthenticatedPrincipal();
			var container = new ContainerBuilder();

			container.RegisterModule(new UiModule());
			container.RegisterModule(new BusinessObjectsModule());

			//container.RegisterType<PhotoChooser>().AsImplementedInterfaces();

			var returnValue = container.Build();
			Csla.ApplicationContext.DataPortalActivator = new ObjectActivator(
				returnValue, new ExecutionCallContext());

			return returnValue;
		}
	}
}
