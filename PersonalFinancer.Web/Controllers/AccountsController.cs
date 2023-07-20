namespace PersonalFinancer.Web.Controllers
{
	using AutoMapper;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using PersonalFinancer.Common.Messages;
	using PersonalFinancer.Services.Accounts;
	using PersonalFinancer.Services.Accounts.Models;
	using PersonalFinancer.Services.User;
	using PersonalFinancer.Services.User.Models;
	using PersonalFinancer.Web.CustomAttributes;
	using PersonalFinancer.Web.Extensions;
	using PersonalFinancer.Web.Models.Account;
	using System.ComponentModel.DataAnnotations;
	using static PersonalFinancer.Common.Constants.RoleConstants;

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
			var viewModel = new CreateEditAccountViewModel { OwnerId = this.User.IdToGuid() };
			await this.SetUserDropdownDataToViewModel(viewModel);

			return this.View(viewModel);
		}

		[Authorize(Roles = UserRoleName)]
		[HttpPost]
		public async Task<IActionResult> Create(CreateEditAccountInputModel inputModel)
		{
			if (inputModel.OwnerId != this.User.IdToGuid())
				return this.BadRequest();

			if (this.ModelState.IsValid)
			{
				try
				{
					CreateEditAccountInputDTO accountDTO =
						this.mapper.Map<CreateEditAccountInputDTO>(inputModel);

					Guid newAccountId = await this.accountsUpdateService.CreateAccountAsync(accountDTO);
					this.TempData[ResponseMessages.TempDataKey] = ResponseMessages.CreatedAccount;

					return this.RedirectToAction(nameof(Details), new { id = newAccountId });
				}
				catch (ArgumentException ex)
				{
					this.ModelState.AddModelError(nameof(inputModel.Name), ex.Message);
				}
			}

			CreateEditAccountViewModel viewModel =
				this.mapper.Map<CreateEditAccountViewModel>(inputModel);

			await this.SetUserDropdownDataToViewModel(viewModel);

			return this.View(viewModel);
		}

		public async Task<IActionResult> Delete([Required] Guid id, string? returnUrl)
		{
			if (!this.ModelState.IsValid)
				return this.BadRequest();

			var viewModel = new DeleteAccountViewModel { ReturnUrl = returnUrl };

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
		[NoHtmlSanitizing]
		public async Task<IActionResult> Delete(DeleteAccountInputModel inputModel)
		{
			if (!this.ModelState.IsValid)
				return this.BadRequest();

			if (inputModel.ConfirmButton == "reject")
				return this.RedirectToAction(nameof(Details), new { inputModel.Id, inputModel.ReturnUrl });

			try
			{
				Guid accountId = inputModel.Id ?? throw new InvalidOperationException();

				await this.accountsUpdateService.DeleteAccountAsync(
					accountId, this.User.IdToGuid(), this.User.IsAdmin(),
					inputModel.ShouldDeleteTransactions ?? false);

				if (this.User.IsAdmin())
				{
					this.TempData[ResponseMessages.TempDataKey] = ResponseMessages.AdminDeletedUserAccount;

					if (inputModel.ReturnUrl != null)
						return this.LocalRedirect(inputModel.ReturnUrl);
				}
				else
				{
					this.TempData[ResponseMessages.TempDataKey] = ResponseMessages.DeletedAccount;
				}

				return this.LocalRedirect("/");
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

		public async Task<IActionResult> Details([Required] Guid id, string? returnUrl)
		{
			if (!this.ModelState.IsValid)
				return this.BadRequest();

			AccountDetailsDTO accountDetails;

			try
			{
				accountDetails = await this.accountsInfoService
					.GetAccountDetailsAsync(id, this.User.IdToGuid(), this.User.IsAdmin());
			}
			catch (InvalidOperationException)
			{
				return this.BadRequest();
			}

			AccountDetailsViewModel viewModel = this.mapper.Map<AccountDetailsViewModel>(accountDetails);
			viewModel.FromLocalTime = DateTime.Now.AddMonths(-1);
			viewModel.ToLocalTime = DateTime.Now;
			viewModel.ReturnUrl = returnUrl;

			return this.View(viewModel);
		}

		public async Task<IActionResult> Filtered(AccountDetailsInputModel inputModel)
		{
			AccountDetailsDTO accountDetails;

			try
			{
				Guid accountId = inputModel.Id ??
					throw new InvalidOperationException(string.Format(
						ExceptionMessages.NotNullableProperty, inputModel.Id));

				accountDetails = await this.accountsInfoService
					.GetAccountDetailsAsync(accountId, this.User.IdToGuid(), this.User.IsAdmin());
			}
			catch (InvalidOperationException)
			{
				return this.BadRequest();
			}

			AccountDetailsViewModel viewModel = this.mapper.Map<AccountDetailsViewModel>(accountDetails);
			this.mapper.Map(inputModel, viewModel);

			return this.View(viewModel);
		}

		public async Task<IActionResult> Edit([Required] Guid id)
		{
			if (!this.ModelState.IsValid)
				return this.BadRequest();

			CreateEditAccountOutputDTO accountDTO;

			try
			{
				accountDTO = await this.accountsInfoService
					.GetAccountFormDataAsync(id, this.User.IdToGuid(), this.User.IsAdmin());
			}
			catch (InvalidOperationException)
			{
				return this.BadRequest();
			}

			CreateEditAccountViewModel viewModel =
				this.mapper.Map<CreateEditAccountViewModel>(accountDTO);

			return this.View(viewModel);
		}

		[HttpPost]
		public async Task<IActionResult> Edit([Required] Guid id, CreateEditAccountInputModel inputModel)
		{
			if (!this.User.IsAdmin() && this.User.IdToGuid() != inputModel.OwnerId)
				return this.BadRequest();

			if (this.ModelState.IsValid)
			{
				try
				{
					CreateEditAccountInputDTO accountDTO =
						this.mapper.Map<CreateEditAccountInputDTO>(inputModel);

					await this.accountsUpdateService.EditAccountAsync(id, accountDTO);

					this.TempData[ResponseMessages.TempDataKey] = this.User.IsAdmin()
						? ResponseMessages.AdminEditedUserAccount
						: ResponseMessages.EditedAccount;

					return this.RedirectToAction(nameof(Details), new { id });
				}
				catch (ArgumentException ex)
				{
					this.ModelState.AddModelError(nameof(inputModel.Name), this.User.IsAdmin()
						? string.Format(ExceptionMessages.AdminExistingUserEntityName, "account", inputModel.Name)
						: ex.Message);
				}
				catch (InvalidOperationException)
				{
					return this.BadRequest();
				}
			}

			CreateEditAccountViewModel viewModel =
				this.mapper.Map<CreateEditAccountViewModel>(inputModel);

			await this.SetUserDropdownDataToViewModel(viewModel);

			return this.View(viewModel);
		}

		/// <exception cref="InvalidOperationException">When the owner ID is invalid.</exception>
		private async Task SetUserDropdownDataToViewModel(CreateEditAccountViewModel viewModel)
		{
			Guid userId = viewModel.OwnerId ?? throw new InvalidOperationException(
				string.Format(ExceptionMessages.NotNullableProperty, viewModel.OwnerId));

			AccountTypesAndCurrenciesDropdownDTO typesAndCurrenciesDTO =
				await this.usersService.GetUserAccountTypesAndCurrenciesDropdownsAsync(userId);

			this.mapper.Map(typesAndCurrenciesDTO, viewModel);
		}
	}
}