using System.Diagnostics;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.WebApi;
using Csla;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using MyVote.BusinessObjects;
using MyVote.Data.Entities;
using MyVote.Services.AppServer.Auth;
using MyVote.Services.AppServer.Filters;

namespace MyVote.Services.AppServer
{
	public class WebApiApplication : 
		HttpApplication
	{
		protected void Application_Start()
		{
			var builder = new ContainerBuilder();
			builder.RegisterModule(new EntitiesModule());
			builder.RegisterModule(new BusinessObjectsModule());
			builder.RegisterModule(new AuthModule());
			builder
				.RegisterApiControllers(Assembly.GetExecutingAssembly())
				.PropertiesAutowired();

			var container = builder.Build();
			GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
			ApplicationContext.DataPortalActivator = new ObjectActivator(container);

			AreaRegistration.RegisterAllAreas();
			GlobalConfiguration.Configure(WebApiConfig.Register);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			GlobalConfiguration.Configuration.Filters.Add(new UnhandledExceptionFilter(/*this.logger*/));
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);

			if (RoleEnvironment.IsAvailable)
			{
				Trace.Listeners.Add(new DiagnosticMonitorTraceListener());
			}
		}
	}
}
