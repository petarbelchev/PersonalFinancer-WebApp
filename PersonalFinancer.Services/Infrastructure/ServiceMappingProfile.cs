namespace PersonalFinancer.Services.Infrastructure
{
    using AutoMapper;
    using PersonalFinancer.Data.Models;
    using PersonalFinancer.Data.Models.Contracts;
    using PersonalFinancer.Services.Accounts.Models;
    using PersonalFinancer.Services.ApiService.Models;
    using PersonalFinancer.Services.Messages.Models;
    using PersonalFinancer.Services.Shared.Models;
    using PersonalFinancer.Services.User.Models;

    public class ServiceMappingProfile : Profile
    {
        public ServiceMappingProfile()
        {
            _ = this.CreateMap<Category, CategoryServiceModel>();

            _ = this.CreateMap<Currency, CurrencyServiceModel>();

            _ = this.CreateMap<Account, AccountServiceModel>();
            _ = this.CreateMap<Account, AccountCardServiceModel>();
            _ = this.CreateMap<Account, AccountCardServiceModel>();
            _ = this.CreateMap<Account, AccountDetailsShortServiceModel>();
            _ = this.CreateMap<AccountFormShortServiceModel, Account>()
                .ForMember(m => m.Name, mf => mf.MapFrom(s => s.Name.Trim()));

            _ = this.CreateMap<AccountType, AccountTypeServiceModel>();

            _ = this.CreateMap<Account, AccountFormServiceModel>()
                .ForMember(m => m.Currencies, mf => mf
                    .MapFrom(s => s.Owner.Currencies.Where(c => !c.IsDeleted)))
                .ForMember(m => m.AccountTypes, mf => mf
                    .MapFrom(s => s.Owner.AccountTypes.Where(at => !at.IsDeleted)));

            _ = this.CreateMap<Transaction, TransactionDetailsServiceModel>()
                .ForMember(m => m.CategoryName, mf => mf
                    .MapFrom(s => s.Category.Name + (s.Category.IsDeleted ? " (Deleted)" : string.Empty)))
                .ForMember(m => m.AccountName, mf => mf
                    .MapFrom(s => s.Account.Name + (s.Account.IsDeleted ? " (Deleted)" : string.Empty)));

            _ = this.CreateMap<TransactionFormShortServiceModel, Transaction>().ReverseMap()
                .ForMember(m => m.Refference, mf => mf.MapFrom(s => s.Refference.Trim()));

            _ = this.CreateMap<Transaction, TransactionTableServiceModel>();

            _ = this.CreateMap<ApplicationUser, UserServiceModel>();

            _ = this.CreateMap<ApplicationUser, UserDetailsServiceModel>()
                .ForMember(m => m.Accounts, mf => mf
                    .MapFrom(s => s.Accounts.Where(a => !a.IsDeleted).OrderBy(a => a.Name)));

            _ = this.CreateMap<ApiEntity, ApiOutputServiceModel>();

            _ = this.CreateMap<Reply, ReplyOutputServiceModel>();
            _ = this.CreateMap<MessageInputServiceModel, Message>();
            _ = this.CreateMap<ReplyInputServiceModel, Reply>();
        }
    }
}
