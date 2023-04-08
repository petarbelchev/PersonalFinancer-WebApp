namespace PersonalFinancer.Web.Infrastructure
{
	using AutoMapper;

	using Microsoft.AspNetCore.Mvc.ModelBinding;
    
    using System.Text;

	using Services.Accounts;
	using Services.Accounts.Models;
	using Services.Shared.Models;
		
    using Web.Models.Accounts;
	using Web.Models.Shared;

	public class ControllerService : IControllerService
    {
        private readonly IAccountsService accountsService;
        private readonly IMapper mapper;

        public ControllerService(
            IAccountsService accountsService,
            IMapper mapper)
        {
            this.accountsService = accountsService;
            this.mapper = mapper;
        }

        public string GetModelErrors(ModelStateDictionary.ValueEnumerable modelStateValues)
        {
            var errors = new StringBuilder();

            foreach (var modelStateVal in modelStateValues)
            {
                foreach (var error in modelStateVal.Errors)
                {
                    errors.AppendLine(error.ErrorMessage);
                }
            }

            return errors.ToString().TrimEnd();
        }

        public async Task PrepareAccountFormModelForReturn<T>(T inputModel)
            where T : IAccountFormModel
        {
            CreateAccountFormDTO accountData =
                await accountsService.GetEmptyAccountForm(inputModel.OwnerId);
            inputModel.AccountTypes = accountData.AccountTypes
                .Select(at => mapper.Map<AccountTypeViewModel>(at));
            inputModel.Currencies = accountData.Currencies
                .Select(c => mapper.Map<CurrencyViewModel>(c));
        }

		public async Task PrepareTransactionFormModelForReturn(TransactionFormModel inputModel)
		{
			EmptyTransactionFormDTO emptyFormModel =
				await accountsService.GetEmptyTransactionForm(inputModel.OwnerId);
			inputModel.UserCategories = emptyFormModel.UserCategories
				.Select(c => mapper.Map<CategoryViewModel>(c));
			inputModel.UserAccounts = emptyFormModel.UserAccounts
				.Select(a => mapper.Map<AccountDropdownViewModel>(a));
		}
	}
}
