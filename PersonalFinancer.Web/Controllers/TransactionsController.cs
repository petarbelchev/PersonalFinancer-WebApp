﻿namespace PersonalFinancer.Web.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using PersonalFinancer.Services.Accounts;
    using PersonalFinancer.Services.Accounts.Models;
    using PersonalFinancer.Services.Shared.Models;
    using PersonalFinancer.Services.User;
    using PersonalFinancer.Services.User.Models;
    using PersonalFinancer.Web.Infrastructure.Extensions;
    using PersonalFinancer.Web.Models.Shared;
    using PersonalFinancer.Web.Models.Transaction;
    using static PersonalFinancer.Data.Constants.RoleConstants;

    [Authorize]
    public class TransactionsController : Controller
    {
        protected readonly IAccountsService accountsService;
        protected readonly IUsersService usersService;
        protected readonly IMapper mapper;

        public TransactionsController(
            IAccountsService accountsService,
            IUsersService usersService,
            IMapper mapper)
        {
            this.accountsService = accountsService;
            this.usersService = usersService;
            this.mapper = mapper;
        }

        [Authorize(Roles = UserRoleName)]
        public async Task<IActionResult> All()
        {
            DateTime startDate = DateTime.Now.AddMonths(-1);
            DateTime endDate = DateTime.Now;

            UserTransactionsViewModel viewModel =
                await this.PrepareUserTransactionsViewModel(this.User.Id(), startDate, endDate);

            return this.View(viewModel);
        }

        [Authorize(Roles = UserRoleName)]
        [HttpPost]
        public async Task<IActionResult> All(DateFilterModel inputModel)
        {
            if (!this.ModelState.IsValid)
                return this.View(this.mapper.Map<UserTransactionsViewModel>(inputModel));

            UserTransactionsViewModel viewModel = await this.PrepareUserTransactionsViewModel(
                this.User.Id(), inputModel.StartDate, inputModel.EndDate);

            return this.View(viewModel);
        }

        [Authorize(Roles = UserRoleName)]
        public async Task<IActionResult> Create()
        {
            UserAccountsAndCategoriesServiceModel userData =
                await this.usersService.GetUserAccountsAndCategories(this.User.Id());

            TransactionFormModel viewModel = this.mapper.Map<TransactionFormModel>(userData);
            viewModel.CreatedOn = DateTime.Now;

            return this.View(viewModel);
        }

        [Authorize(Roles = UserRoleName)]
        [HttpPost]
        public async Task<IActionResult> Create(TransactionFormModel inputModel)
        {
            if (!this.ModelState.IsValid)
            {
                await this.PrepareTransactionFormModelForReturn(inputModel);

                return this.View(inputModel);
            }

            if (inputModel.OwnerId != this.User.Id())
                return this.BadRequest();

            try
            {
                TransactionFormShortServiceModel serviceModel = 
                    this.mapper.Map<TransactionFormShortServiceModel>(inputModel);
                
                Guid newTransactionId = await this.accountsService.CreateTransaction(serviceModel);
                this.TempData["successMsg"] = "You create a new transaction successfully!";

                return this.RedirectToAction(nameof(TransactionDetails), new { id = newTransactionId });
            }
            catch (InvalidOperationException)
            {
                return this.BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id, string? returnUrl = null)
        {
            try
            {
                _ = await this.accountsService.DeleteTransaction(id, this.User.Id(), this.User.IsAdmin());

                this.TempData["successMsg"] = this.User.IsAdmin() ?
                    "You successfully delete a user's transaction!"
                    : "Your transaction was successfully deleted!";
            }
            catch (ArgumentException)
            {
                return this.Unauthorized();
            }
            catch (InvalidOperationException)
            {
                return this.BadRequest();
            }

            return returnUrl != null 
                ? this.LocalRedirect(returnUrl) 
                : this.RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> TransactionDetails(Guid id)
        {
            try
            {
                return this.View(await this.accountsService
                    .GetTransactionDetails(id, this.User.Id(), this.User.IsAdmin()));
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

        public async Task<IActionResult> EditTransaction(Guid id)
        {
            try
            {
                TransactionFormServiceModel transactionData =
                    await this.accountsService.GetTransactionFormData(id);

                if (!this.User.IsAdmin() && this.User.Id() != transactionData.OwnerId)
                    return this.Unauthorized();

                TransactionFormModel viewModel = this.mapper.Map<TransactionFormModel>(transactionData);

                return this.View(viewModel);
            }
            catch (InvalidOperationException)
            {
                return this.BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditTransaction(Guid id, TransactionFormModel inputModel)
        {
            if (!this.ModelState.IsValid)
            {
                await this.PrepareTransactionFormModelForReturn(inputModel);

                return this.View(inputModel);
            }

            Guid ownerId = this.User.IsAdmin()
                ? await this.accountsService.GetOwnerId(inputModel.AccountId)
                : this.User.Id();

            if (inputModel.OwnerId != ownerId)
                return this.Unauthorized();

            TransactionFormShortServiceModel serviceModel = this.mapper.Map<TransactionFormShortServiceModel>(inputModel);

            try
            {
                await this.accountsService.EditTransaction(id, serviceModel);
            }
            catch (InvalidOperationException)
            {
                return this.BadRequest();
            }

            if (this.User.IsAdmin())
            {
                this.TempData["successMsg"] = "You successfully edit User's transaction!";
                return this.RedirectToAction("AccountDetails", "Accounts", new { id = inputModel.AccountId });
            }
            else
            {
                this.TempData["successMsg"] = "Your transaction was successfully edited!";
                return this.RedirectToAction(nameof(TransactionDetails), new { id });
            }
        }

        private async Task PrepareTransactionFormModelForReturn(TransactionFormModel formModel)
        {
            UserAccountsAndCategoriesServiceModel userData =
                await this.usersService.GetUserAccountsAndCategories(formModel.OwnerId);

            formModel.UserCategories = userData.UserCategories;
            formModel.UserAccounts = userData.UserAccounts;
        }

        private async Task<UserTransactionsViewModel> PrepareUserTransactionsViewModel(
            Guid userId, DateTime startDate, DateTime endDate)
        {
            TransactionsServiceModel userTransactions =
                await this.usersService.GetUserTransactions(userId, startDate, endDate);

            UserTransactionsViewModel viewModel = this.mapper.Map<UserTransactionsViewModel>(userTransactions);
            viewModel.Id = userId;
            viewModel.Pagination.TotalElements = userTransactions.TotalTransactionsCount;

            return viewModel;
        }
    }
}
