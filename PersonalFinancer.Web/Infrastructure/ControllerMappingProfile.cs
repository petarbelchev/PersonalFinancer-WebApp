namespace PersonalFinancer.Web.Infrastructure
{
    using AutoMapper;
    using PersonalFinancer.Data.Models;
    using PersonalFinancer.Services.Accounts.Models;
    using PersonalFinancer.Services.Messages.Models;
    using PersonalFinancer.Services.Shared.Models;
    using PersonalFinancer.Services.User.Models;
    using PersonalFinancer.Web.Models.Account;
    using PersonalFinancer.Web.Models.Message;
    using PersonalFinancer.Web.Models.Shared;
    using PersonalFinancer.Web.Models.Transaction;

    public class ControllerMappingProfile : Profile
    {
        public ControllerMappingProfile()
        {
            this.CreateMap<Account, DeleteAccountViewModel>();
            this.CreateMap<Account, AccountFormViewModel>();
            this.CreateMap<AccountDetailsServiceModel, AccountDetailsViewModel>();
            this.CreateMap<AccountFormServiceModel, AccountFormViewModel>();
            this.CreateMap<AccountFormViewModel, AccountFormShortServiceModel>();
            this.CreateMap<AccountDetailsShortServiceModel, AccountDetailsViewModel>();

            this.CreateMap<Transaction, TransactionFormModel>();
            this.CreateMap<TransactionFormModel, TransactionFormShortServiceModel>();
            this.CreateMap<TransactionFormServiceModel, TransactionFormModel>();
            this.CreateMap<TransactionsServiceModel, UserTransactionsViewModel>();
            this.CreateMap<DateFilterModel, UserTransactionsViewModel>();
            this.CreateMap<UserAccountsAndCategoriesServiceModel, TransactionFormModel>();

            this.CreateMap<ApplicationUser, AccountFormViewModel>();

            this.CreateMap<MessageDetailsServiceModel, MessageDetailsViewModel>();
        }
    }
}
