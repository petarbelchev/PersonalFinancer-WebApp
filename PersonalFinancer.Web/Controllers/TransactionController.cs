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

		[Authorize(Roles = UserRoleName)]
		public async Task<IActionResult> All(string? startDate, string? endDate, int page = 1)
		{
			var model = new UserTransactionsExtendedViewModel()
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

			try
			{
				await transactionsService.GetUserTransactionsExtendedViewModel(User.Id(), model);

				ViewBag.Controller = "Transaction";
				ViewBag.Action = "All";

				return View(model);
			}
			catch (ArgumentException ex)
			{
				ModelState.AddModelError(nameof(model.EndDate), ex.Message);

				return View(model);
			}
		}

		[HttpPost]
		[Authorize(Roles = UserRoleName)]
		public async Task<IActionResult> All(DateFilterModel dateModel)
		{
			if (!ModelState.IsValid)
			{
				return View(dateModel);
			}

			var viewModel = new UserTransactionsExtendedViewModel
			{
				StartDate = dateModel.StartDate,
				EndDate = dateModel.EndDate
			};

			try
			{
				await transactionsService.GetUserTransactionsExtendedViewModel(User.Id(), viewModel);

				ViewBag.Controller = "Transaction";
				ViewBag.Action = "All";

				return View(viewModel);
			}
			catch (ArgumentException ex)
			{
				ModelState.AddModelError(nameof(viewModel.EndDate), ex.Message);

				return View(viewModel);
			}
		}

		[Authorize(Roles = UserRoleName)]
		public async Task<IActionResult> Create()
		{
			string userId = User.Id();

			var model = new CreateTransactionFormModel()
			{
				CreatedOn = DateTime.UtcNow
			};

			model.Categories.AddRange(await categoryService.GetUserCategories(userId));
			model.Accounts.AddRange(await accountService.GetUserAccountsDropdownViewModel(userId));

			return View(model);
		}

		[HttpPost]
		[Authorize(Roles = UserRoleName)]
		public async Task<IActionResult> Create(CreateTransactionFormModel model)
		{
			string userId = User.Id();

			if (!ModelState.IsValid)
			{
				model.Categories.AddRange(await categoryService.GetUserCategories(userId));
				model.Accounts.AddRange(await accountService.GetUserAccountsDropdownViewModel(userId));

				return View(model);
			}

			Guid newTransactionId = await transactionsService.CreateTransaction(model);

			TempData["successMsg"] = "You create a new transaction successfully!";

			return RedirectToAction(nameof(Details), new { id = newTransactionId });
		}

		[HttpPost]
		[Authorize(Roles = UserRoleName)]
		public async Task<IActionResult> Delete(Guid id, string? returnUrl = null)
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
			{
				return LocalRedirect(returnUrl);
			}
			else
			{
				return RedirectToAction("Index", "Home");
			}
		}

		[Authorize(Roles = UserRoleName)]
		public async Task<IActionResult> Details(Guid id)
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

		public async Task<IActionResult> Edit(Guid id, string? returnUrl = null)
		{
			try
			{
				EditTransactionFormModel model = await transactionsService.GetEditTransactionFormModel(id);

				if (User.Id() != model.AccountOwnerId && !User.IsAdmin())
				{
					return Unauthorized();
				}

				bool isInitialBalance = await categoryService.IsInitialBalance(model.CategoryId);

				if (isInitialBalance)
				{
					model.Categories.Add(await categoryService.GetCategoryViewModel(model.CategoryId));
				}
				else
				{
					model.Categories.AddRange(await categoryService.GetUserCategories(model.AccountOwnerId));
					model.TransactionTypes.Add(TransactionType.Expense);
				}

				if (await accountService.IsAccountDeleted(model.AccountId) || isInitialBalance)
				{
					model.Accounts.Add(await accountService.GetAccountDropdownViewModel(model.AccountId));
				}
				else
				{
					model.Accounts.AddRange(await accountService.GetUserAccountsDropdownViewModel(model.AccountOwnerId));
				}

				model.TransactionTypes.Add(TransactionType.Income);
				model.ReturnUrl = returnUrl;

				return View(model);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
		}

		[HttpPost]
		public async Task<IActionResult> Edit(EditTransactionFormModel model)
		{
			if (!ModelState.IsValid)
			{
				model.Categories.AddRange(await categoryService.GetUserCategories(model.AccountOwnerId));
				model.Accounts.AddRange(await accountService.GetUserAccountsDropdownViewModel(model.AccountOwnerId));

				return View(model);
			}

			if (User.Id() != model.AccountOwnerId && !User.IsAdmin())
			{
				return Unauthorized();
			}

			try
			{
				await transactionsService.EditTransaction(model);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}

			if (User.IsAdmin())
			{
				TempData["successMsg"] = "You successfully edit User's transaction!";
			}
			else
			{
				TempData["successMsg"] = "Your transaction was successfully edited!";
			}

			if (model.ReturnUrl != null)
			{
				return LocalRedirect(model.ReturnUrl);
			}

			return RedirectToAction(nameof(Details), new { id = model.Id });
		}
	}
}
