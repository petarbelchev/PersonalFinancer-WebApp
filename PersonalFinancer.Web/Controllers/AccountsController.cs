namespace PersonalFinancer.Web.Controllers
{
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;

	using Services.Accounts;
	using Services.Accounts.Models;
	using Services.User;
	using Services.User.Models;

	using Web.Infrastructure;
	using Web.Models.Account;

	using static Data.Constants.RoleConstants;

	[Authorize(Roles = UserRoleName)]
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

		public async Task<IActionResult> Create()
		{
			UserAccountTypesAndCurrenciesServiceModel userData =
				await usersService.GetUserAccountTypesAndCurrencies(this.User.Id());

			var viewModel = new AccountFormViewModel
			{
				AccountTypes = userData.AccountTypes,
				Currencies = userData.Currencies,
				OwnerId = User.Id()
			};

			return View(viewModel);
		}

		[HttpPost]
		public async Task<IActionResult> Create(AccountFormViewModel inputModel)
		{
			if (!ModelState.IsValid)
			{ // NOTE: Repeating code!!!
				UserAccountTypesAndCurrenciesServiceModel userData =
					await usersService.GetUserAccountTypesAndCurrencies(this.User.Id());

				inputModel.AccountTypes = userData.AccountTypes;
				inputModel.Currencies = userData.Currencies;

				return View(inputModel);
			}

			try
			{
				var accountServiceModel = new AccountFormShortServiceModel
				{
					Name = inputModel.Name,
					Balance = inputModel.Balance ?? 0,
					AccountTypeId = inputModel.AccountTypeId,
					CurrencyId = inputModel.CurrencyId,
					OwnerId = User.Id()
				};

				string newAccountId = await accountService.CreateAccount(accountServiceModel);

				TempData["successMsg"] = "You create a new account successfully!";

				return RedirectToAction(nameof(AccountDetails), new { id = newAccountId });
			}
			catch (ArgumentException)
			{
				ModelState.AddModelError(nameof(inputModel.Name), "You already have Account with that name.");

				UserAccountTypesAndCurrenciesServiceModel userData =
					await usersService.GetUserAccountTypesAndCurrencies(this.User.Id());

				inputModel.AccountTypes = userData.AccountTypes;
				inputModel.Currencies = userData.Currencies;

				return View(inputModel);
			}
		}

		public async Task<IActionResult> AccountDetails(string id)
		{
			DateTime startDate = DateTime.UtcNow.AddMonths(-1);
			DateTime endDate = DateTime.UtcNow;

			try
			{
				AccountDetailsServiceModel serviceModel = await accountService
					.GetAccountDetails(id, startDate, endDate, User.Id());
				// NOTE: Repeating code!!!
				var viewModel = new AccountDetailsViewModel
				{
					Id = serviceModel.Id,
					Balance = serviceModel.Balance,
					Name = serviceModel.Name,
					CurrencyName = serviceModel.CurrencyName,
					OwnerId = serviceModel.OwnerId,
					StartDate = startDate,
					EndDate = endDate,
					Transactions = serviceModel.Transactions
				};
				viewModel.Pagination.TotalElements = serviceModel.TotalAccountTransactions;
				viewModel.Routing.ReturnUrl = "/Accounts/AccountDetails/" + id;

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
				catch (Exception)
				{
					return BadRequest();
				}
			}

			try
			{
				AccountDetailsServiceModel accountDetails = await accountService
					.GetAccountDetails(inputModel.Id, inputModel.StartDate, inputModel.EndDate, User.Id());

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
				viewModel.Routing.ReturnUrl = "/Accounts/AccountDetails/" + inputModel.Id;

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
				string accountName = await accountService.GetAccountName(id, User.Id());
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
					inputModel.ShouldDeleteTransactions ?? false,
					User.Id());
			}
			catch (ArgumentException)
			{
				return Unauthorized();
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
				AccountFormServiceModel accountData =
					await accountService.GetAccountFormData(id, User.Id());

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
				UserAccountTypesAndCurrenciesServiceModel userData =
					await usersService.GetUserAccountTypesAndCurrencies(this.User.Id());

				inputModel.AccountTypes = userData.AccountTypes;
				inputModel.Currencies = userData.Currencies;

				return View(inputModel);
			}

			if (inputModel.OwnerId != User.Id())
				return Unauthorized();

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

				TempData["successMsg"] = "Your account was successfully edited!";

				return LocalRedirect(returnUrl);
			}
			catch (ArgumentException)
			{
				ModelState.AddModelError(nameof(inputModel.Name),
					$"You already have Account with \"{inputModel.Name}\" name.");
				
				UserAccountTypesAndCurrenciesServiceModel userData =
					await usersService.GetUserAccountTypesAndCurrencies(this.User.Id());

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