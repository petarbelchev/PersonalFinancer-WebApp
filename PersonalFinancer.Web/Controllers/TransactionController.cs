using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Data.Enums;
using PersonalFinancer.Services.Accounts;
using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Services.Categories;
using PersonalFinancer.Services.Categories.Models;
using PersonalFinancer.Services.Transactions;
using PersonalFinancer.Services.Transactions.Models;
using PersonalFinancer.Web.Infrastructure;
using static PersonalFinancer.Data.Constants.RoleConstants;

namespace PersonalFinancer.Web.Controllers
{
	/// <summary>
	/// Transaction Controller takes care of everything related to Transactions.
	/// </summary>
	[Authorize]
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

		/// <summary>
		/// Returns View with all user's transactions for specific period.
		/// </summary>
		[HttpGet]
		[Authorize(Roles = UserRoleName)]
		public async Task<IActionResult> All(string? startDate, string? endDate, int page = 1)
		{
			AllTransactionsServiceModel model = new AllTransactionsServiceModel()
			{
				Page = page
			};

			if (startDate == null || endDate == null)
			{
				model.StartDate = DateTime.UtcNow.AddMonths(-1);
				model.EndDate = DateTime.UtcNow;
			}
			else
			{
				model.StartDate = DateTime.Parse(startDate);
				model.EndDate = DateTime.Parse(endDate);
			}


			AllTransactionsServiceModel transactions =
				await transactionsService.AllTransactionsServiceModel(User.Id(), model);

			ViewBag.Controller = "Transaction";
			ViewBag.Action = "All";

			return View(transactions);
		}

		/// <summary>
		/// Returns View with all user's transactions for specific period.
		/// </summary>
		[HttpPost]
		[Authorize(Roles = UserRoleName)]
		public async Task<IActionResult> All(AllTransactionsServiceModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			try
			{
				AllTransactionsServiceModel transactions = await transactionsService
					.AllTransactionsServiceModel(User.Id(), model);

				ViewBag.Controller = "Transaction";
				ViewBag.Action = "All";

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
		[Authorize(Roles = UserRoleName)]
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
		[Authorize(Roles = UserRoleName)]
		public async Task<IActionResult> Create(TransactionFormModel transactionFormModel)
		{
			string userId = User.Id();

			if (!ModelState.IsValid)
			{
				transactionFormModel.Categories.AddRange(await categoryService.UserCategories(userId));
				transactionFormModel.Accounts.AddRange(await accountService.AllAccountsDropdownViewModel(userId));

				return View(transactionFormModel);
			}

			Guid newTransactionId = await transactionsService.CreateTransaction(transactionFormModel);

			TempData["successMsg"] = "You create a new transaction successfully!";

			return RedirectToAction("Details", "Transaction", new { id = newTransactionId });
		}

		/// <summary>
		/// Handle with deleting a Transaction.
		/// </summary>
		[HttpPost]
		[Authorize(Roles = UserRoleName)]
		public async Task<IActionResult> Delete(Guid id, string? returnUrl = null)
		{
			try
			{
				await transactionsService.DeleteTransactionById(id);
			}
			catch (ArgumentNullException)
			{
				return NotFound();
			}

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
		[Authorize(Roles = UserRoleName)]
		public async Task<IActionResult> Details(Guid id)
		{
			TransactionExtendedViewModel? transaction =
				await transactionsService.TransactionViewModel(id);

			if (transaction == null)
			{
				return BadRequest();
			}

			return View(transaction);
		}

		/// <summary>
		/// Return Edit Transaction Form Model for editing a Transaction.
		/// </summary>
		[HttpGet]
		public async Task<IActionResult> Edit(Guid id, string? returnUrl = null)
		{
			EditTransactionFormModel? transaction = await transactionsService.EditTransactionFormModelById(id);

			if (transaction == null)
			{
				return BadRequest();
			}

			if (User.Id() != transaction.AccountOwnerId && !User.IsAdmin())
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
				transaction.Categories.AddRange(await categoryService.UserCategories(transaction.AccountOwnerId));
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
				transaction.Accounts.AddRange(await accountService.AllAccountsDropdownViewModel(transaction.AccountOwnerId));
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
			if (!ModelState.IsValid)
			{
				transactionFormModel.Categories.AddRange(await categoryService.UserCategories(transactionFormModel.AccountOwnerId));
				transactionFormModel.Accounts.AddRange(await accountService.AllAccountsDropdownViewModel(transactionFormModel.AccountOwnerId));

				return View(transactionFormModel);
			}

			if (User.Id() != transactionFormModel.AccountOwnerId && !User.IsAdmin())
			{
				return Unauthorized();
			}

			await transactionsService.EditTransaction(transactionFormModel);

			if (User.IsAdmin())
			{
				TempData["successMsg"] = "You successfully edit User's transaction!";
			}
			else
			{
				TempData["successMsg"] = "Your transaction was successfully edited!";
			}

			if (transactionFormModel.ReturnUrl != null)
			{
				return LocalRedirect(transactionFormModel.ReturnUrl);
			}

			return RedirectToAction(nameof(Details), new { id = transactionFormModel.Id });
		}
	}
}
