namespace PersonalFinancer.Web.Areas.Admin.Controllers
{
	using AutoMapper;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;

	using Services.Accounts;
	using Services.Accounts.Models;
	using Services.Shared.Models;

	using Web.Infrastructure;
	using Web.Models.Accounts;
	using Web.Models.Shared;

	using static Data.Constants.RoleConstants;

	[Area("Admin")]
	[Authorize(Roles = AdminRoleName)]
	public class TransactionsController : Controller
	{
		private readonly IAccountsService accountsService;
		private readonly IMapper mapper;
		private readonly IControllerService controllerService;

		public TransactionsController(
			IAccountsService accountsService,
			IMapper mapper,
			IControllerService controllerService)
		{
			this.accountsService = accountsService;
			this.mapper = mapper;
			this.controllerService = controllerService;
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
				await controllerService.PrepareTransactionFormModelForReturn(inputModel);
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
	}
}
