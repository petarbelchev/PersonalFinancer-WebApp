﻿using AutoMapper;

using PersonalFinancer.Data.Models;
using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Services.Shared.Models;
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
			CreateMap<Account, AccountCardViewModel>()
				/*.ForMember(m => m.CurrencyName, mf => mf
					.MapFrom(s => s.Currency.Name))*/;
			CreateMap<Account, AccountCardExtendedViewModel>();
			CreateMap<Account, AccountFormModel>();

			CreateMap<AccountType, AccountTypeViewModel>();

			CreateMap<Transaction, TransactionDetailsViewModel>()
				.ForMember(m => m.CategoryName, mf => mf
					.MapFrom(s => s.Category.Name + (s.Category.IsDeleted ? " (Deleted)" : string.Empty)))
				.ForMember(m => m.AccountName, mf => mf
					.MapFrom(s => s.Account.Name + (s.Account.IsDeleted ? " (Deleted)" : string.Empty)));
			CreateMap<Transaction, TransactionFormModel>();
			CreateMap<Transaction, TransactionTableViewModel>()
				/*.ForMember(m => m.CategoryName, mf => mf.MapFrom(s => s.Category.Name))*/;

			CreateMap<ApplicationUser, UserViewModel>();
			CreateMap<ApplicationUser, AccountFormModel>();
			CreateMap<ApplicationUser, UserDetailsViewModel>()
				.ForMember(m => m.Accounts, mf => mf
					.MapFrom(s => s.Accounts.Where(a => !a.IsDeleted).OrderBy(a => a.Name)));
		}
	}
}
