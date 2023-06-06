using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinancer.Services.Accounts;
using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Services.User;
using PersonalFinancer.Services.User.Models;
using PersonalFinancer.Web.Infrastructure.Extensions;
using PersonalFinancer.Web.Models.Account;
using static PersonalFinancer.Data.Constants.RoleConstants;

namespace PersonalFinancer.Web.Controllers
{
    [Authorize]
	public class AccountsController : Controller
	{
		protected readonly IAccountsService accountService;
		protected readonly IUsersService usersService;
		protected readonly IMapper mapper;

		public AccountsController(
			IAccountsService accountService,
			IMapper mapper,
			IUsersService usersService)
		{
			this.accountService = accountService;
			this.mapper = mapper;
			this.usersService = usersService;
		}

		[Authorize(Roles = UserRoleName)]
		public async Task<IActionResult> Create()
		{
			var viewModel = new AccountFormViewModel { OwnerId = User.Id() };
			await PrepareAccountFormViewModel(viewModel);

			return View(viewModel);
		}

		[Authorize(Roles = UserRoleName)]
		[HttpPost]
		public async Task<IActionResult> Create(AccountFormViewModel inputModel)
		{
			if (!ModelState.IsValid)
			{
				await PrepareAccountFormViewModel(inputModel);

				return View(inputModel);
			}

			if (inputModel.OwnerId != User.Id())
				return BadRequest();

			try
			{
				var accountServiceModel = mapper.Map<AccountFormShortServiceModel>(inputModel);
				string newAccountId = await accountService.CreateAccount(accountServiceModel);
				TempData["successMsg"] = "You create a new account successfully!";

				return RedirectToAction(nameof(AccountDetails), new { id = newAccountId });
			}
			catch (ArgumentException)
			{
				ModelState.AddModelError(nameof(inputModel.Name),
					"You already have Account with that name.");
				await PrepareAccountFormViewModel(inputModel);

				return View(inputModel);
			}
		}

		public async Task<IActionResult> AccountDetails(string id)
		{
			DateTime startDate = DateTime.Now.AddMonths(-1);
			DateTime endDate = DateTime.Now;

			try
			{
				AccountDetailsViewModel viewModel = await GetAccountDetailsViewModel(
					id, startDate, endDate, User.Id(), User.IsAdmin());

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

					var viewModel = mapper.Map<AccountDetailsViewModel>(accShortDetails);

					return View(viewModel);
				}
				catch (Exception)
				{
					return BadRequest();
				}
			}

			try
			{
				AccountDetailsViewModel viewModel = await GetAccountDetailsViewModel(
					inputModel.Id, inputModel.StartDate, inputModel.EndDate, User.Id(), User.IsAdmin());

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
				string accountName = await accountService
					.GetAccountName(id, User.Id(), User.IsAdmin());

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
				await accountService.DeleteAccount(inputModel.Id, User.Id(), User.IsAdmin(),
					inputModel.ShouldDeleteTransactions ?? false);

				if (User.IsAdmin())
				{
					TempData["successMsg"] = "You successfully delete user's account!";
					string ownerId = await accountService.GetOwnerId(inputModel.Id);

					return LocalRedirect("/Admin/Users/Details/" + ownerId);
				}
				else
				{
					TempData["successMsg"] = "Your account was successfully deleted!";

					return RedirectToAction("Index", "Home");
				}
			}
			catch (ArgumentException)
			{
				return Unauthorized();
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
					await accountService.GetAccountFormData(id, User.Id(), User.IsAdmin());

				var viewModel = mapper.Map<AccountFormViewModel>(accountData);

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
				await PrepareAccountFormViewModel(inputModel);

				return View(inputModel);
			}

			string ownerId = User.IsAdmin() ?
				await accountService.GetOwnerId(id)
				: User.Id();

			if (inputModel.OwnerId != ownerId)
				return BadRequest();

			try
			{
				var serviceModel = mapper.Map<AccountFormShortServiceModel>(inputModel);
				await accountService.EditAccount(id, serviceModel);

				TempData["successMsg"] = User.IsAdmin() ?
					"You successfully edited user's account!"
					: "Your account was successfully edited!";

				return LocalRedirect(returnUrl);
			}
			catch (ArgumentException)
			{
				ModelState.AddModelError(nameof(inputModel.Name), User.IsAdmin() ?
					$"The user already have Account with \"{inputModel.Name}\" name."
					: $"You already have Account with \"{inputModel.Name}\" name.");

				await PrepareAccountFormViewModel(inputModel);

				return View(inputModel);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
		}

		private async Task PrepareAccountFormViewModel(AccountFormViewModel viewModel)
		{
			UserAccountTypesAndCurrenciesServiceModel userData =
				await usersService.GetUserAccountTypesAndCurrencies(viewModel.OwnerId);

			viewModel.AccountTypes = userData.AccountTypes;
			viewModel.Currencies = userData.Currencies;
		}

		/// <summary>
		/// Throws InvalidOperationException when Account does not exist
		/// or User is not owner or Administrator.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		private async Task<AccountDetailsViewModel> GetAccountDetailsViewModel(
			string accountId, DateTime startDate, DateTime endDate, string userId, bool isUserAdmin)
		{
			AccountDetailsServiceModel accountDetails = await accountService
				.GetAccountDetails(accountId, startDate, endDate, userId, isUserAdmin);

			var viewModel = mapper.Map<AccountDetailsViewModel>(accountDetails);
			viewModel.Pagination.TotalElements = accountDetails.TotalAccountTransactions;

			if (isUserAdmin)
			{
				viewModel.Routing.Area = "Admin";
				viewModel.Routing.ReturnUrl = "/Admin/Accounts/AccountDetails/" + accountId;
			}
			else
			{
				viewModel.Routing.ReturnUrl = "/Accounts/AccountDetails/" + accountId;
			}

			return viewModel;
		}
	}
}