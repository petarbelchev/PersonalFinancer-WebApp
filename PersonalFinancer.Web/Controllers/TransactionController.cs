namespace PersonalFinancer.Web.Controllers
{
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;

	using Infrastructure;
	using Services.Account;
	using Services.Account.Models;
	using Services.Category;
	using Services.Category.Models;

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

		[HttpGet]
		public async Task<IActionResult> All()
		{
			var model = new AllTransactionsServiceModel()
			{
				StartDate = DateTime.UtcNow.AddMonths(-1),
				EndDate = DateTime.UtcNow
			};

			AllTransactionsServiceModel transactions =
				await accountService.TransactionsViewModelByUserId(User.Id(), model);

			return View(transactions);
		}

		[HttpPost]
		public async Task<IActionResult> All(AllTransactionsServiceModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			AllTransactionsServiceModel transactions =
				await accountService.TransactionsViewModelByUserId(User.Id(), model);

			return View(transactions);
		}

		[HttpGet]
		public async Task<IActionResult> Create(string? returnUrl = null)
		{
			string userId = User.Id();

			var formModel = new TransactionFormModel()
			{
				Categories = await categoryService.UserCategories(userId),
				Accounts = await accountService.AccountsByUserId(userId),
				CreatedOn = DateTime.UtcNow,
				ReturnUrl = returnUrl
			};

			return View(formModel);
		}

		[HttpPost]
		public async Task<IActionResult> Create(TransactionFormModel transactionFormModel)
		{
			string userId = User.Id();

			if (!ModelState.IsValid)
			{
				transactionFormModel.Categories = await categoryService.UserCategories(userId);
				transactionFormModel.Accounts = await accountService.AccountsByUserId(userId);

				return View(transactionFormModel);
			}

			await accountService.CreateTransaction(transactionFormModel);

			TempData["successMsg"] = "You create a new transaction successfully!";

			if (transactionFormModel.ReturnUrl != null)
			{
				return LocalRedirect(transactionFormModel.ReturnUrl);
			}
			else
			{
				return RedirectToAction("Index", "Home");
			}
		}

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

		[HttpGet]
		public async Task<IActionResult> Details(Guid id)
		{
			TransactionExtendedViewModel transaction =
				await accountService.TransactionViewModelById(id);

			return View(transaction);
		}

		[HttpGet]
		public async Task<IActionResult> Edit(Guid id, string? returnUrl = null)
		{
			TransactionFormModel transaction = await accountService.TransactionFormModelById(id);

			string userId = User.Id();

			if (transaction.OwnerId != userId)
			{
				return Unauthorized();
			}

			if (await categoryService.IsInitialBalance(transaction.CategoryId))
			{
				transaction.Categories = new List<CategoryViewModel>()
				{
					await categoryService.CategoryById(transaction.CategoryId)
				};
			}
			else
			{
				transaction.Categories = await categoryService.UserCategories(userId);
			}

			transaction.Accounts = await accountService.AccountsByUserId(userId);
			transaction.ReturnUrl = returnUrl;

			return View(transaction);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(TransactionFormModel transactionFormModel)
		{
			string userId = User.Id();

			if (!ModelState.IsValid)
			{
				transactionFormModel.Categories = await categoryService.UserCategories(userId);
				transactionFormModel.Accounts = await accountService.AccountsByUserId(userId);

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

			return LocalRedirect("~/Transaction/Details/" + transactionFormModel.Id);
		}
	}
}
