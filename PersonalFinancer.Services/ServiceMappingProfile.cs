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
	using static PersonalFinancer.Common.Constants.PaginationConstants;

	public class ServiceMappingProfile : Profile
    {
        public ServiceMappingProfile()
        {
			this.CreateMap<Category, CategoryDropdownDTO>()
				.ForMember(dest => dest.Name, opt => opt
					.MapFrom(src => src.Name + (src.IsDeleted ? " (Deleted)" : string.Empty)));

			this.CreateMap<Currency, CurrencyDropdownDTO>()
				.ForMember(dest => dest.Name, opt => opt
					.MapFrom(src => src.Name + (src.IsDeleted ? " (Deleted)" : string.Empty)));

			this.CreateMap<Account, AccountDropdownDTO>()
				.ForMember(dest => dest.Name, opt => opt
					.MapFrom(src => src.Name + (src.IsDeleted ? " (Deleted)" : string.Empty)));

			this.CreateMap<Account, AccountCardDTO>();
			this.CreateMap<Account, AccountDetailsShortDTO>();

			DateTime startDate = default;
			DateTime endDate = default;

			this.CreateProjection<Account, AccountDetailsLongDTO>()
				.ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => startDate))
				.ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => endDate))
				.ForMember(dest => dest.Transactions, opt => opt
					.MapFrom(src => src.Transactions
						.Where(t =>	t.CreatedOn >= startDate.ToUniversalTime()
									&& t.CreatedOn <= endDate.ToUniversalTime())
						.OrderByDescending(t => t.CreatedOn)
						.Take(TransactionsPerPage)))
				.ForMember(dest => dest.TotalAccountTransactions, opt => opt
					.MapFrom(src => src.Transactions
						.Count(t => t.CreatedOn >= startDate.ToUniversalTime()
									&& t.CreatedOn <= endDate.ToUniversalTime())));

			int page = 1;

			this.CreateProjection<Account, TransactionsDTO>()
				.ForMember(dest => dest.Transactions, opt => opt
					.MapFrom(src => src.Transactions
						.Where(t => t.CreatedOn >= startDate.ToUniversalTime() 
									&& t.CreatedOn <= endDate.ToUniversalTime())
						.OrderByDescending(t => t.CreatedOn)
						.Skip(TransactionsPerPage * (page - 1))
						.Take(TransactionsPerPage)))
				.ForMember(dest => dest.TotalTransactionsCount, opt => opt
					.MapFrom(src => src.Transactions
						.Count(t => t.CreatedOn >= startDate.ToUniversalTime()
									&& t.CreatedOn <= endDate.ToUniversalTime())));

			this.CreateMap<Account, CreateEditAccountDTO>()
                .ForMember(dest => dest.Name, opt => opt
					.MapFrom(src => src.Name))
				.ForMember(dest => dest.OwnerCurrencies, opt => opt
					.MapFrom(src => src.Owner.Currencies
						.Where(c => !c.IsDeleted || c.Id == src.CurrencyId)
						.OrderBy(c => c.Name)))
				.ForMember(dest => dest.OwnerAccountTypes, opt => opt
					.MapFrom(src => src.Owner.AccountTypes
						.Where(at => !at.IsDeleted || at.Id == src.AccountTypeId)
						.OrderBy(at => at.Name)))
				.ReverseMap()
				.ForMember(dest => dest.Name, opt => opt
					.MapFrom(src => src.Name.Trim()))
				.ForMember(dest => dest.Owner, opt => opt.Ignore());

			this.CreateMap<AccountType, AccountTypeDropdownDTO>()
				.ForMember(dest => dest.Name, opt => opt
					.MapFrom(src => src.Name + (src.IsDeleted ? " (Deleted)" : string.Empty)));

			this.CreateMap<Transaction, CreateEditTransactionDTO>()
				.ForMember(dest => dest.CreatedOn, opt => opt
					.MapFrom(src => src.CreatedOn.ToLocalTime()))
				.ForMember(dest => dest.OwnerCategories, opt => opt
					.MapFrom(src => src.Owner.Categories
						.Where(c => !c.IsDeleted || c.Id == src.CategoryId)
						.OrderBy(c => c.Name)))
				.ForMember(dest => dest.OwnerAccounts, opt => opt
					.MapFrom(src => src.Owner.Accounts
						.Where(a => !a.IsDeleted)
						.OrderBy(a => a.Name)))
				.ReverseMap()
				.ForMember(dest => dest.Reference, opt => opt
					.MapFrom(src => src.Reference.Trim()))
				.ForMember(dest => dest.CreatedOn, opt => opt
					.MapFrom(src => src.CreatedOn.ToUniversalTime()))
				.ForMember(dest => dest.Owner, opt => opt.Ignore());

			this.CreateMap<Transaction, TransactionDetailsDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt
					.MapFrom(src => src.Category.Name + (src.Category.IsDeleted ? " (Deleted)" : string.Empty)))
                .ForMember(dest => dest.AccountName, opt => opt
					.MapFrom(src => src.Account.Name + (src.Account.IsDeleted ? " (Deleted)" : string.Empty)))
				.ForMember(dest => dest.AccountCurrencyName, opt => opt
					.MapFrom(src => src.Account.Currency.Name + (src.Account.Currency.IsDeleted ? " (Deleted)" : string.Empty)))
				.ForMember(dest => dest.CreatedOn, opt => opt
					.MapFrom(src => src.CreatedOn.ToLocalTime()));

			this.CreateMap<Transaction, TransactionTableDTO>()
				.ForMember(dest => dest.AccountCurrencyName, opt => opt
					.MapFrom(src => src.Account.Currency.Name + (src.Account.Currency.IsDeleted ? " (Deleted)" : string.Empty)))
				.ForMember(dest => dest.CategoryName, opt => opt
					.MapFrom(src => src.Category.Name + (src.Category.IsDeleted ? " (Deleted)" : string.Empty)))
				.ForMember(dest => dest.CreatedOn, opt => opt
					.MapFrom(src => src.CreatedOn.ToLocalTime()))
				.ForMember(dest => dest.TransactionType, opt => opt
					.MapFrom(src => src.TransactionType.ToString()));

			this.CreateMap<TransactionsDTO, TransactionsPageDTO>();

			this.CreateMap<ApplicationUser, UserInfoDTO>();

			this.CreateMap<ApplicationUser, UserDetailsDTO>()
                .ForMember(dest => dest.Accounts, opt => opt
                    .MapFrom(src => src.Accounts.Where(a => !a.IsDeleted).OrderBy(a => a.Name)));

			this.CreateMap<ApplicationUser, AccountsAndCategoriesDropdownDTO>()
				.ForMember(dest => dest.OwnerCategories, opt => opt
					.MapFrom(src => src.Categories
						.Where(c => !c.IsDeleted)
						.OrderBy(c => c.Name)))
				.ForMember(dest => dest.OwnerAccounts, opt => opt
					.MapFrom(src => src.Accounts
						.Where(a => !a.IsDeleted)
						.OrderBy(a => a.Name)));

			this.CreateMap<ApplicationUser, AccountTypesAndCurrenciesDropdownDTO>()
				.ForMember(dest => dest.OwnerCurrencies, opt => opt
					.MapFrom(src => src.Currencies
						.Where(c => !c.IsDeleted)
						.OrderBy(c => c.Name)))
				.ForMember(dest => dest.OwnerAccountTypes, opt => opt
					.MapFrom(src => src.AccountTypes
						.Where(at => !at.IsDeleted)
						.OrderBy(at => at.Name)));

			this.CreateMap<BaseApiEntity, ApiEntityDTO>();

			this.CreateMap<Reply, ReplyOutputDTO>();
			this.CreateMap<MessageInputDTO, Message>();
			this.CreateMap<Message, MessageOutputDTO>();
			this.CreateMap<ReplyInputDTO, Reply>();
        }
    }
}
