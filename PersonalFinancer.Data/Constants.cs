namespace PersonalFinancer.Data
{
	public class Constants
	{
		public class UserConstants
		{
			public const int UserFirstNameMaxLength = 50;
			public const int UserFirstNameMinLength = 2;

			public const int UserLastNameMaxLength = 50;
			public const int UserLastNameMinLength = 2;

			public const int UserPasswordMaxLength = 18;
			public const int UserPasswordMinLength = 6;
		}

		public class AccountConstants
		{
			public const int AccountNameMaxLength = 15;
			public const int AccountNameMinLength = 2;

			public const double AccountInitialBalanceMaxValue = 10_000_000;
			public const double AccountInitialBalanceMinValue = 0.00;

			public const string CacheKeyValue = "accounts/";
		}

		public class AccountTypeConstants
		{
			public const int AccountTypeNameMaxLength = 15;
			public const int AccountTypeNameMinLength = 2;

			public const string CacheKeyValue = "accountTypes/";
		}

		public class CategoryConstants
		{
			public const int CategoryNameMaxLength = 25;
			public const int CategoryNameMinLength = 2;

			public const string CategoryInitialBalanceName = "Initial Balance";

			public const string CacheKeyValue = "categories/";
		}

		public class TransactionConstants
		{
			public const int TransactionRefferenceMaxLength = 100;
			public const int TransactionRefferenceMinLength = 4;

			public const double TransactionMaxValue = 100_000.00;
			public const double TransactionMinValue = 0.01;
		}

		public class CurrencyConstants
		{
			public const int CurrencyNameMaxLength = 10;
			public const int CurrencyNameMinLength = 2;

			public const string CacheKeyValue = "currencies/";
		}

		public static class HostConstants
		{
			private const string hostUrl = "https://localhost:7187";

			public const string ApiCategoriesUrl = hostUrl + "/api/categories/";
			public const string ApiTransactionsUrl = hostUrl + "/api/transactions/";
			public const string ApiAccountTypesUrl = hostUrl + "/api/accounttypes/";
			public const string ApiCurrencyUrl = hostUrl + "/api/currencies/";
		}

		public static class RoleConstants
		{
			public const string AdminRoleName = "Administrator";
			public const string UserRoleName = "User";
		}
	}
}
