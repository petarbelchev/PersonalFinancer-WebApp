namespace PersonalFinancer.Web.Controllers
{
    using AutoMapper;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using Services.Accounts;
    using Services.Accounts.Models;

    using Web.Infrastructure;
    using Web.Models.Accounts;

    using static Data.Constants.RoleConstants;

    [Authorize(Roles = UserRoleName)]
	public class AccountsController : Controller
	{
		private readonly IAccountsService accountService;
		private readonly IMapper mapper;
		private readonly IControllerService controllerService;

		public AccountsController(
			IAccountsService accountService,
			IMapper mapper,
			IControllerService controllerService)
		{
			this.accountService = accountService;
			this.mapper = mapper;
			this.controllerService = controllerService;
		}

		public async Task<IActionResult> Create()
		{
			CreateAccountFormDTO accountData =
				await accountService.GetEmptyAccountForm(User.Id());

			var viewModel = mapper.Map<CreateAccountFormModel>(accountData);

			return View(viewModel);
		}

		[HttpPost]
		public async Task<IActionResult> Create(CreateAccountFormModel inputModel)
		{
			CreateAccountFormDTO accountData;
			CreateAccountFormModel viewModel;

			if (!ModelState.IsValid)
			{
				accountData = await accountService.GetEmptyAccountForm(User.Id());
				viewModel = mapper.Map<CreateAccountFormModel>(accountData);

				return View(viewModel);
			}

			try
			{
				accountData = mapper.Map<CreateAccountFormDTO>(inputModel);
				string newAccountId = await accountService.CreateAccount(accountData);

				TempData["successMsg"] = "You create a new account successfully!";

				return RedirectToAction(nameof(AccountDetails), new { id = newAccountId });
			}
			catch (ArgumentException)
			{
				ModelState.AddModelError(nameof(inputModel.Name),
					"You already have Account with that name.");

				accountData = await accountService.GetEmptyAccountForm(User.Id());
				viewModel = mapper.Map<CreateAccountFormModel>(accountData);

				return View(viewModel);
			}
		}

		public async Task<IActionResult> AccountDetails(string id)
		{
			try
			{
				AccountDetailsInputDTO inputDTO = new AccountDetailsInputDTO
				{
					Id = id,
					StartDate = DateTime.UtcNow.AddMonths(-1),
					EndDate = DateTime.UtcNow
				};

				AccountDetailsOutputDTO accountData =
					await accountService.GetAccountDetails(inputDTO, User.Id());
				var viewModel = mapper.Map<AccountDetailsViewModel>(accountData);
				viewModel.Pagination.TotalElements = accountData.AllTransactionsCount;
				viewModel.Routing.ReturnUrl = "/Accounts/AccountDetails/" + id;

				return View(viewModel);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
		}

		[HttpPost]
		public async Task<IActionResult> AccountDetails(AccountDetailsInputModel inputModel)
		{
			AccountDetailsOutputDTO accountData;
			AccountDetailsViewModel viewModel;
			var inputDTO = new AccountDetailsInputDTO
			{
				Id = inputModel.Id,
				StartDate = inputModel.StartDate,
				EndDate = inputModel.EndDate
			};

			try
			{
				if (!ModelState.IsValid)
				{
					accountData = await accountService.GetAccountDetailsForReturn(inputDTO);
					viewModel = mapper.Map<AccountDetailsViewModel>(accountData);
				}
				else
				{
					accountData = await accountService.GetAccountDetails(inputDTO, User.Id());
					viewModel = mapper.Map<AccountDetailsViewModel>(accountData);
					viewModel.Pagination.TotalElements = accountData.AllTransactionsCount;
					viewModel.Routing.ReturnUrl = "/Accounts/AccountDetails/" + inputModel.Id;
				}

				return View(viewModel);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
		}

		public async Task<IActionResult> Delete(string id)
		{
			try
			{
				DeleteAccountDTO accountData =
					await accountService.GetDeleteAccountData(id, User.Id());

				var viewModel = mapper.Map<DeleteAccountViewModel>(accountData);

				return View(viewModel);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
		}

		[HttpPost]
		public async Task<IActionResult> Delete(DeleteAccountInputModel inputModel, string returnUrl)
		{
			if (!ModelState.IsValid)
				return BadRequest();

			if (inputModel.ConfirmButton == "reject")
				return LocalRedirect(returnUrl);

			try
			{
				await accountService.DeleteAccount(
					inputModel.Id,
					inputModel.ShouldDeleteTransactions ?? false,
					User.Id());
			}
			catch (ArgumentException)
			{
				return Unauthorized();
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}

			TempData["successMsg"] = "Your account was successfully deleted!";

			return RedirectToAction("Index", "Home");
		}

		public async Task<IActionResult> EditAccount(string id)
		{
			try
			{
				EditAccountFormDTO accountData =
					await accountService.GetAccountForm(id, User.Id());

				var viewModel = mapper.Map<CreateAccountFormModel>(accountData);

				return View(viewModel);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
		}

		[HttpPost]
		public async Task<IActionResult> EditAccount(EditAccountFormModel inputModel)
		{
			if (inputModel.OwnerId != User.Id())
				return Unauthorized();

			try
			{
				if (!ModelState.IsValid)
				{
					await controllerService.PrepareAccountFormModelForReturn(inputModel);

					return View(inputModel);
				}

				EditAccountFormDTO accountData = mapper.Map<EditAccountFormDTO>(inputModel);
				await accountService.EditAccount(accountData);

				TempData["successMsg"] = "Your account was successfully edited!";

				return LocalRedirect(inputModel.ReturnUrl);
			}
			catch (ArgumentException)
			{
				ModelState.AddModelError(nameof(inputModel.Name),
					$"You already have Account with {inputModel.Name} name.");

				await controllerService.PrepareAccountFormModelForReturn(inputModel);

				return View(inputModel);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
		}
	}
}