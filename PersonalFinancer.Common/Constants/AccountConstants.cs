namespace PersonalFinancer.Common.Constants
{
	public static class AccountConstants
	{
		public const int AccountNameMaxLength = 20;
		public const int AccountNameMinLength = 2;

		public const double AccountInitialBalanceMaxValue = 10_000_000;
		public const double AccountInitialBalanceMinValue = -10_000_000;

		public const string AccountCacheKeyValue = "accounts/";
		public const string DeletedAccountCacheKeyValue = "deletedAccounts/";
	}
}
