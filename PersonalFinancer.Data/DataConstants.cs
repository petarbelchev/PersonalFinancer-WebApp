﻿namespace PersonalFinancer.Data
{
	public class DataConstants
	{
		public class User
		{
			public const int UserFirstNameMaxLength = 50;
			public const int UserFirstNameMinLength = 2;

			public const int UserLastNameMaxLength = 50;
			public const int UserLastNameMinLength = 2;

			public const int UserPasswordMaxLength = 18;
			public const int UserPasswordMinLength = 6;
		}

		public class Account
		{
			public const int AccountNameMaxLength = 50;
			public const int AccountNameMinLength = 2;

			public const double AccountInitialBalanceMaxValue = 10_000_000;
			public const double AccountInitialBalanceMinValue = 0.00;
		}

		public class AccountType
		{
			public const int AccountTypeNameMaxLength = 50;
			public const int AccountTypeNameMinLength = 2;
		}

		public class Category
		{
			public const int CategoryNameMaxLength = 50;
			public const int CategoryNameMinLength = 2;
		}

		public class Transaction
		{
			public const int TransactionRefferenceMaxLength = 100;
			public const int TransactionRefferenceMinLength = 4;

			public const double TransactionMaxValue = 100_000.00;
			public const double TransactionMinValue = 0.01;
		}
	}
}