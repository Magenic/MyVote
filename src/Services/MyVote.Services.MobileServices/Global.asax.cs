using MyVoteService;
using System.Web.Http;
using System.Web.Routing;

namespace MyVote.Services.MobileServices
{
	public class WebApiApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			WebApiConfig.Register();
		}
	}
}