using Microsoft.AspNetCore.Mvc;

namespace MyVote.Services.AppServer.Controllers
{
	[Route("api/[controller]")]
	public sealed class HomeController 
		: Controller
	{
		[HttpGet]
		public IActionResult Get()
		{
			return new OkObjectResult(new { Title = "MyVote AppServer" });
		}
	}
}
