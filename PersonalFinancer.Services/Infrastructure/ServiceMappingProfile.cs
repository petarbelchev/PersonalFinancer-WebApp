﻿namespace PersonalFinancer.Services.Infrastructure
{
	using AutoMapper;

	using Data.Models;
	using PersonalFinancer.Services.Account.Models;
	using PersonalFinancer.Services.Currency.Models;
	using Services.Category.Models;

	public class ServiceMappingProfile : Profile
	{
		public ServiceMappingProfile()
		{
			CreateMap<Category, CategoryViewModel>();

			CreateMap<Currency, CurrencyViewModel>();

			CreateMap<Account, AccountDropdownViewModel>();

			CreateMap<Account, AccountDetailsViewModel>()
				.ForMember(m => m.Transactions, mf => mf
					.MapFrom(s => s.Transactions.OrderByDescending(t => t.CreatedOn)));

			CreateMap<Account, AccountCardViewModel>();

			CreateMap<AccountType, AccountTypeViewModel>();
			
			CreateMap<Transaction, AccountDetailsTransactionViewModel>();

			CreateMap<Transaction, TransactionShortViewModel>()
				.ForMember(m => m.AccountName, mf => mf
					.MapFrom(s => s.Account.Name + (s.Account.IsDeleted ? " (Deleted)" : string.Empty)));
			
			CreateMap<Transaction, TransactionFormModel>();

			CreateMap<Transaction, TransactionExtendedViewModel>()
				.ForMember(m => m.CategoryName, mf => mf
					.MapFrom(s => s.Category.Name + (s.Category.IsDeleted ? " (Deleted)" : string.Empty)))
				.ForMember(m => m.AccountName, mf => mf.MapFrom(s => s.Account.Name + (s.Account.IsDeleted ? " (Deleted)" : string.Empty)));
		}
	}
}
