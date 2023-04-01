using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.Accounts;
using PersonalFinancer.Services.Accounts.Models;
using static PersonalFinancer.Data.Constants.RoleConstants;

namespace PersonalFinancer.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(Roles = AdminRoleName)]
	public class TransactionsController : Controller
	{
		private readonly IAccountsService accountsService;

		public TransactionsController(IAccountsService accountsService)
			=> this.accountsService = accountsService;
		
		public async Task<IActionResult> TransactionDetails(string id)
		{
			try
			{
				return View(await accountsService.GetTransactionViewModel(id));
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
				TransactionFormModel viewModel = 
					await accountsService.GetFulfilledTransactionFormModel(id);

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
				await accountsService.PrepareTransactionFormModelForReturn(inputModel);

				return View(inputModel);
			}

			try
			{
				await accountsService.EditTransaction(id, inputModel);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}

			TempData["successMsg"] = "You successfully edit User's transaction!";

			if (returnUrl != null)
				return LocalRedirect(returnUrl);

			return RedirectToAction("AccountDetails", "Accounts", new { id = inputModel.AccountId });
		}
	}
}
