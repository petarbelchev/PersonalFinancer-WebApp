namespace PersonalFinancer.Services
{
	using AutoMapper;
	using PersonalFinancer.Data.Models;
	using PersonalFinancer.Data.Models.Contracts;
	using PersonalFinancer.Services.Accounts.Models;
	using PersonalFinancer.Services.Api.Models;
	using PersonalFinancer.Services.Messages.Models;
	using PersonalFinancer.Services.Shared.Models;
	using PersonalFinancer.Services.User.Models;

	public class ServiceMappingProfile : Profile
    {
        public ServiceMappingProfile()
        {
			this.CreateMap<Category, CategoryServiceModel>()
				.ForMember(m => m.Name, mf => mf
					.MapFrom(s => s.Name + (s.IsDeleted ? " (Deleted)" : string.Empty)));

			this.CreateMap<Currency, CurrencyServiceModel>()
				.ForMember(m => m.Name, mf => mf
					.MapFrom(s => s.Name + (s.IsDeleted ? " (Deleted)" : string.Empty)));

			this.CreateMap<Account, AccountServiceModel>()
				.ForMember(m => m.Name, mf => mf
					.MapFrom(s => s.Name + (s.IsDeleted ? " (Deleted)" : string.Empty)));
			this.CreateMap<Account, AccountCardServiceModel>();
			this.CreateMap<Account, AccountCardServiceModel>();
			this.CreateMap<Account, AccountDetailsShortServiceModel>();
			this.CreateMap<AccountFormShortServiceModel, Account>().ReverseMap()
                .ForMember(m => m.Name, mf => mf.MapFrom(s => s.Name.Trim()));

			this.CreateMap<AccountType, AccountTypeServiceModel>()
				.ForMember(m => m.Name, mf => mf
					.MapFrom(s => s.Name + (s.IsDeleted ? " (Deleted)" : string.Empty)));

			this.CreateMap<Transaction, TransactionDetailsServiceModel>()
                .ForMember(m => m.CategoryName, mf => mf
                    .MapFrom(s => s.Category.Name + (s.Category.IsDeleted ? " (Deleted)" : string.Empty)))
                .ForMember(m => m.AccountName, mf => mf
                    .MapFrom(s => s.Account.Name + (s.Account.IsDeleted ? " (Deleted)" : string.Empty)));

			this.CreateMap<TransactionFormModel, Transaction>().ReverseMap()
                .ForMember(m => m.Reference, mf => mf.MapFrom(s => s.Reference.Trim()));

			this.CreateMap<Transaction, TransactionTableServiceModel>()
				.ForMember(m => m.CategoryName, mf => mf
					.MapFrom(s => s.Category.Name + (s.Category.IsDeleted ? " (Deleted)" : string.Empty)));

			this.CreateMap<ApplicationUser, UserServiceModel>();

			this.CreateMap<ApplicationUser, UserDetailsServiceModel>()
                .ForMember(m => m.Accounts, mf => mf
                    .MapFrom(s => s.Accounts.Where(a => !a.IsDeleted).OrderBy(a => a.Name)));

			this.CreateMap<BaseCacheableApiEntity, ApiOutputServiceModel>();

			this.CreateMap<Reply, ReplyOutputServiceModel>();
			this.CreateMap<MessageInputServiceModel, Message>();
			this.CreateMap<ReplyInputServiceModel, Reply>();
        }
    }
}
