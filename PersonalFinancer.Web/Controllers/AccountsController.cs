namespace PersonalFinancer.Web.Controllers
{
	using AutoMapper;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using PersonalFinancer.Services.Accounts;
	using PersonalFinancer.Services.Accounts.Models;
	using PersonalFinancer.Services.User;
	using PersonalFinancer.Web.Extensions;
	using PersonalFinancer.Web.Models.Account;
	using System.ComponentModel.DataAnnotations;
	using static PersonalFinancer.Data.Constants.RoleConstants;
	using static PersonalFinancer.Web.Constants;

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
			await this.SetUserDropdownDataToViewModel(viewModel);

			return this.View(viewModel);
		}

		[Authorize(Roles = UserRoleName)]
		[HttpPost]
		public async Task<IActionResult> Create(AccountFormViewModel inputModel)
		{
			if (inputModel.OwnerId != this.User.IdToGuid())
				return this.BadRequest();

			if (!this.ModelState.IsValid)
			{
				await this.SetUserDropdownDataToViewModel(inputModel);

				return this.View(inputModel);
			}

			try
			{
				CreateEditAccountDTO accountDTO =
					this.mapper.Map<CreateEditAccountDTO>(inputModel);

				Guid newAccountId = await this.accountsUpdateService.CreateAccountAsync(accountDTO);
				this.TempData["successMsg"] = "You create a new account successfully!";

				return this.RedirectToAction(nameof(AccountDetails), new { id = newAccountId });
			}
			catch (ArgumentException)
			{
				this.ModelState.AddModelError(
					nameof(inputModel.Name),
					"You already have Account with that name.");

				await this.SetUserDropdownDataToViewModel(inputModel);

				return this.View(inputModel);
			}
		}

		public async Task<IActionResult> AccountDetails([Required] Guid id)
		{
			if (!this.ModelState.IsValid)
				return this.BadRequest();

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
					AccountDetailsShortDTO accountDetails =
						await this.accountsInfoService.GetAccountShortDetailsAsync(accountId);

					viewModel = this.mapper.Map<AccountDetailsViewModel>(accountDetails);
				}
				else
				{
					viewModel = await this.GetAccountDetailsViewModel(
						accountId,
						inputModel.StartDate ?? throw new InvalidOperationException("Start Date cannot be null."),
						inputModel.EndDate ?? throw new InvalidOperationException("End Date cannot be null."),
						this.User.IdToGuid(),
						this.User.IsAdmin());
				}
			}
			catch (InvalidOperationException)
			{
				return this.BadRequest();
			}

			return this.View(viewModel);
		}

		public async Task<IActionResult> Delete([Required] Guid id)
		{
			if (!this.ModelState.IsValid)
				return this.BadRequest();

			var viewModel = new DeleteAccountViewModel();

			try
			{
				viewModel.Name = await this.accountsInfoService
					.GetAccountNameAsync(id, this.User.IdToGuid(), this.User.IsAdmin());
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
					Guid ownerId = await this.accountsInfoService.GetAccountOwnerIdAsync(accountId);

					return this.LocalRedirect(UrlPathConstants.AdminUserDetailsPath + ownerId);
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

		public async Task<IActionResult> EditAccount([Required] Guid id)
		{
			if (!this.ModelState.IsValid)
				return this.BadRequest();

			CreateEditAccountDTO accountDTO;

			try
			{
				accountDTO = await this.accountsInfoService
					.GetAccountFormDataAsync(id, this.User.IdToGuid(), this.User.IsAdmin());
			}
			catch (InvalidOperationException)
			{
				return this.BadRequest();
			}

			AccountFormViewModel viewModel =
				this.mapper.Map<AccountFormViewModel>(accountDTO);

			return this.View(viewModel);
		}

		[HttpPost]
		public async Task<IActionResult> EditAccount(
			[Required] Guid id, AccountFormViewModel inputModel, string returnUrl)
		{
			if (!this.User.IsAdmin() && this.User.IdToGuid() != inputModel.OwnerId)
				return this.BadRequest();

			try
			{
				if (!this.ModelState.IsValid)
				{
					await this.SetUserDropdownDataToViewModel(inputModel);

					return this.View(inputModel);
				}

				CreateEditAccountDTO accountDTO =
					this.mapper.Map<CreateEditAccountDTO>(inputModel);

				await this.accountsUpdateService.EditAccountAsync(id, accountDTO);
			}
			catch (ArgumentException)
			{
				this.ModelState.AddModelError(nameof(inputModel.Name), this.User.IsAdmin()
					? $"The user already have Account with \"{inputModel.Name}\" name."
					: $"You already have Account with \"{inputModel.Name}\" name.");

				await this.SetUserDropdownDataToViewModel(inputModel);

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

		/// <summary>
		/// Throws InvalidOperationException when Owner ID is invalid.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		private async Task SetUserDropdownDataToViewModel(AccountFormViewModel viewModel)
		{
			Guid userId = viewModel.OwnerId ?? throw new InvalidOperationException();
			var typesAndCurrenciesDTO = await this.usersService.GetUserAccountTypesAndCurrenciesDropdownDataAsync(userId);
			this.mapper.Map(typesAndCurrenciesDTO, viewModel);
		}

		/// <summary>
		/// Throws InvalidOperationException when Account does not exist
		/// or User is not owner or Administrator.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		private async Task<AccountDetailsViewModel> GetAccountDetailsViewModel(
			Guid accountId, DateTime startDate, DateTime endDate, Guid userId, bool isUserAdmin)
		{
			AccountDetailsLongDTO accountDetails = await this.accountsInfoService
				.GetAccountDetailsAsync(accountId, startDate, endDate, userId, isUserAdmin);

			var viewModel = new AccountDetailsViewModel(accountDetails.TotalAccountTransactions);
			this.mapper.Map(accountDetails, viewModel);

			viewModel.Routing.ReturnUrl = UrlPathConstants.AccountDetailsPath + accountId;

			return viewModel;
		}
	}
}