using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Data.Enums;
using PersonalFinancer.Services.Accounts;
using PersonalFinancer.Services.Categories;
using PersonalFinancer.Services.Transactions;
using PersonalFinancer.Services.Transactions.Models;
using static PersonalFinancer.Data.Constants.RoleConstants;

namespace PersonalFinancer.Web.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = AdminRoleName)]
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

		[HttpPost]
		public async Task<IActionResult> Delete(string id, string? returnUrl)
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

		public async Task<IActionResult> EditTransaction(string id)
		{
			try
			{
				TransactionFormModel viewModel = await transactionsService
					.GetTransactionFormModel(id);

				bool isInitialBalance = await categoryService.IsInitialBalance(viewModel.CategoryId);
				string ownerId = await accountService.GetOwnerId(viewModel.AccountId);

				if (isInitialBalance)
				{
					viewModel.Categories.Add(await categoryService
						.GetCategoryViewModel(viewModel.CategoryId));
				}
				else
				{
					viewModel.Categories.AddRange(await categoryService
						.GetUserCategories(ownerId));

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
						.GetUserAccountsDropdownViewModel(ownerId));
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
			if (!ModelState.IsValid)
			{
				string ownerId = await accountService.GetOwnerId(inputModel.AccountId);

				inputModel.Categories.AddRange(await categoryService
					.GetUserCategories(ownerId));

				inputModel.Accounts.AddRange(await accountService
					.GetUserAccountsDropdownViewModel(ownerId));

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

			TempData["successMsg"] = "You successfully edit User's transaction!";

			if (returnUrl != null)
				return LocalRedirect(returnUrl);

			return RedirectToAction("Details", "Account", new { id = inputModel.AccountId });
		}
	}
}
