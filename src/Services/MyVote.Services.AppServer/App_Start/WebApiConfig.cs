using System.Configuration;
using System.Web.Http;
using System.Web.Http.Cors;
using Microsoft.Azure;
using Microsoft.WindowsAzure.ServiceRuntime;
using MyVote.Services.AppServer.Auth;

namespace MyVote.Services.AppServer
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Configure Cross-Origin Resource Sharing (CORS)
            string originsAllowed = "https://myapp.azurewebsites.net," +
									"http://myapp.azurewebsites.net," +
									"https://myapp-dev.azurewebsites.net," +
									"http://myapp-dev.azurewebsites.net," +
									"http://myapp.com," +
									"https://myapp.com," +
                                    "http://localhost:55001"; //This is the web local address
            var cors = new EnableCorsAttribute(originsAllowed, "*", "*");
            config.EnableCors(cors);

            // register JWT authorization validation
            string zumoMaster = RoleEnvironment.IsAvailable
                ? CloudConfigurationManager.GetSetting("zumoMaster")
                : ConfigurationManager.AppSettings["zumoMaster"];
            JsonWebTokenValidationHandler.Register(config, zumoMaster);

        }
    }
}
