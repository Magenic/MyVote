using System.Web.Mvc;

namespace MyVote.AppServer.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return this.View();
		}
	}
}
