using System.Configuration;
using System.Web.Http;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using MyVote.Services.AppServer.Auth;
using Thinktecture.IdentityModel.Http.Cors.WebApi;

namespace MyVote.Services.AppServer
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            //config.SuppressDefaultHostAuthentication();
            //config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Uncomment the following line of code to enable query support for actions with an IQueryable or IQueryable<T> return type.
            // To avoid processing unexpected or malicious queries, use the validation settings on QueryableAttribute to validate incoming queries.
            // For more information, visit http://go.microsoft.com/fwlink/?LinkId=279712.
            //config.EnableQuerySupport();

            // To disable tracing in your application, please comment out or remove the following line of code
            // For more information, refer to: http://www.asp.net/web-api
            config.EnableSystemDiagnosticsTracing();

            // Configure Cross-Origin Resource Sharing (CORS)
            var corsConfig = new WebApiCorsConfiguration();
            corsConfig.RegisterGlobal(config);
            corsConfig
                .ForAllResources()
                .ForOrigins(
                  "https://myapp.azurewebsites.net",
                  "http://myapp.azurewebsites.net",
                  "http://localhost:55001") //This is the web local address
                .AllowAll();

            // register JWT authorization validation
            string zumoMaster = RoleEnvironment.IsAvailable
                ? CloudConfigurationManager.GetSetting("zumoMaster")
                : ConfigurationManager.AppSettings["zumoMaster"];
            JsonWebTokenValidationHandler.Register(config, zumoMaster);

        }
    }
}
