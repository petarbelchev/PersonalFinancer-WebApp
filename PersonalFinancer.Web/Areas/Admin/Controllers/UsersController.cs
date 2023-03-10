﻿using Microsoft.AspNetCore.Authorization;
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
		public async Task<IActionResult> AccountDetails(Guid id)
		{
			AccountDetailsViewModel? accountModel = await accountService.AccountDetailsViewModel(id);

			if (accountModel == null)
			{
				return NotFound();
			}

			ViewBag.ReturnUrl = "~/Admin/Users/AccountDetails/" + id;

			return View(accountModel);
		}
	}
}
