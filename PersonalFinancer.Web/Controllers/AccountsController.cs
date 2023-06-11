namespace PersonalFinancer.Web.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using PersonalFinancer.Services.Accounts;
    using PersonalFinancer.Services.Accounts.Models;
    using PersonalFinancer.Services.User;
    using PersonalFinancer.Services.User.Models;
    using PersonalFinancer.Web.Infrastructure.Extensions;
    using PersonalFinancer.Web.Models.Account;
    using System.ComponentModel.DataAnnotations;
    using static PersonalFinancer.Data.Constants.RoleConstants;

    [Authorize]
    public class AccountsController : Controller
    {
        protected readonly IAccountsService accountService;
        protected readonly IUsersService usersService;
        protected readonly IMapper mapper;

        public AccountsController(
            IAccountsService accountService,
            IMapper mapper,
            IUsersService usersService)
        {
            this.accountService = accountService;
            this.mapper = mapper;
            this.usersService = usersService;
        }

        [Authorize(Roles = UserRoleName)]
        public async Task<IActionResult> Create()
        {
            var viewModel = new AccountFormViewModel { OwnerId = this.User.IdToGuid() };
            await this.PrepareAccountFormViewModel(viewModel);

            return this.View(viewModel);
        }

        [Authorize(Roles = UserRoleName)]
        [HttpPost]
        public async Task<IActionResult> Create(AccountFormViewModel inputModel)
        {
            if (!this.ModelState.IsValid)
            {
                await this.PrepareAccountFormViewModel(inputModel);

                return this.View(inputModel);
            }

            if (inputModel.OwnerId != this.User.IdToGuid())
                return this.BadRequest();

            try
            {
                AccountFormShortServiceModel accountServiceModel = 
                    this.mapper.Map<AccountFormShortServiceModel>(inputModel);

                Guid newAccountId = await this.accountService.CreateAccount(accountServiceModel);
                this.TempData["successMsg"] = "You create a new account successfully!";

                return this.RedirectToAction(nameof(AccountDetails), new { id = newAccountId });
            }
            catch (ArgumentException)
            {
                this.ModelState.AddModelError(nameof(inputModel.Name),
                    "You already have Account with that name.");
                await this.PrepareAccountFormViewModel(inputModel);

                return this.View(inputModel);
            }
        }

        public async Task<IActionResult> AccountDetails(Guid id)
        {
            var inputModel = new AccountDetailsInputModel
            {
                Id = id,
                StartDate = DateTime.Now.AddMonths(-1),
                EndDate = DateTime.Now
            };

            try
            {
                AccountDetailsViewModel viewModel =
                    await this.GetAccountDetailsViewModel(inputModel, this.User.IdToGuid(), this.User.IsAdmin());

                return this.View(viewModel);
            }
            catch (InvalidOperationException)
            {
                return this.BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> AccountDetails(AccountDetailsInputModel inputModel)
        {
            if (!this.ModelState.IsValid)
            {
                try
                {
                    AccountDetailsShortServiceModel accShortDetails =
                        await this.accountService.GetAccountShortDetails(inputModel.Id);

                    AccountDetailsViewModel viewModel = this.mapper.Map<AccountDetailsViewModel>(accShortDetails);

                    return this.View(viewModel);
                }
                catch (Exception)
                {
                    return this.BadRequest();
                }
            }

            try
            {
                AccountDetailsViewModel viewModel =
                    await this.GetAccountDetailsViewModel(inputModel, this.User.IdToGuid(), this.User.IsAdmin());

                return this.View(viewModel);
            }
            catch (InvalidOperationException)
            {
                return this.BadRequest();
            }
        }

        public async Task<IActionResult> Delete([Required] Guid? id)
        {
            if (!this.ModelState.IsValid)
                return this.BadRequest();

            try
            {
                string accountName = await this.accountService
                    .GetAccountName(id, this.User.IdToGuid(), this.User.IsAdmin());

                var viewModel = new DeleteAccountViewModel { Name = accountName };

                return this.View(viewModel);
            }
            catch (InvalidOperationException)
            {
                return this.BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(DeleteAccountInputModel inputModel)
        {
            if (!this.ModelState.IsValid)
                return this.BadRequest();

            if (inputModel.ConfirmButton == "reject")
                return this.LocalRedirect(inputModel.ReturnUrl);

            try
            {
                await this.accountService.DeleteAccount(
                    inputModel.Id, this.User.IdToGuid(), this.User.IsAdmin(),
                    inputModel.ShouldDeleteTransactions ?? false);

                if (this.User.IsAdmin())
                {
                    this.TempData["successMsg"] = "You successfully delete user's account!";
                    Guid ownerId = await this.accountService.GetOwnerId(inputModel.Id);

                    return this.LocalRedirect("/Admin/Users/Details/" + ownerId);
                }
                else
                {
                    this.TempData["successMsg"] = "Your account was successfully deleted!";

                    return this.RedirectToAction("Index", "Home");
                }
            }
            catch (ArgumentException)
            {
                return this.Unauthorized();
            }
            catch (InvalidOperationException)
            {
                return this.BadRequest();
            }
        }

        public async Task<IActionResult> EditAccount([Required] Guid? id)
        {
            if (!this.ModelState.IsValid)
                return this.BadRequest();

            try
            {
                AccountFormServiceModel accountData = await this.accountService
                    .GetAccountFormData(id, this.User.IdToGuid(), this.User.IsAdmin());

                AccountFormViewModel viewModel = this.mapper.Map<AccountFormViewModel>(accountData);

                return this.View(viewModel);
            }
            catch (InvalidOperationException)
            {
                return this.BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditAccount(
            [Required] Guid? id, AccountFormViewModel inputModel, string returnUrl)
        {
            if (!this.ModelState.IsValid)
            {
                await this.PrepareAccountFormViewModel(inputModel);

                return this.View(inputModel);
            }

            Guid ownerId = this.User.IsAdmin()
                ? await this.accountService.GetOwnerId(id)
                : this.User.IdToGuid();

            if (inputModel.OwnerId != ownerId)
                return this.BadRequest();

            try
            {
                AccountFormShortServiceModel serviceModel = 
                    this.mapper.Map<AccountFormShortServiceModel>(inputModel);
                
                await this.accountService.EditAccount(id, serviceModel);

                this.TempData["successMsg"] = this.User.IsAdmin() ?
                    "You successfully edited user's account!"
                    : "Your account was successfully edited!";

                return this.LocalRedirect(returnUrl);
            }
            catch (ArgumentException)
            {
                this.ModelState.AddModelError(nameof(inputModel.Name), this.User.IsAdmin() ?
                    $"The user already have Account with \"{inputModel.Name}\" name."
                    : $"You already have Account with \"{inputModel.Name}\" name.");

                await this.PrepareAccountFormViewModel(inputModel);

                return this.View(inputModel);
            }
            catch (InvalidOperationException)
            {
                return this.BadRequest();
            }
        }

        private async Task PrepareAccountFormViewModel(AccountFormViewModel viewModel)
        {
            UserAccountTypesAndCurrenciesServiceModel userData =
                await this.usersService.GetUserAccountTypesAndCurrencies(viewModel.OwnerId);

            viewModel.AccountTypes = userData.AccountTypes;
            viewModel.Currencies = userData.Currencies;
        }

        /// <summary>
        /// Throws InvalidOperationException when Account does not exist
        /// or User is not owner or Administrator.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        private async Task<AccountDetailsViewModel> GetAccountDetailsViewModel(
            AccountDetailsInputModel model, Guid userId, bool isUserAdmin)
        {
            AccountDetailsServiceModel accountDetails = await this.accountService
                .GetAccountDetails(model.Id, model.StartDate, model.EndDate, userId, isUserAdmin);

            AccountDetailsViewModel viewModel = this.mapper.Map<AccountDetailsViewModel>(accountDetails);
            viewModel.Pagination.TotalElements = accountDetails.TotalAccountTransactions;

            if (isUserAdmin)
            {
                viewModel.Routing.Area = "Admin";
                viewModel.Routing.ReturnUrl = "/Admin/Accounts/AccountDetails/" + model.Id;
            }
            else
            {
                viewModel.Routing.ReturnUrl = "/Accounts/AccountDetails/" + model.Id;
            }

            return viewModel;
        }
    }
}