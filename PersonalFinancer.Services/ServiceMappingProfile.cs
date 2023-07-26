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
			this.CreateMap<Category, DropdownDTO>()
				.ForMember(dest => dest.Name, opt => opt
					.MapFrom(src => src.Name + (src.IsDeleted ? " (Deleted)" : string.Empty)));

			this.CreateMap<Currency, DropdownDTO>()
				.ForMember(dest => dest.Name, opt => opt
					.MapFrom(src => src.Name + (src.IsDeleted ? " (Deleted)" : string.Empty)));

			this.CreateMap<Account, DropdownDTO>()
				.ForMember(dest => dest.Name, opt => opt
					.MapFrom(src => src.Name + (src.IsDeleted ? " (Deleted)" : string.Empty)));

			this.CreateMap<Account, AccountCardDTO>();
			this.CreateProjection<Account, AccountDetailsDTO>();

			DateTime fromLocalTime = default;
			DateTime toLocalTime = default;
			int page = 1;

			this.CreateProjection<Account, TransactionsDTO>()
				.ForMember(dest => dest.Transactions, opt => opt
					.MapFrom(src => src.Transactions
						.Where(t => t.CreatedOnUtc >= fromLocalTime.ToUniversalTime() 
									&& t.CreatedOnUtc <= toLocalTime.ToUniversalTime())
						.OrderByDescending(t => t.CreatedOnUtc)
						.Skip(TransactionsPerPage * (page - 1))
						.Take(TransactionsPerPage)))
				.ForMember(dest => dest.TotalTransactionsCount, opt => opt
					.MapFrom(src => src.Transactions
						.Count(t => t.CreatedOnUtc >= fromLocalTime.ToUniversalTime()
									&& t.CreatedOnUtc <= toLocalTime.ToUniversalTime())));

			this.CreateMap<Account, CreateEditAccountOutputDTO>()
				.ForMember(dest => dest.Name, opt => opt
					.MapFrom(src => src.Name))
				.ForMember(dest => dest.OwnerCurrencies, opt => opt
					.MapFrom(src => src.Owner.Currencies
						.Where(c => !c.IsDeleted || c.Id == src.CurrencyId)
						.OrderBy(c => c.Name)))
				.ForMember(dest => dest.OwnerAccountTypes, opt => opt
					.MapFrom(src => src.Owner.AccountTypes
						.Where(at => !at.IsDeleted || at.Id == src.AccountTypeId)
						.OrderBy(at => at.Name)));

			this.CreateMap<CreateEditAccountInputDTO, Account>()
				.ForMember(dest => dest.Name, opt => opt
					.MapFrom(src => src.Name.Trim()))
				.ForMember(dest => dest.Owner, opt => opt.Ignore());

			this.CreateMap<AccountType, DropdownDTO>()
				.ForMember(dest => dest.Name, opt => opt
					.MapFrom(src => src.Name + (src.IsDeleted ? " (Deleted)" : string.Empty)));

			this.CreateMap<Transaction, CreateEditTransactionOutputDTO>()
				.ForMember(dest => dest.CreatedOnLocalTime, opt => opt
					.MapFrom(src => src.CreatedOnUtc.ToLocalTime()))
				.ForMember(dest => dest.OwnerCategories, opt => opt
					.MapFrom(src => src.Owner.Categories
						.Where(c => !c.IsDeleted || c.Id == src.CategoryId)
						.OrderBy(c => c.Name)))
				.ForMember(dest => dest.OwnerAccounts, opt => opt
					.MapFrom(src => src.Owner.Accounts
						.Where(a => !a.IsDeleted)
						.OrderBy(a => a.Name)));
			
			this.CreateMap<CreateEditTransactionInputDTO, Transaction>()
				.ForMember(dest => dest.Reference, opt => opt
					.MapFrom(src => src.Reference.Trim()))
				.ForMember(dest => dest.CreatedOnUtc, opt => opt
					.MapFrom(src => src.CreatedOnLocalTime.ToUniversalTime()))
				.ForMember(dest => dest.Owner, opt => opt.Ignore())
				.ReverseMap()
				.ForMember(dest => dest.CreatedOnLocalTime, opt => opt
					.MapFrom(src => src.CreatedOnUtc.ToLocalTime()));

			this.CreateMap<Transaction, TransactionDetailsDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt
					.MapFrom(src => src.Category.Name + (src.Category.IsDeleted ? " (Deleted)" : string.Empty)))
                .ForMember(dest => dest.AccountName, opt => opt
					.MapFrom(src => src.Account.Name + (src.Account.IsDeleted ? " (Deleted)" : string.Empty)))
				.ForMember(dest => dest.AccountCurrencyName, opt => opt
					.MapFrom(src => src.Account.Currency.Name + (src.Account.Currency.IsDeleted ? " (Deleted)" : string.Empty)))
				.ForMember(dest => dest.CreatedOnLocalTime, opt => opt
					.MapFrom(src => src.CreatedOnUtc.ToLocalTime()));

			this.CreateMap<Transaction, TransactionTableDTO>()
				.ForMember(dest => dest.AccountCurrencyName, opt => opt
					.MapFrom(src => src.Account.Currency.Name + (src.Account.Currency.IsDeleted ? " (Deleted)" : string.Empty)))
				.ForMember(dest => dest.CategoryName, opt => opt
					.MapFrom(src => src.Category.Name + (src.Category.IsDeleted ? " (Deleted)" : string.Empty)))
				.ForMember(dest => dest.CreatedOnLocalTime, opt => opt
					.MapFrom(src => src.CreatedOnUtc.ToLocalTime()))
				.ForMember(dest => dest.TransactionType, opt => opt
					.MapFrom(src => src.TransactionType.ToString()));

			this.CreateMap<ApplicationUser, UserInfoDTO>();

			this.CreateMap<ApplicationUser, UserDetailsDTO>()
                .ForMember(dest => dest.Accounts, opt => opt
                    .MapFrom(src => src.Accounts
						.Where(a => !a.IsDeleted)
						.OrderBy(a => a.Name)));

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

			this.CreateProjection<ApplicationUser, UserUsedDropdownsDTO>()
				.ForMember(dest => dest.OwnerAccounts, opt => opt
					.MapFrom(src => src.Accounts
						.Where(a => !a.IsDeleted || a.Transactions.Any())))
				.ForMember(dest => dest.OwnerAccountTypes, opt => opt
					.MapFrom(src => src.AccountTypes
						.Where(at => !at.IsDeleted || at.Accounts.Any(a => a.Transactions.Any()))))
				.ForMember(dest => dest.OwnerCurrencies, opt => opt
					.MapFrom(src => src.Currencies
						.Where(c => !c.IsDeleted || c.Accounts.Any(a => a.Transactions.Any()))))
				.ForMember(dest => dest.OwnerCategories, opt => opt
					.MapFrom(src => src.Categories
						.Where(c => !c.IsDeleted || c.Transactions.Any())));

			this.CreateMap<BaseApiEntity, ApiEntityDTO>();

			this.CreateMap<MessageInputDTO, Message>()
				.ForMember(dest => dest.Subject, opt => opt
					.MapFrom(src => src.Subject.Trim()))
				.ForMember(dest => dest.Content, opt => opt
					.MapFrom(src => src.Content.Trim()));

			this.CreateMap<Message, MessageOutputDTO>()
				.ForMember(dest => dest.IsSeen, opt => opt
					.MapFrom(src => src.IsSeenByAuthor));
			
			this.CreateMap<ReplyInputDTO, Reply>()
				.ForMember(dest => dest.Content, opt => opt
					.MapFrom(src => src.Content.Trim()));

			this.CreateMap<Reply, ReplyOutputDTO>();

			this.CreateMap<Message, MessageDetailsDTO>();
		}
	}
}
