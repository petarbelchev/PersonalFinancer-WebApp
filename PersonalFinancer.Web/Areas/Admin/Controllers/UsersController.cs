using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.Accounts;
using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Services.User;
using System;
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
			=> View(await userService.All());

		public async Task<IActionResult> Details(string id)
		{
			ViewBag.Area = "Admin";
			ViewBag.Controller = "Users";
			ViewBag.Action = "AccountDetails";

			return View(await userService.UserDetails(id));
		}

		public async Task<IActionResult> AccountDetails
			(Guid id, string? startDate, string? endDate, int page = 1)
		{
			AccountDetailsViewModel model;

			if (startDate == null || endDate == null)
			{
				model = await accountService.GetAccountDetailsViewModel(id, DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow, page);
			}
			else
			{
				model = await accountService.GetAccountDetailsViewModel(id, DateTime.Parse(startDate), DateTime.Parse(endDate), page);
			}

			ViewBag.Area = "Admin";
			ViewBag.Controller = "Users";
			ViewBag.Action = "AccountDetails";
			ViewBag.ReturnUrl = "~/Admin/Users/AccountDetails/" + id;
			ViewBag.ModelId = model.Id;

			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> AccountDetails(AccountDetailsViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			model = await accountService.GetAccountDetailsViewModel(model.Id, model.StartDate, model.EndDate);

			ViewBag.Area = "Admin";
			ViewBag.Controller = "Users";
			ViewBag.Action = "AccountDetails";
			ViewBag.ReturnUrl = "~/Admin/Users/AccountDetails/" + model.Id;
			ViewBag.ModelId = model.Id;

			return View(model);
		}
	}
}
