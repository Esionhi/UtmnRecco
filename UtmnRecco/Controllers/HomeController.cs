using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using UtmnRecco.Models;
using UtmnRecco.Services;
using VkNet.Enums.Filters;

namespace UtmnRecco.Controllers
{
	[Route("")]
	public class HomeController : AnalyzingController
	{
		[HttpGet]
		public IActionResult Home()
		{
			return View("Views/Welcome.cshtml");
		}

		[HttpGet("[action]")]
		public async Task<IActionResult> Result([FromQuery]string link, [FromServices]AnalyzingService analyzer)
		{
			string screenName = link.Replace(@"https://vk.com/", string.Empty);
			var users = await VkApi.Users.GetAsync(new[] { screenName }, ProfileFields.Photo100);
			var user = users[0];
			var response = VkApi.Call(
				"execute.getUserInfo",
				new VkNet.Utils.VkParameters(new Dictionary<string, string>
				{
					["id"] = user.Id.ToString()
				}));
			
			var fetchedUser = JsonConvert.DeserializeObject<FetchedResponse>(response.RawJson).Response;

			fetchedUser.Wall.Count = 300;
			fetchedUser.Wall.Items = fetchedUser.Wall.Items.Take(300).ToArray();

			var result = analyzer.Analyze(fetchedUser);
			ViewData["Name"] = $"{user.FirstName} {user.LastName}";
			ViewData["FacultyName"] = result.First().Key.ToString();
			ViewData["FacultyLink"] = "https://utmn.ru";
			ViewData["AvatarUrl"] = user.Photo100;
			return View("Views/Result.cshtml", result);
		}
	}
}