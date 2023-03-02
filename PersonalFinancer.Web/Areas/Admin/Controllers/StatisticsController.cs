using Microsoft.AspNetCore.Mvc;

namespace PersonalFinancer.Web.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class StatisticsController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
