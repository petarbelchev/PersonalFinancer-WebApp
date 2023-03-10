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

		[HttpGet]
		[Authorize(Roles = UserRoleName)]
		public async Task<IActionResult> Create()
		{
			string userId = User.Id();

			AccountFormModel formModel = new AccountFormModel
			{
				AccountTypes = await accountTypeService.AccountTypesViewModel(userId),
				Currencies = await currencyService.UserCurrencies(userId)
			};

			return View(formModel);
		}

		[HttpPost]
		[Authorize(Roles = UserRoleName)]
		public async Task<IActionResult> Create(AccountFormModel accountFormModel)
		{
			if (!ModelState.IsValid)
			{
				return View(accountFormModel);
			}

			try
			{
				Guid newAccountId = await accountService.CreateAccount(User.Id(), accountFormModel);

				TempData["successMsg"] = "You create a new account successfully!";

				return RedirectToAction(nameof(Details), new { id = newAccountId });
			}
			catch (InvalidOperationException)
			{
				ModelState.AddModelError(nameof(accountFormModel.Name), "You already have Account with that name.");

				accountFormModel.AccountTypes = await accountTypeService.AccountTypesViewModel(User.Id());
				accountFormModel.Currencies = await currencyService.UserCurrencies(User.Id());

				return View(accountFormModel);
			}
		}

		[HttpGet]
		[Authorize(Roles = UserRoleName)]
		public async Task<IActionResult> Details(Guid id, string? startDate, string? endDate, int page = 1)
		{
			if (!await accountService.IsAccountOwner(User.Id(), id))
			{
				return Unauthorized();
			}

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

				ViewBag.Area = "";
				ViewBag.Controller = "Account";
				ViewBag.Action = "Details";
				ViewBag.ReturnUrl = "~/Account/Details/" + id;

				return View(model);
			}
			catch (NullReferenceException)
			{
				return BadRequest();
			}
		}

		[HttpPost]
		[Authorize(Roles = UserRoleName)]
		public async Task<IActionResult> Details(AccountDetailsViewModel accountModel)
		{
			if (!ModelState.IsValid)
			{
				return View(accountModel);
			}

			try
			{
				AccountDetailsViewModel accountDetails = await accountService
					.AccountDetailsViewModel(accountModel.Id, accountModel.StartDate, accountModel.EndDate);

				ViewBag.Area = "";
				ViewBag.Controller = "Account";
				ViewBag.Action = "Details";
				ViewBag.ReturnUrl = "~/Account/Details/" + accountDetails.Id;

				return View(accountDetails);
			}
			catch (Exception)
			{
				return BadRequest();
			}
		}

		[HttpGet]
		public async Task<IActionResult> Delete(Guid id, string? returnUrl)
		{
			if (!await accountService.IsAccountOwner(User.Id(), id) && !User.IsAdmin())
			{
				return Unauthorized();
			}

			DeleteAccountViewModel? accountModel = await accountService.DeleteAccountViewModel(id);

			if (accountModel == null)
			{
				return BadRequest();
			}

			if (returnUrl != null)
			{
				accountModel.ReturnUrl = returnUrl;
			}

			return View(accountModel);
		}

		[HttpPost]
		public async Task<IActionResult> Delete(DeleteAccountViewModel accountModel, string confirmButton)
		{
			if (confirmButton == "reject")
			{
				return LocalRedirect(accountModel.ReturnUrl);
			}

			if (!await accountService.IsAccountOwner(User.Id(), accountModel.Id) && !User.IsAdmin())
			{
				return Unauthorized();
			}

			await accountService.DeleteAccountById(accountModel.Id, User.Id(), accountModel.ShouldDeleteTransactions);

			if (User.IsAdmin())
			{
				TempData["successMsg"] = "You successfully delete user's account!";

				return LocalRedirect("~/Admin/Users/Details/" + accountModel.OwnerId);
			}

			TempData["successMsg"] = "Your account was successfully deleted!";

			return RedirectToAction("Index", "Home");
		}

		[HttpGet]
		public async Task<IActionResult> Edit(Guid id, string? returnUrl)
		{
			EditAccountFormModel accountModel = await accountService.GetEditAccountFormModel(id);

			if (!User.IsAdmin())
			{
				accountModel.Currencies = await currencyService.UserCurrencies(User.Id());
				accountModel.AccountTypes = await accountTypeService.AccountTypesViewModel(User.Id());
			}
			else
			{
				accountModel.Currencies = await currencyService.UserCurrencies(accountModel.OwnerId);
				accountModel.AccountTypes = await accountTypeService.AccountTypesViewModel(accountModel.OwnerId);
			}

			if (returnUrl != null)
			{
				accountModel.ReturnUrl = returnUrl;
			}

			return View(accountModel);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(EditAccountFormModel accountModel)
		{
			if (!ModelState.IsValid)
			{
				accountModel.Currencies = await currencyService.UserCurrencies(User.Id());
				accountModel.AccountTypes = await accountTypeService.AccountTypesViewModel(User.Id());

				return View(accountModel);
			}

			try
			{
				await accountService.EditAccount(accountModel, User.Id());

				if (accountModel.ReturnUrl != null)
				{
					return LocalRedirect(accountModel.ReturnUrl);
				}

				return RedirectToAction(nameof(Details), new { id = accountModel.Id });
			}
			catch (NullReferenceException)
			{
				return BadRequest();
			}
			catch (InvalidOperationException)
			{
				ModelState.AddModelError(nameof(accountModel.Name), $"You already have Account with {accountModel.Name} name.");

				accountModel.Currencies = await currencyService.UserCurrencies(User.Id());
				accountModel.AccountTypes = await accountTypeService.AccountTypesViewModel(User.Id());

				return View(accountModel);
			}
		}
	}
}