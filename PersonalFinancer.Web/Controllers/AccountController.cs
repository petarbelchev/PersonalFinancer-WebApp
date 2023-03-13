using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.Accounts;
using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Services.AccountTypes;
using PersonalFinancer.Services.Currencies;
using PersonalFinancer.Web.Infrastructure;
using static PersonalFinancer.Data.Constants.RoleConstants;

namespace PersonalFinancer.Web.Controllers
{
	[Authorize]
	public class AccountController : Controller
	{
		private readonly IAccountService accountService;
		private readonly ICurrencyService currencyService;
		private readonly IAccountTypeService accountTypeService;

		public AccountController(
			IAccountService accountService,
			ICurrencyService currencyService,
			IAccountTypeService accountTypeService)
		{
			this.accountService = accountService;
			this.currencyService = currencyService;
			this.accountTypeService = accountTypeService;
		}

		[Authorize(Roles = UserRoleName)]
		public async Task<IActionResult> Create()
		{
			string userId = User.Id();

			CreateAccountFormModel formModel = new CreateAccountFormModel
			{
				AccountTypes = await accountTypeService.GetUserAccountTypesViewModel(userId),
				Currencies = await currencyService.GetUserCurrencies(userId)
			};

			return View(formModel);
		}

		[HttpPost]
		[Authorize(Roles = UserRoleName)]
		public async Task<IActionResult> Create(CreateAccountFormModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			try
			{
				Guid newAccountId = await accountService.CreateAccount(User.Id(), model);

				TempData["successMsg"] = "You create a new account successfully!";

				return RedirectToAction(nameof(Details), new { id = newAccountId });
			}
			catch (ArgumentException)
			{
				ModelState.AddModelError(nameof(model.Name), "You already have Account with that name.");

				model.AccountTypes = await accountTypeService.GetUserAccountTypesViewModel(User.Id());
				model.Currencies = await currencyService.GetUserCurrencies(User.Id());

				return View(model);
			}
		}

		[Authorize(Roles = UserRoleName)]
		public async Task<IActionResult> Details(Guid id, string? startDate, string? endDate, int page = 1)
		{
			AccountDetailsViewModel model;

			try
			{
				if (!await accountService.IsAccountOwner(User.Id(), id))
				{
					return Unauthorized();
				}

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

			ViewBag.Area = "";
			ViewBag.Controller = "Account";
			ViewBag.Action = "Details";
			ViewBag.ReturnUrl = "~/Account/Details/" + model.Id;
			ViewBag.ModelId = model.Id;

			return View(model);
		}

		[HttpPost]
		[Authorize(Roles = UserRoleName)]
		public async Task<IActionResult> Details(AccountDetailsViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			try
			{
				model = await accountService.GetAccountDetailsViewModel(model.Id, model.StartDate, model.EndDate);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}

			ViewBag.Area = "";
			ViewBag.Controller = "Account";
			ViewBag.Action = "Details";
			ViewBag.ReturnUrl = "~/Account/Details/" + model.Id;
			ViewBag.ModelId = model.Id;

			return View(model);
		}

		public async Task<IActionResult> Delete(Guid id, string? returnUrl)
		{
			try
			{
				if (!await accountService.IsAccountOwner(User.Id(), id) && !User.IsAdmin())
				{
					return Unauthorized();
				}

				DeleteAccountViewModel model = await accountService.GetDeleteAccountViewModel(id);

				if (returnUrl != null)
				{
					model.ReturnUrl = returnUrl;
				}

				return View(model);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
		}

		[HttpPost]
		public async Task<IActionResult> Delete(DeleteAccountViewModel model, string confirmButton)
		{
			if (confirmButton == "reject")
			{
				return LocalRedirect(model.ReturnUrl);
			}

			try
			{
				if (!await accountService.IsAccountOwner(User.Id(), model.Id) && !User.IsAdmin())
				{
					return Unauthorized();
				}

				await accountService.DeleteAccount(model.Id, User.Id(), model.ShouldDeleteTransactions);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}

			if (User.IsAdmin())
			{
				TempData["successMsg"] = "You successfully delete user's account!";

				return LocalRedirect("~/Admin/Users/Details/" + model.OwnerId);
			}

			TempData["successMsg"] = "Your account was successfully deleted!";

			return RedirectToAction("Index", "Home");
		}

		public async Task<IActionResult> Edit(Guid id, string? returnUrl)
		{
			try
			{
				EditAccountFormModel model = await accountService.GetEditAccountFormModel(id);

				if (!User.IsAdmin())
				{
					model.Currencies = await currencyService.GetUserCurrencies(User.Id());
					model.AccountTypes = await accountTypeService.GetUserAccountTypesViewModel(User.Id());
				}
				else
				{
					model.Currencies = await currencyService.GetUserCurrencies(model.OwnerId);
					model.AccountTypes = await accountTypeService.GetUserAccountTypesViewModel(model.OwnerId);
				}

				if (returnUrl != null)
				{
					model.ReturnUrl = returnUrl;
				}

				return View(model);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
		}

		[HttpPost]
		public async Task<IActionResult> Edit(EditAccountFormModel model)
		{
			if (!ModelState.IsValid)
			{
				model.Currencies = await currencyService.GetUserCurrencies(User.Id());
				model.AccountTypes = await accountTypeService.GetUserAccountTypesViewModel(User.Id());

				return View(model);
			}

			try
			{
				await accountService.EditAccount(model, User.Id());
			}
			catch (ArgumentException)
			{
				ModelState.AddModelError(nameof(model.Name), $"You already have Account with {model.Name} name.");

				model.Currencies = await currencyService.GetUserCurrencies(User.Id());
				model.AccountTypes = await accountTypeService.GetUserAccountTypesViewModel(User.Id());

				return View(model);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}

			if (model.ReturnUrl != null)
			{
				return LocalRedirect(model.ReturnUrl);
			}

			return RedirectToAction(nameof(Details), new { id = model.Id });
		}
	}
}