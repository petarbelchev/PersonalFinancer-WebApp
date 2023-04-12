namespace PersonalFinancer.Services.Infrastructure
{
	using AutoMapper;

	using Data.Models;

	using Services.Accounts.Models;
	using Services.Shared.Models;
	using Services.User.Models;

	public class ServiceMappingProfile : Profile
	{
		public ServiceMappingProfile()
		{
			CreateMap<Category, CategoryServiceModel>();

			CreateMap<Currency, CurrencyServiceModel>();

			CreateMap<Account, AccountServiceModel>();
			CreateMap<Account, AccountCardServiceModel>()
					/*.ForMember(m => m.CurrencyName, mf => mf
						.MapFrom(s => s.Currency.Name))*/;
			CreateMap<Account, AccountCardServiceModel>();

			CreateMap<AccountType, AccountTypeServiceModel>();

			CreateMap<Transaction, TransactionDetailsServiceModel>()
				.ForMember(m => m.CategoryName, mf => mf
					.MapFrom(s => s.Category.Name + (s.Category.IsDeleted ? " (Deleted)" : string.Empty)))
				.ForMember(m => m.AccountName, mf => mf
					.MapFrom(s => s.Account.Name + (s.Account.IsDeleted ? " (Deleted)" : string.Empty)));
			CreateMap<Transaction, TransactionTableServiceModel>()
				/*.ForMember(m => m.CategoryName, mf => mf.MapFrom(s => s.Category.Name))*/;

			CreateMap<ApplicationUser, UserServiceModel>();
			CreateMap<ApplicationUser, UserDetailsServiceModel>()
				.ForMember(m => m.Accounts, mf => mf
					.MapFrom(s => s.Accounts.Where(a => !a.IsDeleted).OrderBy(a => a.Name)));
		}
	}
}
