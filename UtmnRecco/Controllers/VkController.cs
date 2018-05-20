using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using VkNet;

namespace UtmnRecco.Controllers
{
	public class VkController : Controller
	{
		VkApi vkApi;

		public VkApi VkApi
		{
			get
			{
				if (vkApi == null)
					vkApi = HttpContext.RequestServices.GetRequiredService<VkApi>();
				return vkApi;
			}
		}
	}
}