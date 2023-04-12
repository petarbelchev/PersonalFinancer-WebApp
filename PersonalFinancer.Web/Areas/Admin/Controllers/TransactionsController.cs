using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.Accounts;
using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Services.User.Models;
using PersonalFinancer.Services.User;
using PersonalFinancer.Web.Infrastructure;
using static PersonalFinancer.Data.Constants.RoleConstants;
using PersonalFinancer.Web.Models.Transaction;

namespace PersonalFinancer.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(Roles = AdminRoleName)]
	public class TransactionsController : Controller
	{
		private readonly IAccountsService accountsService;
		private readonly IUsersService usersService;

		public TransactionsController(
			IAccountsService accountsService,
			IUsersService usersService)
		{
			this.accountsService = accountsService;
			this.usersService = usersService;
		}

		public async Task<IActionResult> TransactionDetails(string id)
		{
			try
			{
				return View(await accountsService.GetTransactionDetails(id));
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
				TransactionFormServiceModel transactionData = 
					await accountsService.GetTransactionFormData(id);

				var viewModel = new TransactionFormModel
				{
					OwnerId = transactionData.OwnerId,
					AccountId = transactionData.AccountId,
					Amount = transactionData.Amount,
					TransactionType = transactionData.TransactionType,
					Refference = transactionData.Refference,
					CreatedOn = transactionData.CreatedOn,
					CategoryId = transactionData.CategoryId,
					UserAccounts = transactionData.UserAccounts,
					UserCategories = transactionData.UserCategories
				};

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
				UserAccountsAndCategoriesServiceModel userData =
					await usersService.GetUserAccountsAndCategories(inputModel.OwnerId);
				
				inputModel.UserCategories = userData.UserCategories;
				inputModel.UserAccounts = userData.UserAccounts;

				return View(inputModel);
			}

			var serviceModel = new TransactionFormShortServiceModel
			{
				Amount = inputModel.Amount,
				CategoryId = inputModel.CategoryId,
				AccountId = inputModel.AccountId,
				CreatedOn = inputModel.CreatedOn,
				OwnerId = inputModel.OwnerId,
				Refference = inputModel.Refference,
				TransactionType = inputModel.TransactionType
			};

			try
			{
				await accountsService.EditTransaction(id, serviceModel);
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
