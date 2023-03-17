using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Data.Enums;
using PersonalFinancer.Services.Accounts;
using PersonalFinancer.Services.Categories;
using PersonalFinancer.Services.Shared.Models;
using PersonalFinancer.Services.Transactions;
using PersonalFinancer.Services.Transactions.Models;
using PersonalFinancer.Web.Infrastructure;
using static PersonalFinancer.Data.Constants.RoleConstants;

namespace PersonalFinancer.Web.Controllers
{
	[Authorize(Roles = UserRoleName)]
	public class TransactionController : Controller
	{
		private readonly ICategoryService categoryService;
		private readonly IAccountService accountService;
		private readonly ITransactionsService transactionsService;

		public TransactionController(
			ICategoryService categoryService,
			IAccountService accountService,
			ITransactionsService transactionsService)
		{
			this.categoryService = categoryService;
			this.accountService = accountService;
			this.transactionsService = transactionsService;
		}

		public async Task<IActionResult> All(string? startDate, string? endDate, int page = 1)
		{
			var viewModel = new UserTransactionsExtendedViewModel();

			viewModel.Pagination.Page = page;

			if (startDate == null || endDate == null)
			{
				viewModel.Dates = new DateFilterModel
				{
					StartDate = DateTime.UtcNow.AddMonths(-1),
					EndDate = DateTime.UtcNow
				};
			}
			else
			{
				viewModel.Dates = new DateFilterModel
				{
					StartDate = DateTime.Parse(startDate),
					EndDate = DateTime.Parse(endDate)
				};
			}

			try
			{
				await transactionsService.GetUserTransactionsExtendedViewModel(User.Id(), viewModel);

				return View(viewModel);
			}
			catch (ArgumentException ex)
			{
				ModelState.AddModelError(nameof(viewModel.Dates.EndDate), ex.Message);

				return View(viewModel);
			}
		}

		[HttpPost]
		public async Task<IActionResult> All(DateFilterModel dateModel)
		{
			if (!ModelState.IsValid)
				return View(dateModel);

			var viewModel = new UserTransactionsExtendedViewModel
			{
				Dates = dateModel
			};

			try
			{
				await transactionsService.GetUserTransactionsExtendedViewModel(User.Id(), viewModel);

				return View(viewModel);
			}
			catch (ArgumentException ex)
			{
				ModelState.AddModelError(nameof(viewModel.Dates.EndDate), ex.Message);

				return View(viewModel);
			}
		}

		public async Task<IActionResult> Create()
		{
			string userId = User.Id();

			var viewModel = new TransactionFormModel()
			{
				CreatedOn = DateTime.UtcNow
			};

			viewModel.Categories.AddRange(await categoryService.GetUserCategories(userId));
			viewModel.Accounts.AddRange(await accountService.GetUserAccountsDropdownViewModel(userId));

			return View(viewModel);
		}

		[HttpPost]
		public async Task<IActionResult> Create(TransactionFormModel inputModel)
		{
			if (!ModelState.IsValid)
			{
				inputModel.Categories.AddRange(
					await categoryService.GetUserCategories(User.Id()));

				inputModel.Accounts.AddRange(
					await accountService.GetUserAccountsDropdownViewModel(User.Id()));

				return View(inputModel);
			}

			try
			{
				string newTransactionId = await transactionsService.CreateTransaction(inputModel);

				TempData["successMsg"] = "You create a new transaction successfully!";

				return RedirectToAction(nameof(Details), new { id = newTransactionId });
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
				await transactionsService.DeleteTransaction(id);
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

		public async Task<IActionResult> Details(string id)
		{
			try
			{
				return View(await transactionsService.GetTransactionViewModel(id));
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
				TransactionFormModel viewModel = await transactionsService
					.GetTransactionFormModel(id);

				if (!await accountService.IsAccountOwner(User.Id(), viewModel.AccountId))
					return Unauthorized();

				bool isInitialBalance = await categoryService.IsInitialBalance(viewModel.CategoryId);

				if (isInitialBalance)
				{
					viewModel.Categories.Add(await categoryService
						.GetCategoryViewModel(viewModel.CategoryId));
				}
				else
				{
					viewModel.Categories.AddRange(await categoryService
						.GetUserCategories(User.Id()));

					viewModel.TransactionTypes.Add(TransactionType.Expense);
				}

				if (await accountService.IsAccountDeleted(viewModel.AccountId) || isInitialBalance)
				{
					viewModel.Accounts.Add(await accountService
						.GetAccountDropdownViewModel(viewModel.AccountId));
				}
				else
				{
					viewModel.Accounts.AddRange(await accountService
						.GetUserAccountsDropdownViewModel(User.Id()));
				}

				viewModel.TransactionTypes.Add(TransactionType.Income);

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
			if (!await accountService.IsAccountOwner(User.Id(), inputModel.AccountId))
				return Unauthorized();

			if (!ModelState.IsValid)
			{
				inputModel.Categories.AddRange(await categoryService
					.GetUserCategories(User.Id()));

				inputModel.Accounts.AddRange(await accountService
					.GetUserAccountsDropdownViewModel(User.Id()));

				return View(inputModel);
			}

			try
			{
				await transactionsService.EditTransaction(id, inputModel);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}

			TempData["successMsg"] = "Your transaction was successfully edited!";

			if (returnUrl != null)
				return LocalRedirect(returnUrl);

			return RedirectToAction(nameof(Details), new { id });
		}
	}
}
