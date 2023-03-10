using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.Accounts;
using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Services.User;
using PersonalFinancer.Services.User.Models;
using static PersonalFinancer.Data.Constants.RoleConstants;

namespace PersonalFinancer.Web.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = AdminRoleName)]
	public class UsersController : Controller
	{
		private readonly IUserService userService;
		private readonly IAccountService accountService;

		public UsersController(
			IUserService userService,
			IAccountService accountService)
		{
			this.userService = userService;
			this.accountService = accountService;
		}

		public async Task<IActionResult> Index()
		{
			IEnumerable<UserViewModel> users = await userService.All();

			return View(users);
		}

		[HttpGet]
		public async Task<IActionResult> Details(string id)
		{
			try
			{
				UserDetailsViewModel user = await userService.UserDetails(id);

				return View(user);
			}
			catch (NullReferenceException)
			{
				return NotFound();
			}
		}

		[HttpGet]
		public async Task<IActionResult> AccountDetails(Guid id, string? startDate, string? endDate, int page = 1)
		{
			try
			{
				AccountDetailsViewModel model;

				if (startDate == null || endDate == null)
				{
					model = await accountService.AccountDetailsViewModel(id, DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow, page);
				}
				else
				{
					model = await accountService.AccountDetailsViewModel(id, DateTime.Parse(startDate), DateTime.Parse(endDate), page);
				}

				ViewBag.Area = "Admin";
				ViewBag.Controller = "Users";
				ViewBag.Action = "AccountDetails";
				ViewBag.ReturnUrl = "~/Admin/Users/AccountDetails/" + id;

				return View(model);
			}
			catch (NullReferenceException)
			{
				return BadRequest();
			}
		}
	}
}
