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
				await this.PrepareAccountsTypesCategoriesCurrenciesAsync(userId, viewModel);

				return this.View(viewModel);
			}

			viewModel = await this.PrepareUserTransactionsViewModelAsync(userId, inputModel);

			return this.View(viewModel);
		}

		[Authorize(Roles = UserRoleName)]
		public async Task<IActionResult> Create()
		{
			var viewModel = new TransactionFormModel() { OwnerId = this.User.IdToGuid() };
			await this.PrepareTransactionFormModelAsync(viewModel);
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
				await this.PrepareTransactionFormModelAsync(inputModel);

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
		public async Task<IActionResult> Delete(Guid id, string? returnUrl = null)
		{
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

		public async Task<IActionResult> TransactionDetails(Guid id)
		{
			try
			{
				return this.View(await this.accountsInfoService
					.GetTransactionDetailsAsync(id, this.User.IdToGuid(), this.User.IsAdmin()));
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

		public async Task<IActionResult> EditTransaction([Required] Guid? id)
		{
			TransactionFormModel viewModel;

			try
			{
				Guid transactionId = id ?? throw new InvalidOperationException("Transaction ID cannot be a null.");

				Guid ownerId = this.User.IsAdmin()
					? await this.accountsInfoService.GetTransactionOwnerIdAsync(transactionId)
					: this.User.IdToGuid();

				viewModel = await this.accountsInfoService.GetTransactionFormDataAsync(transactionId, ownerId);
			}
			catch (InvalidOperationException)
			{
				return this.BadRequest();
			}

			return this.View(viewModel);
		}

		[HttpPost]
		public async Task<IActionResult> EditTransaction([Required] Guid? id, TransactionFormModel inputModel)
		{
			try
			{
				Guid transactionId = id ?? throw new InvalidOperationException("Transaction ID cannot be a null.");

				if (!this.User.IsAdmin() && this.User.IdToGuid() != inputModel.OwnerId)
					return this.BadRequest();

				if (!this.ModelState.IsValid)
				{
					await this.PrepareTransactionFormModelAsync(inputModel);

					return this.View(inputModel);
				}

				await this.accountsUpdateService.EditTransactionAsync(transactionId, inputModel);
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

		private async Task PrepareTransactionFormModelAsync(TransactionFormModel formModel)
		{
			Guid ownerId = formModel.OwnerId ?? throw new InvalidOperationException("Owner ID cannot be a null.");
			formModel.UserAccounts = await this.usersService.GetUserAccountsDropdownData(ownerId);
			formModel.UserCategories = await this.usersService.GetUserCategoriesDropdownData(ownerId);
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

			await this.PrepareAccountsTypesCategoriesCurrenciesAsync(userId, viewModel);

			return viewModel;
		}

		private async Task PrepareAccountsTypesCategoriesCurrenciesAsync(Guid userId, UserTransactionsViewModel viewModel)
		{
			viewModel.Accounts = await this.usersService.GetUserAccountsDropdownData(userId);
			viewModel.Categories = await this.usersService.GetUserCategoriesDropdownData(userId);
			viewModel.AccountTypes = await this.usersService.GetUserAccountTypesDropdownData(userId);
			viewModel.Currencies = await this.usersService.GetUserCurrenciesDropdownData(userId);
		}
	}
}
