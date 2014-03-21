using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.WebApi;
using Csla;
using Microsoft.WindowsAzure.ServiceRuntime;
using MyVote.AppServer.Auth;
using MyVote.BusinessObjects;
using MyVote.Repository;

namespace MyVote.AppServer
{
	public class WebApiApplication
		: HttpApplication
	{
		protected void Application_Start()
		{
			var builder = new ContainerBuilder();
			
			new RepositoryContainerBuilderComposition().Compose(builder);
			new BusinessObjectsContainerBuilderComposition().Compose(builder);
			new AuthContainerBuilderComposition().Compose(builder);
			builder
				.RegisterApiControllers(Assembly.GetExecutingAssembly())
				.PropertiesAutowired();

			var container = builder.Build();
			GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
			ApplicationContext.DataPortalActivator = new ObjectActivator(container);

			AreaRegistration.RegisterAllAreas();

			WebApiConfig.Register(GlobalConfiguration.Configuration);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);

			if (RoleEnvironment.IsAvailable)
			{
				System.Diagnostics.Trace.Listeners.Add(
					new Microsoft.WindowsAzure.Diagnostics.DiagnosticMonitorTraceListener());
			}
		}
	}
}