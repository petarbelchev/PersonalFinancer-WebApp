namespace PersonalFinancer.Web.Controllers
{
	using AutoMapper;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using PersonalFinancer.Services.Accounts;
	using PersonalFinancer.Services.Accounts.Models;
	using PersonalFinancer.Services.User;
	using PersonalFinancer.Web.Extensions;
	using PersonalFinancer.Web.Models.Transaction;
	using System.ComponentModel.DataAnnotations;
	using static PersonalFinancer.Data.Constants.RoleConstants;

	[Authorize]
	public class TransactionsController : Controller
	{
		protected readonly IAccountsUpdateService accountsUpdateService;
		protected readonly IAccountsInfoService accountsInfoService;
		protected readonly IUsersService usersService;
		protected readonly IMapper mapper;

		public TransactionsController(
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
		public async Task<IActionResult> All()
		{
			var inputModel = new UserTransactionsInputModel
			{
				StartDate = DateTime.Now.AddMonths(-1),
				EndDate = DateTime.Now
			};

			UserTransactionsViewModel viewModel =
				await this.PrepareUserTransactionsViewModelAsync(this.User.IdToGuid(), inputModel);

			return this.View(viewModel);
		}

		[Authorize(Roles = UserRoleName)]
		[HttpPost]
		public async Task<IActionResult> All(UserTransactionsInputModel inputModel)
		{
			Guid userId = this.User.IdToGuid();
			UserTransactionsViewModel viewModel;

			if (!this.ModelState.IsValid)
			{
				viewModel = this.mapper.Map<UserTransactionsViewModel>(inputModel);
				viewModel.Id = userId;
				await this.PrepareAccountsTypesCategoriesAndCurrenciesAsync(viewModel);

				return this.View(viewModel);
			}

			viewModel = await this.PrepareUserTransactionsViewModelAsync(userId, inputModel);

			return this.View(viewModel);
		}

		[Authorize(Roles = UserRoleName)]
		public async Task<IActionResult> Create()
		{
			var viewModel = new TransactionFormModel() { OwnerId = this.User.IdToGuid() };
			await this.PrepareAccountsAndCategoriesAsync(viewModel);
			viewModel.CreatedOn = DateTime.Now;

			return this.View(viewModel);
		}

		[Authorize(Roles = UserRoleName)]
		[HttpPost]
		public async Task<IActionResult> Create(TransactionFormModel inputModel)
		{
			if (inputModel.OwnerId != this.User.IdToGuid())
				return this.BadRequest();

			if (!this.ModelState.IsValid)
			{
				await this.PrepareAccountsAndCategoriesAsync(inputModel);

				return this.View(inputModel);
			}

			try
			{
				Guid newTransactionId = await this.accountsUpdateService.CreateTransactionAsync(inputModel);
				this.TempData["successMsg"] = "You create a new transaction successfully!";

				return this.RedirectToAction(nameof(TransactionDetails), new { id = newTransactionId });
			}
			catch (InvalidOperationException)
			{
				return this.BadRequest();
			}
		}

		[HttpPost]
		public async Task<IActionResult> Delete([Required] Guid id, string? returnUrl = null)
		{
			if (!this.ModelState.IsValid)
				return this.BadRequest();

			try
			{
				await this.accountsUpdateService.DeleteTransactionAsync(id, this.User.IdToGuid(), this.User.IsAdmin());

				this.TempData["successMsg"] = this.User.IsAdmin() ?
					"You successfully delete a user's transaction!"
					: "Your transaction was successfully deleted!";
			}
			catch (ArgumentException)
			{
				return this.Unauthorized();
			}
			catch (InvalidOperationException)
			{
				return this.BadRequest();
			}

			return returnUrl != null
				? this.LocalRedirect(returnUrl)
				: this.RedirectToAction("Index", "Home");
		}

		public async Task<IActionResult> TransactionDetails([Required] Guid id)
		{
			if (!this.ModelState.IsValid)
				return this.BadRequest();

			TransactionDetailsServiceModel viewModel;

			try
			{
				viewModel = await this.accountsInfoService
					.GetTransactionDetailsAsync(id, this.User.IdToGuid(), this.User.IsAdmin());
			}
			catch (ArgumentException)
			{
				return this.Unauthorized();
			}
			catch (InvalidOperationException)
			{
				return this.BadRequest();
			}

			return this.View(viewModel);
		}

		public async Task<IActionResult> EditTransaction([Required] Guid id)
		{
			if (!this.ModelState.IsValid)
				return this.BadRequest();

			TransactionFormModel viewModel;

			try
			{
				Guid ownerId = this.User.IsAdmin()
					? await this.accountsInfoService.GetTransactionOwnerIdAsync(id)
					: this.User.IdToGuid();

				viewModel = await this.accountsInfoService.GetTransactionFormDataAsync(id, ownerId);
			}
			catch (InvalidOperationException)
			{
				return this.BadRequest();
			}

			return this.View(viewModel);
		}

		[HttpPost]
		public async Task<IActionResult> EditTransaction([Required] Guid id, TransactionFormModel inputModel)
		{
			try
			{
				if (!this.User.IsAdmin() && this.User.IdToGuid() != inputModel.OwnerId)
					return this.BadRequest();

				if (!this.ModelState.IsValid)
				{
					await this.PrepareAccountsAndCategoriesAsync(inputModel);

					return this.View(inputModel);
				}

				await this.accountsUpdateService.EditTransactionAsync(id, inputModel);
			}
			catch (InvalidOperationException)
			{
				return this.BadRequest();
			}

			this.TempData["successMsg"] = this.User.IsAdmin()
				? "You successfully edit User's transaction!"
				: "Your transaction was successfully edited!";

			return this.RedirectToAction(nameof(TransactionDetails), new { id });
		}

		/// <summary>
		/// Throws InvalidOperationException when Owner ID is invalid.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		private async Task PrepareAccountsAndCategoriesAsync(TransactionFormModel formModel)
		{
			Guid ownerId = formModel.OwnerId ?? throw new InvalidOperationException("Owner ID cannot be null.");
			formModel.UserAccounts = await this.usersService.GetUserAccountsDropdownData(ownerId, withDeleted: false);
			formModel.UserCategories = await this.usersService.GetUserCategoriesDropdownData(ownerId, withDeleted: false);
		}

		private async Task<UserTransactionsViewModel> PrepareUserTransactionsViewModelAsync(
			Guid userId, UserTransactionsInputModel inputModel)
		{
			UserTransactionsViewModel viewModel =
				this.mapper.Map<UserTransactionsViewModel>(inputModel);

			TransactionsServiceModel userTransactions =
				await this.accountsInfoService.GetUserTransactionsAsync(userId, inputModel);

			viewModel.Id = userId;
			viewModel.Transactions = userTransactions.Transactions;
			viewModel.Pagination.TotalElements = userTransactions.TotalTransactionsCount;

			await this.PrepareAccountsTypesCategoriesAndCurrenciesAsync(viewModel);

			return viewModel;
		}

		private async Task PrepareAccountsTypesCategoriesAndCurrenciesAsync(UserTransactionsViewModel viewModel)
		{
			viewModel.Accounts = await this.usersService.GetUserAccountsDropdownData(viewModel.Id, true);
			viewModel.Categories = await this.usersService.GetUserCategoriesDropdownData(viewModel.Id, true);
			viewModel.AccountTypes = await this.usersService.GetUserAccountTypesDropdownData(viewModel.Id, true);
			viewModel.Currencies = await this.usersService.GetUserCurrenciesDropdownData(viewModel.Id, true);
		}
	}
}
