using AutoMapper;
using PersonalFinancer.Data.Models;
using PersonalFinancer.Data.Models.Contracts;
using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Services.ApiService.Models;
using PersonalFinancer.Services.Messages.Models;
using PersonalFinancer.Services.Shared.Models;
using PersonalFinancer.Services.User.Models;

namespace PersonalFinancer.Services.Infrastructure
{
    public class ServiceMappingProfile : Profile
	{
		public ServiceMappingProfile()
		{
			CreateMap<Category, CategoryServiceModel>();

			CreateMap<Currency, CurrencyServiceModel>();

			CreateMap<Account, AccountServiceModel>();
			CreateMap<Account, AccountCardServiceModel>();
			CreateMap<Account, AccountCardServiceModel>();
			CreateMap<Account, AccountDetailsShortServiceModel>();
			CreateMap<AccountFormShortServiceModel, Account>()
				.ForMember(m => m.Name, mf => mf.MapFrom(s => s.Name.Trim()));

			CreateMap<AccountType, AccountTypeServiceModel>();

			CreateMap<Account, AccountFormServiceModel>()
				.ForMember(m => m.Currencies, mf => mf
					.MapFrom(s => s.Owner.Currencies.Where(c => !c.IsDeleted)))
				.ForMember(m => m.AccountTypes, mf => mf
					.MapFrom(s => s.Owner.AccountTypes.Where(at => !at.IsDeleted)));


			CreateMap<Transaction, TransactionDetailsServiceModel>()
				.ForMember(m => m.CategoryName, mf => mf
					.MapFrom(s => s.Category.Name + (s.Category.IsDeleted ? " (Deleted)" : string.Empty)))
				.ForMember(m => m.AccountName, mf => mf
					.MapFrom(s => s.Account.Name + (s.Account.IsDeleted ? " (Deleted)" : string.Empty)));

			CreateMap<TransactionFormShortServiceModel, Transaction>().ReverseMap()
				.ForMember(m => m.Refference, mf => mf.MapFrom(s => s.Refference.Trim()));

			CreateMap<Transaction, TransactionTableServiceModel>();

			CreateMap<ApplicationUser, UserServiceModel>();

			CreateMap<ApplicationUser, UserDetailsServiceModel>()
				.ForMember(m => m.Accounts, mf => mf
					.MapFrom(s => s.Accounts.Where(a => !a.IsDeleted).OrderBy(a => a.Name)));

			CreateMap<ApiEntity, ApiOutputServiceModel>();

			CreateMap<Reply, ReplyOutputServiceModel>();
			CreateMap<MessageInputServiceModel, Message>();
			CreateMap<ReplyInputServiceModel, Reply>();
		}
	}
}
