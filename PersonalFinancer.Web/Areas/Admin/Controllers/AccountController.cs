using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.Accounts;
using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Services.Shared.Models;
using static PersonalFinancer.Data.Constants.RoleConstants;

namespace PersonalFinancer.Web.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = AdminRoleName)]
	public class AccountController : Controller
	{
		private readonly IAccountService accountService;

		public AccountController(IAccountService accountService)
		{
			this.accountService = accountService;
		}

		public async Task<IActionResult> Index(int page = 1)
		{
			ViewBag.Area = "Admin";
			ViewBag.Controller = "Account";
			ViewBag.Action = "Index";
			ViewBag.NameOfElements = "accounts";

			return View(await accountService.GetAllUsersAccountCardsViewModel(page));
		}

		public async Task<IActionResult> Details
			(Guid id, string? startDate, string? endDate, int page = 1)
		{
			AccountDetailsViewModel model;

			try
			{
				if (startDate == null || endDate == null)
				{
					model = await accountService.GetAccountDetailsViewModel(id, DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow, page);
				}
				else
				{
					model = await accountService.GetAccountDetailsViewModel(id, DateTime.Parse(startDate), DateTime.Parse(endDate), page);
				}
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}

			ViewBag.Area = "Admin";
			ViewBag.Controller = "Account";
			ViewBag.Action = "Details";
			ViewBag.ReturnUrl = "~/Admin/Account/Details/" + id;
			ViewBag.ModelId = model.Dates.Id;

			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Details(DateFilterModel dateModel)
		{
			if (!ModelState.IsValid)
			{
				return View(dateModel);
			}

			AccountDetailsViewModel model;

			try
			{
				model = await accountService.GetAccountDetailsViewModel(dateModel.Id, dateModel.StartDate, dateModel.EndDate);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}

			ViewBag.Area = "Admin";
			ViewBag.Controller = "Account";
			ViewBag.Action = "Details";
			ViewBag.ReturnUrl = "~/Admin/Account/Details/" + model.Dates.Id;
			ViewBag.ModelId = model.Dates.Id;

			return View(model);
		}
	}
}
