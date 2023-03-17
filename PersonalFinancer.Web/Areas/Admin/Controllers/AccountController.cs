using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.Accounts;
using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Services.AccountTypes;
using PersonalFinancer.Services.Currencies;
using PersonalFinancer.Services.Shared.Models;
using static PersonalFinancer.Data.Constants.RoleConstants;

namespace PersonalFinancer.Web.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = AdminRoleName)]
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

		public async Task<IActionResult> Index(int page = 1)
			=> View(await accountService.GetAllUsersAccountCardsViewModel(page));

		public async Task<IActionResult> Details
			(string id, string? startDate, string? endDate, int page = 1)
		{
			DetailsAccountViewModel viewModel;

			try
			{
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

			viewModel.Routing.Area = "Admin";
			viewModel.Routing.ReturnUrl = "/Admin/Account/Details/" + id;
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

			viewModel.Routing.Area = "Admin";
			viewModel.Routing.ReturnUrl = "/Admin/Account/Details/" + id;
			ViewBag.ModelId = id;

			return View(viewModel);
		}

		public async Task<IActionResult> Delete(string id)
		{
			if (!ModelState.IsValid)
				return BadRequest();

			try
			{
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
				string ownerId = await accountService.GetOwnerId(inputModel.Id);

				await accountService.DeleteAccount(
					inputModel.Id, ownerId,
					inputModel.ShouldDeleteTransactions ?? false);

				TempData["successMsg"] = "You successfully delete user's account!";

				return LocalRedirect("/Admin/Users/Details/" + ownerId);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
		}

		public async Task<IActionResult> EditAccount(string id)
		{
			try
			{
				AccountFormModel viewModel = await accountService
					.GetEditAccountModel(id);

				string ownerId = await accountService.GetOwnerId(id);

				viewModel.Currencies = await currencyService
					.GetUserCurrencies(ownerId);

				viewModel.AccountTypes = await accountTypeService
					.GetUserAccountTypesViewModel(ownerId);

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
			string ownerId = string.Empty;

			try
			{
				ownerId = await accountService.GetOwnerId(id);

				if (!ModelState.IsValid)
				{
					inputModel.Currencies = await currencyService
						.GetUserCurrencies(ownerId);

					inputModel.AccountTypes = await accountTypeService
						.GetUserAccountTypesViewModel(ownerId);

					return View(inputModel);
				}

				await accountService.EditAccount(id, inputModel, ownerId);

				TempData["successMsg"] = "You successfully edited user's account!";

				return LocalRedirect(returnUrl);
			}
			catch (ArgumentException)
			{
				ModelState.AddModelError(nameof(inputModel.Name), $"You already have Account with {inputModel.Name} name.");

				inputModel.Currencies = await currencyService
					.GetUserCurrencies(ownerId);

				inputModel.AccountTypes = await accountTypeService
					.GetUserAccountTypesViewModel(ownerId);

				return View(inputModel);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
		}
	}
}
