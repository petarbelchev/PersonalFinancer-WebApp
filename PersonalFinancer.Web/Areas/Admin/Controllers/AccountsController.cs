namespace PersonalFinancer.Web.Areas.Admin.Controllers
{
	using AutoMapper;

	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;

	using Services.Accounts;
	using Services.Accounts.Models;

	using Web.Infrastructure;
	using Web.Models.Accounts;

	using static Data.Constants.RoleConstants;

	[Area("Admin")]
	[Authorize(Roles = AdminRoleName)]
	public class AccountsController : Controller
	{
		private readonly IAccountsService accountsService;
		private readonly IMapper mapper;
		private readonly IControllerService controllerService;

		public AccountsController(
			IAccountsService accountsService,
			IMapper mapper,
			IControllerService controllerService)
		{
			this.accountsService = accountsService;
			this.mapper = mapper;
			this.controllerService = controllerService;
		}

		public async Task<IActionResult> Index(int page = 1)
		{
			var viewModel = new UsersAccountCardsViewModel();
			AccountCardsOutputDTO accountCardsData = await accountsService
				.GetUsersAccountCards(page, viewModel.Pagination.ElementsPerPage);

			viewModel.Accounts = accountCardsData.Accounts
				.Select(ac => mapper.Map<AccountCardExtendedViewModel>(ac)).ToArray();
			viewModel.Pagination.Page = accountCardsData.Page;
			viewModel.Pagination.TotalElements = accountCardsData.AllAccountsCount;

			return View(viewModel);
		}

		public async Task<IActionResult> AccountDetails(string id)
		{
			try
			{
				var inputDTO = new AccountDetailsInputDTO
				{
					Id = id,
					StartDate = DateTime.UtcNow.AddMonths(-1),
					EndDate = DateTime.UtcNow
				};

				AccountDetailsOutputDTO accountData =
					await accountsService.GetAccountDetails(inputDTO);

				var viewModel = mapper.Map<AccountDetailsViewModel>(accountData);
				viewModel.Pagination.TotalElements = accountData.AllTransactionsCount;
				viewModel.Routing.Area = "Admin";
				viewModel.Routing.ReturnUrl = "/Admin/Accounts/AccountDetails/" + id;

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
					accountData = await accountsService.GetAccountDetailsForReturn(inputDTO);
					viewModel = mapper.Map<AccountDetailsViewModel>(accountData);
				}
				else
				{
					accountData = await accountsService.GetAccountDetails(inputDTO);
					viewModel = mapper.Map<AccountDetailsViewModel>(accountData);
					viewModel.Pagination.TotalElements = accountData.AllTransactionsCount;
					viewModel.Routing.Area = "Admin";
					viewModel.Routing.ReturnUrl = "/Admin/Accounts/AccountDetails/" + inputModel.Id;
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
					await accountsService.GetDeleteAccountData(id);

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
				string ownerId = await accountsService.GetOwnerId(inputModel.Id);

				await accountsService.DeleteAccount(
					inputModel.Id, inputModel.ShouldDeleteTransactions ?? false);

				TempData["successMsg"] = "You successfully delete user's account!";

				return LocalRedirect("/Admin/Users/Details/" + ownerId);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
		}

		public async Task<IActionResult> EditAccount(string id)
		{
			try
			{
				EditAccountFormDTO accountData = await accountsService.GetAccountForm(id);

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
			EditAccountFormDTO accountData;

			try
			{
				if (!ModelState.IsValid)
				{
					await controllerService.PrepareAccountFormModelForReturn(inputModel);

					return View(inputModel);
				}

				accountData = mapper.Map<EditAccountFormDTO>(inputModel);
				await accountsService.EditAccount(accountData);

				TempData["successMsg"] = "You successfully edited user's account!";

				return LocalRedirect(inputModel.ReturnUrl);
			}
			catch (ArgumentException)
			{
				ModelState.AddModelError(nameof(inputModel.Name),
					$"The User already have Account with {inputModel.Name} name.");

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
