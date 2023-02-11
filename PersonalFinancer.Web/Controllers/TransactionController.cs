namespace PersonalFinancer.Web.Controllers
{
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using NuGet.Packaging;

	using Services.Account;
	using Services.Account.Models;
	using Services.Category;
	using Infrastructure;

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

		//public async Task<IActionResult> All()
		//{
		//	return View();
		//}

		[HttpGet]
		public async Task<IActionResult> Create()
		{
			var formModel = new TransactionServiceModel();
			formModel.Categories.AddRange(await categoryService.All());
			formModel.Accounts.AddRange(await accountService.AllAccounts(User.Id()));
			formModel.CreatedOn = DateTime.Now;

			return View(formModel);
		}

		[HttpPost]
		public async Task<IActionResult> Create(TransactionServiceModel transactionFormModel)
		{
			if (!ModelState.IsValid)
			{
				transactionFormModel.Categories.AddRange(await categoryService.All());
				transactionFormModel.Accounts.AddRange(await accountService.AllAccounts(User.Id()));

				return View(transactionFormModel);
			}

			try
			{
				await accountService.CreateTransaction(transactionFormModel);
			}
			catch (Exception)
			{
				return BadRequest();
			}

			return RedirectToAction("Index", "Dashboard");
		}

		[HttpGet]
		public async Task<IActionResult> Delete(int id)
		{
			try
			{
				await accountService.DeleteTransactionById(id);
			}
			catch (Exception)
			{
				return BadRequest();
			}

			return RedirectToAction("Index", "Dashboard");
		}

		[HttpGet]
		public async Task<IActionResult> Edit(int id, string? returnUrl = null)
		{
			var transaction = await accountService.GetTransactionById(id);

			if (transaction == null)
				return BadRequest();

			string userId = User.Id();

			if (transaction.OwnerId != userId)
				return Unauthorized();

			if (transaction.CategoryId == 1)
				transaction.Categories.Add(await categoryService.CategoryById(transaction.CategoryId));
			else
				transaction.Categories.AddRange(await categoryService.All());

			transaction.Accounts.AddRange(await accountService.AllAccounts(userId));

			transaction.ReturnUrl = returnUrl;

			return View(transaction);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(TransactionServiceModel transactionFormModel)
		{
			string userId = User.Id();

			if (!ModelState.IsValid)
			{
				transactionFormModel.Categories.AddRange(await categoryService.All());
				transactionFormModel.Accounts.AddRange(await accountService.AllAccounts(userId));

				return View(transactionFormModel);
			}

			if (!await accountService.IsAccountOwner(userId, transactionFormModel.AccountId))
				return Unauthorized();

			try
			{
				await accountService.EditTransaction(transactionFormModel);
			}
			catch (Exception)
			{
				return BadRequest();
			}

			if (transactionFormModel.ReturnUrl != null)
				return LocalRedirect(transactionFormModel.ReturnUrl);

			return RedirectToAction("Index", "Dashboard");
		}
	}
}
