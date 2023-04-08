namespace PersonalFinancer.Services.Infrastructure
{
	using AutoMapper;

	using Data.Models;

	using Services.Accounts.Models;
	using Services.Categories.Models;
	using Services.Currencies.Models;
	using Services.Shared.Models;
	using Services.User.Models;

	public class ServiceMappingProfile : Profile
	{
		public ServiceMappingProfile()
		{
			CreateMap<Category, CategoryOutputDTO>();

			CreateMap<Currency, CurrencyOutputDTO>();

			CreateMap<Account, DeleteAccountDTO>();
			CreateMap<Account, AccountDTO>();
			CreateMap<Account, AccountCardDTO>();
			CreateMap<Account, AccountCardExtendedDTO>()
				.ForMember(m => m.CurrencyName, mf => mf.MapFrom(s => s.Currency.Name));

			CreateMap<AccountType, AccountTypeOutputDTO>();

			CreateMap<Transaction, TransactionDetailsDTO>()
				.ForMember(m => m.CategoryName, mf => mf
					.MapFrom(s => s.Category.Name + (s.Category.IsDeleted ? " (Deleted)" : string.Empty)))
				.ForMember(m => m.AccountName, mf => mf
					.MapFrom(s => s.Account.Name + (s.Account.IsDeleted ? " (Deleted)" : string.Empty)));
			CreateMap<Transaction, TransactionTableDTO>()
				.ForMember(m => m.CategoryName, mf => mf.MapFrom(s => s.Category.Name));

			CreateMap<ApplicationUser, UserDTO>();
			CreateMap<ApplicationUser, UserDetailsDTO>()
				.ForMember(m => m.Accounts, mf => mf.MapFrom(s => s.Accounts.Where(a => !a.IsDeleted).OrderBy(a => a.Name)));
		}
	}
}
