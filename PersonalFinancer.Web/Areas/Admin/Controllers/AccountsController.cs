using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.Accounts;
using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Services.User;
using PersonalFinancer.Services.User.Models;
using PersonalFinancer.Web.Infrastructure;
using PersonalFinancer.Web.Models.Account;
using static PersonalFinancer.Data.Constants.RoleConstants;

namespace PersonalFinancer.Web.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = AdminRoleName)]
	public class AccountsController : Controller
	{
		private readonly IAccountsService accountService;
		private readonly IUsersService usersService;

		public AccountsController(
			IAccountsService accountService,
			IUsersService usersService)
		{
			this.accountService = accountService;
			this.usersService = usersService;
		}

		public async Task<IActionResult> Index(int page = 1)
		{
			UsersAccountCardsServiceModel usersAccountCardsData =
				await accountService.GetUsersAccountCardsData(page);

			var viewModel = new UsersAccountCardsViewModel
			{
				Accounts = usersAccountCardsData.Accounts
			};
			viewModel.Pagination.Page = page;
			viewModel.Pagination.TotalElements = usersAccountCardsData.TotalUsersAccountsCount;

			return View(viewModel);
		}

		public async Task<IActionResult> AccountDetails(string id)
		{
			DateTime startDate = DateTime.UtcNow.AddMonths(-1);
			DateTime endDate = DateTime.UtcNow;

			try
			{
				AccountDetailsServiceModel accountDetails =
					await accountService.GetAccountDetails(id, startDate, endDate);

				var viewModel = new AccountDetailsViewModel
				{
					Id = accountDetails.Id,
					Name = accountDetails.Name,
					Balance = accountDetails.Balance,
					OwnerId = accountDetails.OwnerId,
					CurrencyName = accountDetails.CurrencyName,
					Transactions = accountDetails.Transactions,
					StartDate = startDate,
					EndDate = endDate
				};
				viewModel.Pagination.TotalElements = accountDetails.TotalAccountTransactions;
				viewModel.Routing.Area = "Admin";
				viewModel.Routing.ReturnUrl = "/Admin/Accounts/AccountDetails/" + id;

				return View(viewModel);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
		}

		[HttpPost]
		public async Task<IActionResult> AccountDetails(AccountDetailsInputModel inputModel)
		{
			if (!ModelState.IsValid)
			{
				try
				{
					AccountDetailsShortServiceModel accShortDetails =
						await accountService.GetAccountShortDetails(inputModel.Id);

					var viewModel = new AccountDetailsViewModel
					{
						Name = accShortDetails.Name,
						Balance = accShortDetails.Balance,
						CurrencyName = accShortDetails.CurrencyName
					};

					return View(viewModel);
				}
				catch (InvalidOperationException)
				{
					return BadRequest();
				}
			}

			try
			{
				AccountDetailsServiceModel accountDetails = await accountService
					.GetAccountDetails(inputModel.Id, inputModel.StartDate, inputModel.EndDate);

				var viewModel = new AccountDetailsViewModel
				{
					Id = accountDetails.Id,
					Balance = accountDetails.Balance,
					Name = accountDetails.Name,
					CurrencyName = accountDetails.CurrencyName,
					OwnerId = accountDetails.OwnerId,
					StartDate = inputModel.StartDate,
					EndDate = inputModel.EndDate,
					Transactions = accountDetails.Transactions
				};
				viewModel.Pagination.TotalElements = accountDetails.TotalAccountTransactions;
				viewModel.Routing.Area = "Admin";
				viewModel.Routing.ReturnUrl = "/Admin/Accounts/AccountDetails/" + accountDetails.Id;

				return View(viewModel);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
		}

		public async Task<IActionResult> Delete(string id)
		{
			if (!ModelState.IsValid)
				return BadRequest();

			try
			{
				string accountName = await accountService.GetAccountName(id);
				var viewModel = new DeleteAccountViewModel { Name = accountName };

				return View(viewModel);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
		}

		[HttpPost]
		public async Task<IActionResult> Delete(DeleteAccountInputModel inputModel)
		{
			if (!ModelState.IsValid)
				return BadRequest();

			if (inputModel.ConfirmButton == "reject")
				return LocalRedirect(inputModel.ReturnUrl);

			try
			{
				await accountService.DeleteAccount(
					inputModel.Id,
					inputModel.ShouldDeleteTransactions ?? false);

				TempData["successMsg"] = "You successfully delete user's account!";

				string ownerId = await accountService.GetOwnerId(inputModel.Id);

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
				AccountFormServiceModel accountData =
					await accountService.GetAccountFormData(id);

				var viewModel = new AccountFormViewModel
				{
					Name = accountData.Name,
					Balance = accountData.Balance,
					AccountTypeId = accountData.AccountTypeId,
					CurrencyId = accountData.CurrencyId,
					OwnerId = accountData.OwnerId,
					AccountTypes = accountData.AccountTypes,
					Currencies = accountData.Currencies
				};

				return View(viewModel);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
		}

		[HttpPost]
		public async Task<IActionResult> EditAccount(
			string id, AccountFormViewModel inputModel, string returnUrl)
		{
			if (!ModelState.IsValid)
			{
				string ownerId = await accountService.GetOwnerId(id);

				UserAccountTypesAndCurrenciesServiceModel userData =
					await usersService.GetUserAccountTypesAndCurrencies(ownerId);

				inputModel.AccountTypes = userData.AccountTypes;
				inputModel.Currencies = userData.Currencies;

				return View(inputModel);
			}

			try
			{
				var serviceModel = new AccountFormShortServiceModel
				{
					Name = inputModel.Name,
					Balance = inputModel.Balance ?? 0,
					OwnerId = inputModel.OwnerId,
					AccountTypeId = inputModel.AccountTypeId,
					CurrencyId = inputModel.CurrencyId
				};

				await accountService.EditAccount(id, serviceModel);

				TempData["successMsg"] = "You successfully edited user's account!";

				return LocalRedirect(returnUrl);
			}
			catch (ArgumentException)
			{
				ModelState.AddModelError(
					nameof(inputModel.Name),
					$"You already have Account with {inputModel.Name} name.");

				string ownerId = await accountService.GetOwnerId(id);

				UserAccountTypesAndCurrenciesServiceModel userData =
					await usersService.GetUserAccountTypesAndCurrencies(ownerId);

				inputModel.AccountTypes = userData.AccountTypes;
				inputModel.Currencies = userData.Currencies;

				return View(inputModel);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
		}
	}
}
