namespace PersonalFinancer.Services
{
	using AutoMapper;
	using PersonalFinancer.Data.Models;
	using PersonalFinancer.Data.Models.Contracts;
	using PersonalFinancer.Data.Models.Enums;
	using PersonalFinancer.Services.Accounts.Models;
	using PersonalFinancer.Services.Api.Models;
	using PersonalFinancer.Services.Messages.Models;
	using PersonalFinancer.Services.Shared.Models;
	using PersonalFinancer.Services.User.Models;

	public class ServiceMappingProfile : Profile
    {
        public ServiceMappingProfile()
        {
			this.CreateMap<Category, CategoryDropdownDTO>()
				.ForMember(x => x.Name, y => y
					.MapFrom(z => z.Name + (z.IsDeleted ? " (Deleted)" : string.Empty)));

			this.CreateMap<Currency, CurrencyDropdownDTO>()
				.ForMember(x => x.Name, y => y
					.MapFrom(z => z.Name + (z.IsDeleted ? " (Deleted)" : string.Empty)));

			this.CreateMap<Account, AccountDropdownDTO>()
				.ForMember(x => x.Name, y => y
					.MapFrom(z => z.Name + (z.IsDeleted ? " (Deleted)" : string.Empty)));

			this.CreateMap<Account, AccountCardDTO>();
			this.CreateMap<Account, AccountDetailsShortDTO>();

			this.CreateMap<CreateEditAccountDTO, Account>().ReverseMap()
                .ForMember(x => x.Name, y => y
					.MapFrom(z => z.Name.Trim()))
				.ForMember(x => x.OwnerCurrencies, y => y
					.MapFrom(z => z.Owner.Currencies
						.Where(c => !c.IsDeleted)
						.OrderBy(c => c.Name)))
				.ForMember(x => x.OwnerAccountTypes, y => y
					.MapFrom(z => z.Owner.AccountTypes
						.Where(at => !at.IsDeleted)
						.OrderBy(at => at.Name)));

			this.CreateMap<AccountType, AccountTypeDropdownDTO>()
				.ForMember(x => x.Name, y => y
					.MapFrom(z => z.Name + (z.IsDeleted ? " (Deleted)" : string.Empty)));

			this.CreateMap<Transaction, CreateEditTransactionDTO>().ReverseMap();

			this.CreateMap<Transaction, TransactionDetailsDTO>()
                .ForMember(x => x.CategoryName, y => y
					.MapFrom(z => z.Category.Name + (z.Category.IsDeleted ? " (Deleted)" : string.Empty)))
                .ForMember(x => x.AccountName, y => y
					.MapFrom(z => z.Account.Name + (z.Account.IsDeleted ? " (Deleted)" : string.Empty)));

			this.CreateMap<Transaction, TransactionTableDTO>()
				.ForMember(x => x.AccountCurrencyName, y => y
					.MapFrom(z => z.Account.Currency.Name + (z.Account.Currency.IsDeleted ? " (Deleted)" : string.Empty)))
				.ForMember(x => x.CategoryName, y => y
					.MapFrom(z => z.Category.Name + (z.Category.IsDeleted ? " (Deleted)" : string.Empty)))
				.ForMember(x => x.CreatedOn, y => y
					.MapFrom(z => z.CreatedOn.ToLocalTime()))
				.ForMember(x => x.TransactionType, y => y
					.MapFrom(z => z.TransactionType.ToString()));

			this.CreateMap<TransactionsDTO, TransactionsPageDTO>();

			this.CreateMap<ApplicationUser, UserInfoDTO>();

			this.CreateMap<ApplicationUser, UserDetailsDTO>()
                .ForMember(x => x.Accounts, y => y
                    .MapFrom(z => z.Accounts.Where(a => !a.IsDeleted).OrderBy(a => a.Name)));

			this.CreateMap<BaseApiEntity, ApiEntityDTO>();

			this.CreateMap<Reply, ReplyOutputDTO>();
			this.CreateMap<MessageInputDTO, Message>();
			this.CreateMap<ReplyInputDTO, Reply>();
        }
    }
}
