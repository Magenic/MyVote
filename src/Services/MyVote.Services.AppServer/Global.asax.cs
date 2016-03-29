using Autofac;
using Autofac.Integration.WebApi;
using Csla;
using MyVote.BusinessObjects;
using MyVote.Data.Entities;
using MyVote.Services.AppServer.Auth;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MyVote.Services.AppServer
{
	public class MvcApplication : HttpApplication
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
			ApplicationContext.DataPortalActivator = new ObjectActivator(
				container, new ActivatorCallContext());


			AreaRegistration.RegisterAllAreas();
			GlobalConfiguration.Configure(WebApiConfig.Register);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
		}
	}
}
