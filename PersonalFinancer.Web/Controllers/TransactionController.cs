using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Packaging;

using PersonalFinancer.Services.Account;
using PersonalFinancer.Services.Account.Models;
using PersonalFinancer.Services.Category;
using PersonalFinancer.Web.Infrastructure;

namespace PersonalFinancer.Web.Controllers
{
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
			var formModel = new TransactionFormModel();
			formModel.Categories.AddRange(await categoryService.All());
			formModel.Accounts.AddRange(await accountService.AllAccounts(User.Id()));
			formModel.CreatedOn = DateTime.Now;

			return View(formModel);
		}

		[HttpPost]
		public async Task<IActionResult> Create(TransactionFormModel transactionFormModel)
		{
			if (!ModelState.IsValid)
			{
				transactionFormModel.Categories.AddRange(await categoryService.All());
				transactionFormModel.Accounts.AddRange(await accountService.AllAccounts(User.Id()));

				return View(transactionFormModel);
			}

			try
			{
				await accountService.Add(transactionFormModel);
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
				await accountService.Delete(id);
			}
			catch (Exception)
			{
				return BadRequest();
			}

			return RedirectToAction("Index", "Dashboard");
		}

		[HttpGet]
		public async Task<IActionResult> Edit(int id)
		{
			var transaction = await accountService.FindTransactionById(id);

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

			return View(transaction);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(TransactionFormModel transactionFormModel)
		{
			string userId = User.Id();

			if (!ModelState.IsValid)
			{
				transactionFormModel.Categories.AddRange(await categoryService.All());
				transactionFormModel.Accounts.AddRange(await accountService.AllAccounts(userId));

				return View(transactionFormModel);
			}

			if (!await accountService.IsOwner(userId, transactionFormModel.AccountId))
				return Unauthorized();

			try
			{
				await accountService.Edit(transactionFormModel);
			}
			catch (Exception)
			{
				return BadRequest();
			}

			return RedirectToAction("Index", "Dashboard");
		}
	}
}
