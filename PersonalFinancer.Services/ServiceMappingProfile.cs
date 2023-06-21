namespace PersonalFinancer.Services
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
			this.CreateMap<Category, CategoryServiceModel>();

			this.CreateMap<Currency, CurrencyServiceModel>();

			this.CreateMap<Account, AccountServiceModel>();
			this.CreateMap<Account, AccountCardServiceModel>();
			this.CreateMap<Account, AccountCardServiceModel>();
			this.CreateMap<Account, AccountDetailsShortServiceModel>();
			this.CreateMap<AccountFormShortServiceModel, Account>()
                .ForMember(m => m.Name, mf => mf.MapFrom(s => s.Name.Trim()));

			this.CreateMap<AccountType, AccountTypeServiceModel>();

			this.CreateMap<Account, AccountFormServiceModel>()
                .ForMember(m => m.Currencies, mf => mf
                    .MapFrom(s => s.Owner.Currencies.Where(c => !c.IsDeleted)))
                .ForMember(m => m.AccountTypes, mf => mf
                    .MapFrom(s => s.Owner.AccountTypes.Where(at => !at.IsDeleted)));

			this.CreateMap<Transaction, TransactionDetailsServiceModel>()
                .ForMember(m => m.CategoryName, mf => mf
                    .MapFrom(s => s.Category.Name + (s.Category.IsDeleted ? " (Deleted)" : string.Empty)))
                .ForMember(m => m.AccountName, mf => mf
                    .MapFrom(s => s.Account.Name + (s.Account.IsDeleted ? " (Deleted)" : string.Empty)));

			this.CreateMap<TransactionFormModel, Transaction>().ReverseMap()
                .ForMember(m => m.Reference, mf => mf.MapFrom(s => s.Reference.Trim()));

			this.CreateMap<Transaction, TransactionTableServiceModel>();

			this.CreateMap<ApplicationUser, UserServiceModel>();

			this.CreateMap<ApplicationUser, UserDetailsServiceModel>()
                .ForMember(m => m.Accounts, mf => mf
                    .MapFrom(s => s.Accounts.Where(a => !a.IsDeleted).OrderBy(a => a.Name)));

			this.CreateMap<ApiEntity, ApiOutputServiceModel>();

			this.CreateMap<Reply, ReplyOutputServiceModel>();
			this.CreateMap<MessageInputServiceModel, Message>();
			this.CreateMap<ReplyInputServiceModel, Reply>();
        }
    }
}
