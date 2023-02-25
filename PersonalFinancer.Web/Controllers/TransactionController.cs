﻿namespace PersonalFinancer.Web.Controllers
{
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;

	using Infrastructure;
	using Data.Enums;
	using Services.Account;
	using Services.Account.Models;
	using Services.Category;
	using PersonalFinancer.Services.Category.Models;

	/// <summary>
	/// Transaction Controller takes care of everything related to Transactions.
	/// </summary>
	[Authorize]
	public class TransactionController : Controller
	{
		private readonly ICategoryService categoryService;
		private readonly IAccountService accountService;

		public TransactionController(
			ICategoryService categoryService,
			IAccountService accountService)
		{
			this.categoryService = categoryService;
			this.accountService = accountService;
		}

		/// <summary>
		/// Returns View with all user's transactions for specific period.
		/// </summary>
		[HttpGet]
		public async Task<IActionResult> All()
		{
			AllTransactionsServiceModel model = new AllTransactionsServiceModel()
			{
				StartDate = DateTime.UtcNow.AddMonths(-1),
				EndDate = DateTime.UtcNow
			};

			AllTransactionsServiceModel transactions =
				await accountService.AllTransactionsViewModel(User.Id(), model);

			return View(transactions);
		}

		/// <summary>
		/// Returns View with all user's transactions for specific period.
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> All(AllTransactionsServiceModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			try
			{
				AllTransactionsServiceModel transactions =
				await accountService.AllTransactionsViewModel(User.Id(), model);

				return View(transactions);
			}
			catch (ArgumentException ex)
			{
				ModelState.AddModelError(nameof(model.EndDate), ex.Message);
				return View(model);
			}
		}

		/// <summary>
		/// Returns View with Transaction Form Model for creating new Transaction.
		/// </summary>
		[HttpGet]
		public async Task<IActionResult> Create()
		{
			string userId = User.Id();

			TransactionFormModel formModel = new TransactionFormModel()
			{
				CreatedOn = DateTime.UtcNow
			};

			formModel.Categories.AddRange(await categoryService.UserCategories(userId));
			formModel.Accounts.AddRange(await accountService.AllAccountsDropdownViewModel(userId));

			return View(formModel);
		}

		/// <summary>
		/// Handle with Transaction Form Model and creates new Transaction. 
		/// If success redirect to the new Transaction Details page.
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> Create(TransactionFormModel transactionFormModel)
		{
			string userId = User.Id();

			if (!ModelState.IsValid)
			{
				transactionFormModel.Categories.AddRange(await categoryService.UserCategories(userId));
				transactionFormModel.Accounts.AddRange(await accountService.AllAccountsDropdownViewModel(userId));

				return View(transactionFormModel);
			}

			Guid newTransactionId = await accountService.CreateTransaction(transactionFormModel);

			TempData["successMsg"] = "You create a new transaction successfully!";

			return RedirectToAction("Details", "Transaction", new { id = newTransactionId });
		}

		/// <summary>
		/// Handle with deleting Transaction.
		/// </summary>
		[HttpGet]
		public async Task<IActionResult> Delete(Guid id, string? returnUrl = null)
		{
			await accountService.DeleteTransactionById(id);

			TempData["successMsg"] = "Your transaction was successfully deleted!";

			if (returnUrl != null)
			{
				return LocalRedirect(returnUrl);
			}
			else
			{
				return RedirectToAction("Index", "Home");
			}
		}

		/// <summary>
		/// Returns Transaction View Model for Transaction Details page. 
		/// </summary>
		[HttpGet]
		public async Task<IActionResult> Details(Guid id)
		{
			TransactionExtendedViewModel? transaction =
				await accountService.TransactionViewModel(id);

			if (transaction == null)
			{
				return BadRequest();
			}

			return View(transaction);
		}

		/// <summary>
		/// Return Edit Transaction Form Model for edit a Transaction.
		/// </summary>
		[HttpGet]
		public async Task<IActionResult> Edit(Guid id, string? returnUrl = null)
		{
			EditTransactionFormModel? transaction = await accountService.EditTransactionFormModelById(id);

			if (transaction == null)
			{
				return BadRequest();
			}

			string userId = User.Id();

			if (!await accountService.IsAccountOwner(userId, transaction.AccountId))
			{
				return Unauthorized();
			}

			bool isInitialBalance = await categoryService.IsInitialBalance(transaction.CategoryId);

			if (isInitialBalance)
			{
				CategoryViewModel? category = await categoryService
					.CategoryById(transaction.CategoryId);

				if (category != null)
				{
					transaction.Categories.Add(category);
				}
			}
			else
			{
				transaction.Categories.AddRange(await categoryService.UserCategories(userId));
				transaction.TransactionTypes.Add(TransactionType.Expense);
			}

			if (await accountService.IsAccountDeleted(transaction.AccountId) || isInitialBalance)
			{
				AccountDropdownViewModel? account = await accountService
					.AccountDropdownViewModel(transaction.AccountId);

				if (account != null)
				{
					transaction.Accounts.Add(account);
				}
			}
			else
			{
				transaction.Accounts.AddRange(await accountService.AllAccountsDropdownViewModel(userId));
			}

			transaction.TransactionTypes.Add(TransactionType.Income);
			transaction.ReturnUrl = returnUrl;

			return View(transaction);
		}

		/// <summary>
		/// Handle with edited Transaction.
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> Edit(EditTransactionFormModel transactionFormModel)
		{
			string userId = User.Id();

			if (!ModelState.IsValid)
			{
				transactionFormModel.Categories.AddRange(await categoryService.UserCategories(userId));
				transactionFormModel.Accounts.AddRange(await accountService.AllAccountsDropdownViewModel(userId));

				return View(transactionFormModel);
			}

			if (!await accountService.IsAccountOwner(userId, transactionFormModel.AccountId))
			{
				return Unauthorized();
			}

			await accountService.EditTransaction(transactionFormModel);

			TempData["successMsg"] = "Your transaction was successfully edited!";

			if (transactionFormModel.ReturnUrl != null)
			{
				return LocalRedirect(transactionFormModel.ReturnUrl);
			}

			return RedirectToAction(nameof(Details), new { id = transactionFormModel.Id });
		}
	}
}
