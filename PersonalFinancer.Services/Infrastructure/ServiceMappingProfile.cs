using AutoMapper;

using PersonalFinancer.Data.Models;
using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Services.AccountTypes.Models;
using PersonalFinancer.Services.Categories.Models;
using PersonalFinancer.Services.Currencies.Models;
using PersonalFinancer.Services.Transactions.Models;
using PersonalFinancer.Services.User.Models;

namespace PersonalFinancer.Services.Infrastructure
{
	public class ServiceMappingProfile : Profile
	{
		public ServiceMappingProfile()
		{
			CreateMap<Category, CategoryViewModel>();

			CreateMap<Currency, CurrencyViewModel>();

			CreateMap<Account, AccountDropdownViewModel>();
			CreateMap<Account, DeleteAccountViewModel>();
			CreateMap<Account, AccountCardViewModel>();
			CreateMap<Account, EditAccountFormModel>();

			CreateMap<AccountType, AccountTypeViewModel>();

			CreateMap<Transaction, TransactionShortViewModel>()
				.ForMember(m => m.AccountName, mf => mf
					.MapFrom(s => s.Account.Name + (s.Account.IsDeleted ? " (Deleted)" : string.Empty)));
			CreateMap<Transaction, CreateTransactionFormModel>();
			CreateMap<Transaction, EditTransactionFormModel>();
			CreateMap<Transaction, TransactionExtendedViewModel>()
				.ForMember(m => m.CategoryName, mf => mf
					.MapFrom(s => s.Category.Name + (s.Category.IsDeleted ? " (Deleted)" : string.Empty)))
				.ForMember(m => m.AccountName, mf => mf.MapFrom(s => s.Account.Name + (s.Account.IsDeleted ? " (Deleted)" : string.Empty)));

			CreateMap<ApplicationUser, UserViewModel>();
			CreateMap<ApplicationUser, UserDetailsViewModel>()
				.ForMember(m => m.Accounts, mf => mf.MapFrom(s => s.Accounts.Where(a => !a.IsDeleted)));
		}
	}
}
