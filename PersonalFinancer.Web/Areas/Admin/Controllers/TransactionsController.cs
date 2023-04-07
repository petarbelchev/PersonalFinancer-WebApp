using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.Accounts;
using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Services.Shared.Models;
using PersonalFinancer.Web.Infrastructure;
using PersonalFinancer.Web.Models.Accounts;
using PersonalFinancer.Web.Models.Shared;
using static PersonalFinancer.Data.Constants.RoleConstants;

namespace PersonalFinancer.Web.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = AdminRoleName)]
	public class TransactionsController : Controller
	{
		private readonly IAccountsService accountsService;
		private readonly IMapper mapper;

		public TransactionsController(
			IAccountsService accountsService,
			IMapper mapper)
		{
			this.accountsService = accountsService;
			this.mapper = mapper;
		}

		public async Task<IActionResult> TransactionDetails(string id)
		{
			try
			{
				TransactionDetailsDTO transactionData =
					await accountsService.GetTransactionDetails(id);
				var viewModel = mapper.Map<TransactionDetailsViewModel>(transactionData);

				return View(viewModel);
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
				FulfilledTransactionFormDTO formDTO =
				   await accountsService.GetFulfilledTransactionForm(id);

				var viewModel = mapper.Map<TransactionFormModel>(formDTO);

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
				await PrepareTransactionFormModelForReturn(inputModel);
				return View(inputModel);
			}

			try
			{
				var inputDTO = mapper.Map<EditTransactionInputDTO>(inputModel);
				await accountsService.EditTransaction(inputDTO);
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

		// NOTE: Think to simplify
		private async Task PrepareTransactionFormModelForReturn(TransactionFormModel inputModel)
		{
			EmptyTransactionFormDTO emptyFormModel =
				await accountsService.GetEmptyTransactionForm(User.Id());
			inputModel.UserCategories = emptyFormModel.UserCategories
				.Select(c => mapper.Map<CategoryViewModel>(c));
			inputModel.UserAccounts = emptyFormModel.UserAccounts
				.Select(a => mapper.Map<AccountDropdownViewModel>(a));
		}
	}
}
