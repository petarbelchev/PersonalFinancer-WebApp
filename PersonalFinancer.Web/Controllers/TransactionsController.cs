namespace PersonalFinancer.Web.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using PersonalFinancer.Services.Accounts;
    using PersonalFinancer.Services.Accounts.Models;
    using PersonalFinancer.Services.Transactions;
    using PersonalFinancer.Services.Transactions.Models;
    using PersonalFinancer.Services.User;
    using PersonalFinancer.Services.User.Models;
    using PersonalFinancer.Web.Infrastructure.Extensions;
    using PersonalFinancer.Web.Models.Shared;
    using PersonalFinancer.Web.Models.Transaction;
    using static PersonalFinancer.Data.Constants.RoleConstants;

    [Authorize]
	public class TransactionsController : Controller
	{
		protected readonly IAccountsService accountsService;
		protected readonly IUsersService usersService;
		protected readonly ITransactionsInfoService transactionsInfoService;
		protected readonly IMapper mapper;

		public TransactionsController(
			IAccountsService accountsService,
			IUsersService usersService,
			ITransactionsInfoService transactionsInfoService,
			IMapper mapper)
		{
			this.accountsService = accountsService;
			this.usersService = usersService;
			this.transactionsInfoService = transactionsInfoService;
			this.mapper = mapper;
		}

		[Authorize(Roles = UserRoleName)]
		public async Task<IActionResult> All()
		{
			DateTime startDate = DateTime.Now.AddMonths(-1);
			DateTime endDate = DateTime.Now;

			UserTransactionsViewModel viewModel =
				await this.PrepareUserTransactionsViewModel(this.User.IdToGuid(), startDate, endDate);

			return this.View(viewModel);
		}

		[Authorize(Roles = UserRoleName)]
		[HttpPost]
		public async Task<IActionResult> All(DateFilterModel inputModel)
		{
			if (!this.ModelState.IsValid)
				return this.View(this.mapper.Map<UserTransactionsViewModel>(inputModel));

			UserTransactionsViewModel viewModel = await this.PrepareUserTransactionsViewModel(
				this.User.IdToGuid(), inputModel.StartDate, inputModel.EndDate);

			return this.View(viewModel);
		}

		[Authorize(Roles = UserRoleName)]
		public async Task<IActionResult> Create()
		{
			UserAccountsAndCategoriesServiceModel userData =
				await this.usersService.GetUserAccountsAndCategoriesAsync(this.User.IdToGuid());

			TransactionFormModel viewModel = this.mapper.Map<TransactionFormModel>(userData);
			viewModel.CreatedOn = DateTime.Now;

			return this.View(viewModel);
		}

		[Authorize(Roles = UserRoleName)]
		[HttpPost]
		public async Task<IActionResult> Create(TransactionFormModel inputModel)
		{
			if (!this.ModelState.IsValid)
			{
				await this.PrepareTransactionFormModelForReturn(inputModel);

				return this.View(inputModel);
			}

			if (inputModel.OwnerId != this.User.IdToGuid())
				return this.BadRequest();

			try
			{
				TransactionFormShortServiceModel serviceModel =
					this.mapper.Map<TransactionFormShortServiceModel>(inputModel);

				Guid newTransactionId = await this.accountsService.CreateTransactionAsync(serviceModel);
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
				await this.accountsService.DeleteTransactionAsync(id, this.User.IdToGuid(), this.User.IsAdmin());

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
				return this.View(await this.accountsService
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

		public async Task<IActionResult> EditTransaction(Guid id)
		{
			TransactionFormServiceModel transactionData;

			try
			{
				transactionData = await this.accountsService.GetTransactionFormDataAsync(id);
			}
			catch (InvalidOperationException)
			{
				return this.BadRequest();
			}

			if (!this.User.IsAdmin() && this.User.IdToGuid() != transactionData.OwnerId)
				return this.Unauthorized();

			TransactionFormModel viewModel = this.mapper.Map<TransactionFormModel>(transactionData);

			return this.View(viewModel);
		}

		[HttpPost]
		public async Task<IActionResult> EditTransaction(Guid id, TransactionFormModel inputModel)
		{
			if (!this.ModelState.IsValid)
			{
				await this.PrepareTransactionFormModelForReturn(inputModel);

				return this.View(inputModel);
			}

			try
			{
				Guid ownerId = this.User.IsAdmin()
					? await this.accountsService.GetOwnerIdAsync(inputModel.AccountId ?? throw new InvalidOperationException())
					: this.User.IdToGuid();

				if (inputModel.OwnerId != ownerId)
					return this.Unauthorized();

				TransactionFormShortServiceModel serviceModel = this.mapper.Map<TransactionFormShortServiceModel>(inputModel);

				await this.accountsService.EditTransactionAsync(id, serviceModel);
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

		private async Task PrepareTransactionFormModelForReturn(TransactionFormModel formModel)
		{
			if (formModel.OwnerId != null)
			{
				UserAccountsAndCategoriesServiceModel userData =
					await this.usersService.GetUserAccountsAndCategoriesAsync(
						formModel.OwnerId ?? throw new InvalidOperationException());

				formModel.UserCategories = userData.UserCategories;
				formModel.UserAccounts = userData.UserAccounts;
			}
		}

		private async Task<UserTransactionsViewModel> PrepareUserTransactionsViewModel(
			Guid userId, DateTime startDate, DateTime endDate)
		{
			TransactionsServiceModel userTransactions =
				await this.transactionsInfoService.GetUserTransactionsAsync(userId, startDate, endDate);

			UserTransactionsViewModel viewModel = this.mapper.Map<UserTransactionsViewModel>(userTransactions);
			viewModel.Id = userId;
			viewModel.Pagination.TotalElements = userTransactions.TotalTransactionsCount;

			return viewModel;
		}
	}
}
