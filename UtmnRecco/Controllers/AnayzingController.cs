using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using UtmnRecco.Services;

namespace UtmnRecco.Controllers
{
	public class AnalyzingController : VkController, IAsyncActionFilter
	{
		AnalyzingService fetcher;

		public AnalyzingService Fetcher
		{
			get
			{
				if (fetcher == null)
					fetcher = HttpContext.RequestServices.GetRequiredService<AnalyzingService>();
				return fetcher;
			}
		}

		[NonAction]
		public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			if (!Fetcher.IsFetchingCompleted)
			{
				context.Result = View("Views/FetchingInProgress.cshtml");
				return;
			}
			await next();
		}
	}
}