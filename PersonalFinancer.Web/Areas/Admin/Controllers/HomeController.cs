namespace PersonalFinancer.Web.Areas.Admin.Controllers
{
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;

	using static Data.DataConstants.RoleConstants;

	[Area("Admin")]
	[Authorize(Roles = AdminRoleName)]
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
