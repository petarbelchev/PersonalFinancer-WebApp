namespace PersonalFinancer.Web.Controllers
{
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;

	using Services.Accounts;
	using Services.Accounts.Models;
	using Services.Shared.Models;
	using Services.User;
	using Services.User.Models;
	
	using Web.Infrastructure;
	using Web.Models.Shared;
	using Web.Models.Transaction;
	
	using static Data.Constants.RoleConstants;

	[Authorize(Roles = UserRoleName)]
	public class TransactionsController : Controller
	{
		private readonly IAccountsService accountsService;
		private readonly IUsersService usersService;

		public TransactionsController(
			IAccountsService accountsService,
			IUsersService usersService)
		{
			this.accountsService = accountsService;
			this.usersService = usersService;
		}

		public async Task<IActionResult> All()
		{
			DateTime startDate = DateTime.UtcNow.AddMonths(-1);
			DateTime endDate = DateTime.UtcNow;

			TransactionsServiceModel userTransactions =
				await usersService.GetUserTransactions(User.Id(), startDate, endDate);

			var viewModel = new UserTransactionsViewModel
			{
				Id = User.Id(),
				StartDate = startDate,
				EndDate = endDate,
				Transactions = userTransactions.Transactions
			};
			viewModel.Pagination.TotalElements = userTransactions.TotalTransactionsCount;

			return View(viewModel);
		}

		[HttpPost]
		public async Task<IActionResult> All(DateFilterModel inputModel)
		{
			if (!ModelState.IsValid)
				return View(new UserTransactionsViewModel
				{
					StartDate = inputModel.StartDate,
					EndDate = inputModel.EndDate
				});

			TransactionsServiceModel userTransactions = await usersService
				.GetUserTransactions(User.Id(), inputModel.StartDate, inputModel.EndDate);

			var viewModel = new UserTransactionsViewModel
			{
				Id = User.Id(),
				StartDate = inputModel.StartDate,
				EndDate = inputModel.EndDate,
				Transactions = userTransactions.Transactions
			};
			viewModel.Pagination.TotalElements = userTransactions.TotalTransactionsCount;

			return View(viewModel);
		}

		public async Task<IActionResult> Create()
		{
			UserAccountsAndCategoriesServiceModel userData =
				await usersService.GetUserAccountsAndCategories(User.Id());

			var viewModel = new TransactionFormModel
			{
				UserAccounts = userData.UserAccounts,
				UserCategories = userData.UserCategories,
				CreatedOn = DateTime.UtcNow,
				OwnerId = User.Id()
			};

			return View(viewModel);
		}

		[HttpPost]
		public async Task<IActionResult> Create(TransactionFormModel inputModel)
		{
			if (!ModelState.IsValid)
			{
				UserAccountsAndCategoriesServiceModel userData =
					await usersService.GetUserAccountsAndCategories(User.Id());

				inputModel.UserCategories = userData.UserCategories;
				inputModel.UserAccounts = userData.UserAccounts;

				return View(inputModel);
			}

			try
			{
				var serviceModel = new TransactionFormShortServiceModel
				{
					OwnerId = User.Id(),
					AccountId = inputModel.AccountId,
					Amount = inputModel.Amount,
					CategoryId = inputModel.CategoryId,
					CreatedOn = inputModel.CreatedOn,
					Refference = inputModel.Refference,
					TransactionType = inputModel.TransactionType
				};

				string newTransactionId =
					await accountsService.CreateTransaction(serviceModel);

				TempData["successMsg"] = "You create a new transaction successfully!";

				return RedirectToAction(nameof(TransactionDetails), new { id = newTransactionId });
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
		}

		[HttpPost]
		public async Task<IActionResult> Delete(string id, string? returnUrl = null)
		{
			try
			{
				await accountsService.DeleteTransaction(id, User.Id());
			}
			catch (ArgumentException)
			{
				return Unauthorized();
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}

			TempData["successMsg"] = "Your transaction was successfully deleted!";

			if (returnUrl != null)
				return LocalRedirect(returnUrl);
			else
				return RedirectToAction("Index", "Home");
		}

		public async Task<IActionResult> TransactionDetails(string id)
		{
			try
			{
				return View(await accountsService.GetTransactionDetails(id));
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
		}

		public async Task<IActionResult> EditTransaction(string id)
		{
			try
			{
				TransactionFormServiceModel transactionData =
					await accountsService.GetTransactionFormData(id);

				if (User.Id() != transactionData.OwnerId)
					return Unauthorized();

				var viewModel = new TransactionFormModel
				{
					OwnerId = transactionData.OwnerId,
					AccountId = transactionData.AccountId,
					Amount = transactionData.Amount,
					TransactionType = transactionData.TransactionType,
					Refference = transactionData.Refference,
					CreatedOn = transactionData.CreatedOn,
					CategoryId = transactionData.CategoryId,
					UserAccounts = transactionData.UserAccounts,
					UserCategories = transactionData.UserCategories,
					IsInitialBalance = transactionData.IsInitialBalance
				};

				return View(viewModel);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
		}

		[HttpPost]
		public async Task<IActionResult> EditTransaction(
			string id, TransactionFormModel inputModel, string? returnUrl)
		{
			if (!ModelState.IsValid)
			{
				UserAccountsAndCategoriesServiceModel userData =
					await usersService.GetUserAccountsAndCategories(User.Id());
				
				inputModel.UserCategories = userData.UserCategories;
				inputModel.UserAccounts = userData.UserAccounts;

				return View(inputModel);
			}

			if (User.Id() != inputModel.OwnerId)
				return Unauthorized();

			var serviceModel = new TransactionFormShortServiceModel
			{
				Amount = inputModel.Amount,
				CategoryId = inputModel.CategoryId,
				AccountId = inputModel.AccountId,
				CreatedOn = inputModel.CreatedOn,
				OwnerId = inputModel.OwnerId,
				Refference = inputModel.Refference,
				TransactionType = inputModel.TransactionType
			};

			try
			{
				await accountsService.EditTransaction(id, serviceModel);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}

			TempData["successMsg"] = "Your transaction was successfully edited!";

			if (returnUrl != null)
				return LocalRedirect(returnUrl);

			return RedirectToAction(nameof(TransactionDetails), new { id });
		}
	}
}
