using Microsoft.AspNetCore.Mvc;
using UtmnRecco.Services;

namespace UtmnRecco.Controllers
{
	[Route("[controller]")]
	public class ServiceController : Controller
	{
		[HttpGet("[action]")]
		public IActionResult Fetched([FromServices]AnalyzingService service)
		{
			if (!service.IsFetchingCompleted)
				return Ok("Fetching is not completed");
			return Json(service.Users);
		}
	}
}