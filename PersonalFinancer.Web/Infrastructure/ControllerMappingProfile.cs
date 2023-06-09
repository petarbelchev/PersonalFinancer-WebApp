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
            _ = this.CreateMap<Account, DeleteAccountViewModel>();
            _ = this.CreateMap<Account, AccountFormViewModel>();
            _ = this.CreateMap<AccountDetailsServiceModel, AccountDetailsViewModel>();
            _ = this.CreateMap<AccountFormServiceModel, AccountFormViewModel>();
            _ = this.CreateMap<AccountFormViewModel, AccountFormShortServiceModel>();
            _ = this.CreateMap<AccountDetailsShortServiceModel, AccountDetailsViewModel>();

            _ = this.CreateMap<Transaction, TransactionFormModel>();
            _ = this.CreateMap<TransactionFormModel, TransactionFormShortServiceModel>();
            _ = this.CreateMap<TransactionFormServiceModel, TransactionFormModel>();
            _ = this.CreateMap<TransactionsServiceModel, UserTransactionsViewModel>();
            _ = this.CreateMap<DateFilterModel, UserTransactionsViewModel>();
            _ = this.CreateMap<UserAccountsAndCategoriesServiceModel, TransactionFormModel>();

            _ = this.CreateMap<ApplicationUser, AccountFormViewModel>();

            _ = this.CreateMap<MessageDetailsServiceModel, MessageDetailsViewModel>();
        }
    }
}
