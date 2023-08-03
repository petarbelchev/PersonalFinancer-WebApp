namespace PersonalFinancer.Services
{
	using AutoMapper;
	using PersonalFinancer.Data.Models;
	using PersonalFinancer.Data.Models.Contracts;
	using PersonalFinancer.Services.Accounts.Models;
	using PersonalFinancer.Services.Api.Models;
	using PersonalFinancer.Services.Messages.Models;
	using PersonalFinancer.Services.Shared.Models;
	using PersonalFinancer.Services.Users.Models;
	using static PersonalFinancer.Common.Constants.PaginationConstants;

	public class ServiceMappingProfile : Profile
    {
        public ServiceMappingProfile()
        {
			this.CreateMap<Category, DropdownDTO>()
				.ForMember(d => d.Name, opt => opt
					.MapFrom(s => s.Name + (s.IsDeleted ? " (Deleted)" : string.Empty)));

			this.CreateMap<Currency, DropdownDTO>()
				.ForMember(d => d.Name, opt => opt
					.MapFrom(s => s.Name + (s.IsDeleted ? " (Deleted)" : string.Empty)));

			this.CreateMap<Account, DropdownDTO>()
				.ForMember(d => d.Name, opt => opt
					.MapFrom(s => s.Name + (s.IsDeleted ? " (Deleted)" : string.Empty)));

			this.CreateMap<Account, AccountCardDTO>();
			this.CreateProjection<Account, AccountDetailsDTO>();

			DateTime fromLocalTime = default;
			DateTime toLocalTime = default;
			int page = 1;

			this.CreateProjection<Account, TransactionsDTO>()
				.ForMember(d => d.Transactions, opt => opt
					.MapFrom(s => s.Transactions
						.Where(t => t.CreatedOnUtc >= fromLocalTime.ToUniversalTime() 
									&& t.CreatedOnUtc <= toLocalTime.ToUniversalTime())
						.OrderByDescending(t => t.CreatedOnUtc)
						.Skip(TransactionsPerPage * (page - 1))
						.Take(TransactionsPerPage)))
				.ForMember(d => d.TotalTransactionsCount, opt => opt
					.MapFrom(s => s.Transactions
						.Count(t => t.CreatedOnUtc >= fromLocalTime.ToUniversalTime()
									&& t.CreatedOnUtc <= toLocalTime.ToUniversalTime())));

			this.CreateMap<Account, CreateEditAccountOutputDTO>()
				.ForMember(d => d.Name, opt => opt
					.MapFrom(s => s.Name))
				.ForMember(d => d.OwnerCurrencies, opt => opt
					.MapFrom(s => s.Owner.Currencies
						.Where(c => !c.IsDeleted || c.Id == s.CurrencyId)
						.OrderBy(c => c.Name)))
				.ForMember(d => d.OwnerAccountTypes, opt => opt
					.MapFrom(s => s.Owner.AccountTypes
						.Where(at => !at.IsDeleted || at.Id == s.AccountTypeId)
						.OrderBy(at => at.Name)));

			this.CreateMap<CreateEditAccountInputDTO, Account>()
				.ForMember(d => d.Name, opt => opt
					.MapFrom(s => s.Name.Trim()))
				.ForMember(d => d.Owner, opt => opt.Ignore());

			this.CreateMap<AccountType, DropdownDTO>()
				.ForMember(d => d.Name, opt => opt
					.MapFrom(s => s.Name + (s.IsDeleted ? " (Deleted)" : string.Empty)));

			this.CreateMap<Transaction, CreateEditTransactionOutputDTO>()
				.ForMember(d => d.CreatedOnLocalTime, opt => opt
					.MapFrom(s => s.CreatedOnUtc.ToLocalTime()))
				.ForMember(d => d.OwnerCategories, opt => opt
					.MapFrom(s => s.Owner.Categories
						.Where(c => !c.IsDeleted || c.Id == s.CategoryId)
						.OrderBy(c => c.Name)))
				.ForMember(d => d.OwnerAccounts, opt => opt
					.MapFrom(s => s.Owner.Accounts
						.Where(a => !a.IsDeleted)
						.OrderBy(a => a.Name)));
			
			this.CreateMap<CreateEditTransactionInputDTO, Transaction>()
				.ForMember(d => d.Reference, opt => opt
					.MapFrom(s => s.Reference.Trim()))
				.ForMember(d => d.CreatedOnUtc, opt => opt
					.MapFrom(s => s.CreatedOnLocalTime.ToUniversalTime()))
				.ForMember(d => d.Owner, opt => opt.Ignore())
				.ReverseMap()
				.ForMember(d => d.CreatedOnLocalTime, opt => opt
					.MapFrom(s => s.CreatedOnUtc.ToLocalTime()));

			this.CreateMap<Transaction, TransactionDetailsDTO>()
                .ForMember(d => d.CategoryName, opt => opt
					.MapFrom(s => s.Category.Name + (s.Category.IsDeleted ? " (Deleted)" : string.Empty)))
                .ForMember(d => d.AccountName, opt => opt
					.MapFrom(s => s.Account.Name + (s.Account.IsDeleted ? " (Deleted)" : string.Empty)))
				.ForMember(d => d.AccountCurrencyName, opt => opt
					.MapFrom(s => s.Account.Currency.Name + (s.Account.Currency.IsDeleted ? " (Deleted)" : string.Empty)))
				.ForMember(d => d.CreatedOnLocalTime, opt => opt
					.MapFrom(s => s.CreatedOnUtc.ToLocalTime()));

			this.CreateMap<Transaction, TransactionTableDTO>()
				.ForMember(d => d.AccountCurrencyName, opt => opt
					.MapFrom(s => s.Account.Currency.Name + (s.Account.Currency.IsDeleted ? " (Deleted)" : string.Empty)))
				.ForMember(d => d.CategoryName, opt => opt
					.MapFrom(s => s.Category.Name + (s.Category.IsDeleted ? " (Deleted)" : string.Empty)))
				.ForMember(d => d.CreatedOnLocalTime, opt => opt
					.MapFrom(s => s.CreatedOnUtc.ToLocalTime()))
				.ForMember(d => d.TransactionType, opt => opt
					.MapFrom(s => s.TransactionType.ToString()));

			this.CreateMap<ApplicationUser, UserInfoDTO>();

			this.CreateMap<ApplicationUser, UserDetailsDTO>()
                .ForMember(d => d.Accounts, opt => opt
                    .MapFrom(s => s.Accounts
						.Where(a => !a.IsDeleted)
						.OrderBy(a => a.Name)));

			this.CreateMap<ApplicationUser, AccountsAndCategoriesDropdownDTO>()
				.ForMember(d => d.OwnerCategories, opt => opt
					.MapFrom(s => s.Categories
						.Where(c => !c.IsDeleted)
						.OrderBy(c => c.Name)))
				.ForMember(d => d.OwnerAccounts, opt => opt
					.MapFrom(s => s.Accounts
						.Where(a => !a.IsDeleted)
						.OrderBy(a => a.Name)));

			this.CreateMap<ApplicationUser, AccountTypesAndCurrenciesDropdownDTO>()
				.ForMember(d => d.OwnerCurrencies, opt => opt
					.MapFrom(s => s.Currencies
						.Where(c => !c.IsDeleted)
						.OrderBy(c => c.Name)))
				.ForMember(d => d.OwnerAccountTypes, opt => opt
					.MapFrom(s => s.AccountTypes
						.Where(at => !at.IsDeleted)
						.OrderBy(at => at.Name)));

			this.CreateProjection<ApplicationUser, UserUsedDropdownsDTO>()
				.ForMember(d => d.OwnerAccounts, opt => opt
					.MapFrom(s => s.Accounts
						.Where(a => !a.IsDeleted || a.Transactions.Any())
						.OrderBy(a => a.Name)))
				.ForMember(d => d.OwnerAccountTypes, opt => opt
					.MapFrom(s => s.AccountTypes
						.Where(at => !at.IsDeleted || at.Accounts.Any(a => a.Transactions.Any()))
						.OrderBy(at => at.Name)))
				.ForMember(d => d.OwnerCurrencies, opt => opt
					.MapFrom(s => s.Currencies
						.Where(c => !c.IsDeleted || c.Accounts.Any(a => a.Transactions.Any()))
						.OrderBy(c => c.Name)))
				.ForMember(d => d.OwnerCategories, opt => opt
					.MapFrom(s => s.Categories
						.Where(c => !c.IsDeleted || c.Transactions.Any())
						.OrderBy(c => c.Name)));

			this.CreateMap<BaseApiEntity, ApiEntityDTO>();

			this.CreateMap<MessageInputDTO, Message>()
				.ForMember(d => d.Subject, opt => opt
					.MapFrom(s => s.Subject.Trim()))
				.ForMember(d => d.Content, opt => opt
					.MapFrom(s => s.Content.Trim()))
				.ForMember(d => d.Image, opt => opt.Ignore());

			this.CreateMap<ReplyInputDTO, Reply>()
				.ForMember(d => d.Content, opt => opt
					.MapFrom(s => s.Content.Trim()));

			this.CreateMap<Reply, ReplyOutputDTO>();

			this.CreateMap<Message, MessageDetailsDTO>();
		}
	}
}
