using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
		//private readonly ITransactionService transactionService;
		private readonly IAccountService accountService;

		public TransactionController(
			ICategoryService categoryService,
			//ITransactionService transactionService,
			IAccountService accountService)
		{
			//this.transactionService = transactionService;
			this.categoryService = categoryService;
			this.accountService = accountService;
		}

		public async Task<IActionResult> All()
		{
			return View();
		}

		[HttpGet]
		public async Task<IActionResult> Create()
		{
			return View(new CreateTransactionFormModel()
			{
				Categories = await categoryService.All(),
				Accounts = await accountService.AllAccounts(User.Id())
			});
		}

		[HttpPost]
		public async Task<IActionResult> Create(CreateTransactionFormModel transactionFormModel)
		{
			if (!ModelState.IsValid)
			{
				transactionFormModel.Categories = await categoryService.All();
				transactionFormModel.Accounts = await accountService.AllAccounts(User.Id());

				return View(transactionFormModel);
			}

			try
			{
				await accountService.AddTransaction(transactionFormModel);
			}
			catch (Exception)
			{
				return BadRequest();
			}

			return RedirectToAction("Index", "Dashboard");
		}
	}
}
