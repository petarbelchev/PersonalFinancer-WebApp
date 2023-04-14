namespace PersonalFinancer.Services.Infrastructure
{
	using AutoMapper;

	using Data.Models;

	using Services.Accounts.Models;
	using Services.ApiService.Models;
	using Services.Shared.Models;
	using Services.User.Models;

	public class ServiceMappingProfile : Profile
	{
		public ServiceMappingProfile()
		{
			CreateMap<Category, CategoryServiceModel>();

			CreateMap<Currency, CurrencyServiceModel>();

			CreateMap<Account, AccountServiceModel>();
			CreateMap<Account, AccountCardServiceModel>();
			CreateMap<Account, AccountCardServiceModel>();
			CreateMap<AccountFormShortServiceModel, Account>()
				.ForMember(m => m.Name, mf => mf.MapFrom(s => s.Name.Trim()));

			CreateMap<AccountType, AccountTypeServiceModel>();

			CreateMap<Transaction, TransactionDetailsServiceModel>()
				.ForMember(m => m.CategoryName, mf => mf
					.MapFrom(s => s.Category.Name + (s.Category.IsDeleted ? " (Deleted)" : string.Empty)))
				.ForMember(m => m.AccountName, mf => mf
					.MapFrom(s => s.Account.Name + (s.Account.IsDeleted ? " (Deleted)" : string.Empty)));
			CreateMap<TransactionFormShortServiceModel, Transaction>()
				.ForMember(m => m.Refference, mf => mf.MapFrom(s => s.Refference.Trim()));

			CreateMap<ApplicationUser, UserServiceModel>();
			CreateMap<ApplicationUser, UserDetailsServiceModel>()
				.ForMember(m => m.Accounts, mf => mf
					.MapFrom(s => s.Accounts.Where(a => !a.IsDeleted).OrderBy(a => a.Name)));

			CreateMap<ApiEntity, ApiOutputServiceModel>();
		}
	}
}
