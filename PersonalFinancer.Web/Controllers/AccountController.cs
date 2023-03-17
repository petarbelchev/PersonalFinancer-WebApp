using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.Accounts;
using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Services.AccountTypes;
using PersonalFinancer.Services.Currencies;
using PersonalFinancer.Services.Shared.Models;
using PersonalFinancer.Web.Infrastructure;
using static PersonalFinancer.Data.Constants.RoleConstants;

namespace PersonalFinancer.Web.Controllers
{
	[Authorize(Roles = UserRoleName)]
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

		public async Task<IActionResult> Create()
		{
			return View(new AccountFormModel
			{
				AccountTypes = await accountTypeService.GetUserAccountTypesViewModel(User.Id()),
				Currencies = await currencyService.GetUserCurrencies(User.Id())
			});
		}

		[HttpPost]
		public async Task<IActionResult> Create(AccountFormModel inputModel)
		{
			if (!ModelState.IsValid)
			{
				inputModel.AccountTypes = await accountTypeService
					.GetUserAccountTypesViewModel(User.Id());

				inputModel.Currencies = await currencyService
					.GetUserCurrencies(User.Id());

				return View(inputModel);
			}

			try
			{
				string newAccountId = await accountService.CreateAccount(User.Id(), inputModel);

				TempData["successMsg"] = "You create a new account successfully!";

				return RedirectToAction(nameof(Details), new { id = newAccountId });
			}
			catch (ArgumentException)
			{
				ModelState.AddModelError(
					nameof(inputModel.Name),
					"You already have Account with that name.");

				inputModel.AccountTypes = await accountTypeService
					.GetUserAccountTypesViewModel(User.Id());

				inputModel.Currencies = await currencyService
					.GetUserCurrencies(User.Id());

				return View(inputModel);
			}
		}

		public async Task<IActionResult> Details(
			string id, string? startDate, string? endDate, int page = 1)
		{
			if (!ModelState.IsValid)
				return BadRequest();

			DetailsAccountViewModel viewModel;

			try
			{
				if (!await accountService.IsAccountOwner(User.Id(), id))
					return Unauthorized();

				if (startDate == null || endDate == null)
				{
					viewModel = await accountService.GetAccountDetailsViewModel(
						id, DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow, page);
				}
				else
				{
					viewModel = await accountService.GetAccountDetailsViewModel(
						id, DateTime.Parse(startDate), DateTime.Parse(endDate), page);
				}
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}

			viewModel.Routing.ReturnUrl = "/Account/Details/" + id;
			ViewBag.ModelId = id;

			return View(viewModel);
		}

		[HttpPost]
		public async Task<IActionResult> Details(string id, DateFilterModel dateModel)
		{
			if (!ModelState.IsValid)
				return View(dateModel);

			DetailsAccountViewModel viewModel;

			try
			{
				viewModel = await accountService.GetAccountDetailsViewModel(
					id, dateModel.StartDate, dateModel.EndDate);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}

			viewModel.Routing.ReturnUrl = "/Account/Details/" + id;
			ViewBag.ModelId = id;

			return View(viewModel);
		}

		public async Task<IActionResult> Delete(string id)
		{
			if (!ModelState.IsValid)
				return BadRequest();

			try
			{
				if (!await accountService.IsAccountOwner(User.Id(), id))
					return Unauthorized();

				DeleteAccountViewModel viewModel = await accountService
					.GetDeleteAccountViewModel(id);

				return View(viewModel);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
		}

		[HttpPost]
		public async Task<IActionResult> Delete(DeleteAccountInputModel inputModel, string returnUrl)
		{
			if (!ModelState.IsValid)
				return BadRequest();

			if (inputModel.ConfirmButton == "reject")
				return LocalRedirect(returnUrl);

			try
			{
				if (!await accountService.IsAccountOwner(User.Id(), inputModel.Id))
					return Unauthorized();

				await accountService.DeleteAccount(
					inputModel.Id, User.Id(),
					inputModel.ShouldDeleteTransactions ?? false);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}

			TempData["successMsg"] = "Your account was successfully deleted!";

			return RedirectToAction("Index", "Home");
		}

		public async Task<IActionResult> EditAccount(string id)
		{
			try
			{
				AccountFormModel viewModel = await accountService
					.GetEditAccountModel(id);

				viewModel.Currencies = await currencyService
					.GetUserCurrencies(User.Id());

				viewModel.AccountTypes = await accountTypeService
					.GetUserAccountTypesViewModel(User.Id());

				return View(viewModel);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
		}

		[HttpPost]
		public async Task<IActionResult> EditAccount(
			string id, AccountFormModel inputModel, string returnUrl)
		{
			if (!ModelState.IsValid)
			{
				inputModel.Currencies = await currencyService
					.GetUserCurrencies(User.Id());

				inputModel.AccountTypes = await accountTypeService
					.GetUserAccountTypesViewModel(User.Id());

				return View(inputModel);
			}

			try
			{
				await accountService.EditAccount(id, inputModel, User.Id());

				TempData["successMsg"] = "Your account was successfully edited!";

				return LocalRedirect(returnUrl);
			}
			catch (ArgumentException)
			{
				ModelState.AddModelError(
					nameof(inputModel.Name),
					$"You already have Account with {inputModel.Name} name.");

				inputModel.Currencies = await currencyService
					.GetUserCurrencies(User.Id());

				inputModel.AccountTypes = await accountTypeService
					.GetUserAccountTypesViewModel(User.Id());

				return View(inputModel);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
		}
	}
}