namespace PersonalFinancer.Web.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using PersonalFinancer.Services.Accounts;
    using PersonalFinancer.Services.Accounts.Models;
    using PersonalFinancer.Services.User;
    using PersonalFinancer.Services.User.Models;
    using PersonalFinancer.Web.Extensions;
    using PersonalFinancer.Web.Models.Account;
    using System.ComponentModel.DataAnnotations;
    using static PersonalFinancer.Data.Constants.RoleConstants;

    [Authorize]
	public class AccountsController : Controller
	{
		protected readonly IAccountsUpdateService accountsUpdateService;
		protected readonly IAccountsInfoService accountsInfoService;
		protected readonly IUsersService usersService;
		protected readonly IMapper mapper;

		public AccountsController(
			IAccountsUpdateService accountsUpdateService,
			IAccountsInfoService accountsInfoService,
			IUsersService usersService,
			IMapper mapper)
		{
			this.accountsUpdateService = accountsUpdateService;
			this.accountsInfoService = accountsInfoService;
			this.usersService = usersService;
			this.mapper = mapper;
		}

		[Authorize(Roles = UserRoleName)]
		public async Task<IActionResult> Create()
		{
			var viewModel = new AccountFormViewModel { OwnerId = this.User.IdToGuid() };
			await this.PrepareAccountFormViewModel(viewModel);

			return this.View(viewModel);
		}

		[Authorize(Roles = UserRoleName)]
		[HttpPost]
		public async Task<IActionResult> Create(AccountFormViewModel inputModel)
		{
			if (!this.ModelState.IsValid)
			{
				await this.PrepareAccountFormViewModel(inputModel);

				return this.View(inputModel);
			}

			if (inputModel.OwnerId != this.User.IdToGuid())
				return this.BadRequest();

			try
			{
				AccountFormShortServiceModel accountServiceModel =
					this.mapper.Map<AccountFormShortServiceModel>(inputModel);

				Guid newAccountId = await this.accountsUpdateService.CreateAccountAsync(accountServiceModel);
				this.TempData["successMsg"] = "You create a new account successfully!";

				return this.RedirectToAction(nameof(AccountDetails), new { id = newAccountId });
			}
			catch (ArgumentException)
			{
				this.ModelState.AddModelError(nameof(inputModel.Name),
					"You already have Account with that name.");
				await this.PrepareAccountFormViewModel(inputModel);

				return this.View(inputModel);
			}
		}

		public async Task<IActionResult> AccountDetails(Guid id)
		{
			DateTime startDate = DateTime.Now.AddMonths(-1);
			DateTime endDate = DateTime.Now;

			AccountDetailsViewModel viewModel;

			try
			{
				viewModel = await this.GetAccountDetailsViewModel(
					id, startDate, endDate, this.User.IdToGuid(), this.User.IsAdmin());
			}
			catch (InvalidOperationException)
			{
				return this.BadRequest();
			}

			return this.View(viewModel);
		}

		[HttpPost]
		public async Task<IActionResult> AccountDetails(AccountDetailsInputModel inputModel)
		{
			AccountDetailsViewModel viewModel;

			try
			{
				Guid accountId = inputModel.Id ?? throw new InvalidOperationException();

				if (!this.ModelState.IsValid)
				{
					AccountDetailsShortServiceModel accountDetails =
						await this.accountsInfoService.GetAccountShortDetailsAsync(accountId);

					viewModel = this.mapper.Map<AccountDetailsViewModel>(accountDetails);
				}
				else
				{
					viewModel = await this.GetAccountDetailsViewModel(
						accountId, inputModel.StartDate, inputModel.EndDate, this.User.IdToGuid(), this.User.IsAdmin());
				}
			}
			catch (InvalidOperationException)
			{
				return this.BadRequest();
			}

			return this.View(viewModel);
		}

		public async Task<IActionResult> Delete([Required] Guid? id)
		{
			if (!this.ModelState.IsValid)
				return this.BadRequest();

			var viewModel = new DeleteAccountViewModel();

			try
			{
				Guid accountId = id ?? throw new InvalidOperationException();

				viewModel.Name = await this.accountsInfoService
					.GetAccountNameAsync(accountId, this.User.IdToGuid(), this.User.IsAdmin());
			}
			catch (InvalidOperationException)
			{
				return this.BadRequest();
			}

			return this.View(viewModel);
		}

		[HttpPost]
		public async Task<IActionResult> Delete(DeleteAccountInputModel inputModel)
		{
			if (!this.ModelState.IsValid)
				return this.BadRequest();

			if (inputModel.ConfirmButton == "reject")
				return this.LocalRedirect(inputModel.ReturnUrl);

			try
			{
				Guid accountId = inputModel.Id ?? throw new InvalidOperationException();

				await this.accountsUpdateService.DeleteAccountAsync(
					accountId, this.User.IdToGuid(), this.User.IsAdmin(),
					inputModel.ShouldDeleteTransactions ?? false);

				if (this.User.IsAdmin())
				{
					this.TempData["successMsg"] = "You successfully delete user's account!";
					Guid ownerId = await this.accountsInfoService.GetOwnerIdAsync(accountId);

					return this.LocalRedirect("/Admin/Users/Details/" + ownerId);
				}
				else
				{
					this.TempData["successMsg"] = "Your account was successfully deleted!";

					return this.RedirectToAction("Index", "Home");
				}
			}
			catch (ArgumentException)
			{
				return this.Unauthorized();
			}
			catch (InvalidOperationException)
			{
				return this.BadRequest();
			}
		}

		public async Task<IActionResult> EditAccount([Required] Guid? id)
		{
			if (!this.ModelState.IsValid)
				return this.BadRequest();

			AccountFormServiceModel accountData;

			try
			{
				Guid accountId = id ?? throw new InvalidOperationException();

				accountData = await this.accountsInfoService
					.GetAccountFormDataAsync(accountId, this.User.IdToGuid(), this.User.IsAdmin());
			}
			catch (InvalidOperationException)
			{
				return this.BadRequest();
			}

			AccountFormViewModel viewModel = this.mapper.Map<AccountFormViewModel>(accountData);

			return this.View(viewModel);
		}

		[HttpPost]
		public async Task<IActionResult> EditAccount(
			[Required] Guid? id, AccountFormViewModel inputModel, string returnUrl)
		{
			if (!this.ModelState.IsValid)
			{
				await this.PrepareAccountFormViewModel(inputModel);

				return this.View(inputModel);
			}

			try
			{
				Guid accountId = id ?? throw new InvalidOperationException();

				Guid ownerId = this.User.IsAdmin()
					? await this.accountsInfoService.GetOwnerIdAsync(accountId)
					: this.User.IdToGuid();

				if (inputModel.OwnerId != ownerId)
					return this.BadRequest();

				AccountFormShortServiceModel serviceModel =
					this.mapper.Map<AccountFormShortServiceModel>(inputModel);

				await this.accountsUpdateService.EditAccountAsync(accountId, serviceModel);
			}
			catch (ArgumentException)
			{
				this.ModelState.AddModelError(nameof(inputModel.Name), this.User.IsAdmin()
					? $"The user already have Account with \"{inputModel.Name}\" name."
					: $"You already have Account with \"{inputModel.Name}\" name.");

				await this.PrepareAccountFormViewModel(inputModel);

				return this.View(inputModel);
			}
			catch (InvalidOperationException)
			{
				return this.BadRequest();
			}

			this.TempData["successMsg"] = this.User.IsAdmin()
				? "You successfully edited user's account!"
				: "Your account was successfully edited!";

			return this.LocalRedirect(returnUrl);
		}

		private async Task PrepareAccountFormViewModel(AccountFormViewModel viewModel)
		{
			if (viewModel.OwnerId != null)
			{
				UserAccountTypesAndCurrenciesServiceModel userData =
					await this.usersService.GetUserAccountTypesAndCurrenciesAsync(
						viewModel.OwnerId ?? throw new InvalidOperationException());

				viewModel.AccountTypes = userData.AccountTypes;
				viewModel.Currencies = userData.Currencies;
			}
		}

		/// <summary>
		/// Throws InvalidOperationException when Account does not exist
		/// or User is not owner or Administrator.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		private async Task<AccountDetailsViewModel> GetAccountDetailsViewModel(
			Guid accountId, DateTime startDate, DateTime endDate, Guid userId, bool isUserAdmin)
		{
			AccountDetailsServiceModel accountDetails = await this.accountsInfoService
				.GetAccountDetailsAsync(accountId, startDate, endDate, userId, isUserAdmin);

			AccountDetailsViewModel viewModel = this.mapper.Map<AccountDetailsViewModel>(accountDetails);
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