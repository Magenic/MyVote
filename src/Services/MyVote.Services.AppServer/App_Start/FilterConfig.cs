using MyVote.Services.AppServer.Filters;
using System.Web;
using System.Web.Mvc;

namespace MyVote.Services.AppServer
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
			//filters.Add(new UnhandledExceptionFilter(/*this.logger*/));
		}
	}
}
