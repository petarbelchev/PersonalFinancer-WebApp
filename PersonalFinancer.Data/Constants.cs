namespace PersonalFinancer.Data
{
    public static class Constants
    {
        public static class UserConstants
        {
            public const int UserNameMaxLength = 25;
            public const int UserNameMinLength = 2;

            public const int UserFirstNameMaxLength = 50;
            public const int UserFirstNameMinLength = 2;

            public const int UserLastNameMaxLength = 50;
            public const int UserLastNameMinLength = 2;

            public const int UserPasswordMaxLength = 18;
            public const int UserPasswordMinLength = 6;

            public const int PhoneNumberLength = 10;
        }

        public static class AccountConstants
        {
            public const int AccountNameMaxLength = 20;
            public const int AccountNameMinLength = 2;

            public const double AccountInitialBalanceMaxValue = 10_000_000;
            public const double AccountInitialBalanceMinValue = -10_000_000;

            public const string AccountCacheKeyValue = "accounts/";
            public const string DeletedAccountCacheKeyValue = "deletedAccounts/";
		}

        public static class AccountTypeConstants
        {
            public const int AccountTypeNameMaxLength = 20;
            public const int AccountTypeNameMinLength = 2;

            public const string AccTypeCacheKeyValue = "accountTypes/";
            public const string DeletedAccTypeCacheKeyValue = "deletedAccountTypes/";
		}

        public static class CurrencyConstants
        {
            public const int CurrencyNameMaxLength = 10;
            public const int CurrencyNameMinLength = 2;

            public const string CurrencyCacheKeyValue = "currencies/";
            public const string DeletedCurrencyCacheKeyValue = "deletedCurrencies/";
		}

        public static class TransactionConstants
        {
            public const int TransactionReferenceMaxLength = 100;
            public const int TransactionReferenceMinLength = 4;

            public const double TransactionMaxValue = 100_000.00;
            public const double TransactionMinValue = 0.01;
        }

        public static class CategoryConstants
        {
            public const int CategoryNameMaxLength = 25;
            public const int CategoryNameMinLength = 2;

            public const string CategoryInitialBalanceName = "Initial Balance";
            public const string InitialBalanceCategoryId = "e241b89f-b094-4f79-bb09-efc6f47c2cb3";

            public const string CategoryCacheKeyValue = "categories/";
            public const string DeletedCategoryCacheKeyValue = "deletedCategories/";
		}

        public static class SeedConstants
        {
            public const string FirstUserEmail = "petar@mail.com";
            public const string AdminEmail = "admin@admin.com";

            public const string FirstUserPassword = "petar123";
            public const string AdminPassword = "admin123";

            public const string FirstUserCashAccountTypeId = "f4c3803a-7ed5-4d78-9038-7b21bf08a040";
            public const string FirstUserBankAccountTypeId = "1dfe1780-daed-4198-8360-378aa33c5411";
            public const string FirstUserSavingAccountTypeId = "daef2351-e2e9-43b9-b908-8d7d00bf3df6";

            public const string FoodDrinkCategoryId = "93cebd34-a9f5-4862-a8c9-3b6eea63e94c";
            public const string UtilitiesCategoryId = "d59cbb57-3b9e-4b37-9b74-a375eecba8c8";
            public const string TransportCategoryId = "b58a7947-eecf-40d0-b84e-c6947fcbfd86";
            public const string MedicalHealthcareCategoryId = "96e441e3-c5a6-427f-bb32-85940242d9ee";
            public const string SalaryCategoryId = "081a7be8-15c4-426e-872c-dfaf805e3fec";
            public const string MoneyTransferCategoryId = "e03634d5-1970-4e01-8568-42756e9ad973";
            public const string DividendsCategoryId = "459dc945-0d2c-4a07-a2aa-55b4c5e57f9f";

            public const string FirstUserBGNCurrencyId = "3bf454ad-941b-4ab6-a1ad-c212bfc46e7d";
            public const string FirstUserEURCurrencyId = "dab2761d-acb1-43bc-b56b-0d9c241c8882";
            public const string FirstUserUSDCurrencyId = "2f2c29e5-4463-4d5d-bfd2-e0f973c24e8f";

            public const string CashBgnAccountId = "ca5f67dd-78d7-4bb6-b42e-6a73dd79e805";
            public const string BankBgnAccountId = "ba7def5d-b00c-4e05-8d0b-5df2c47273b5";
            public const string BankEurAccountId = "44c67e3a-2dfe-491c-b7fc-eb78fe8b8946";
            public const string BankUsdAccountId = "303430dc-63a3-4436-8907-a274ec29f608";
        }

        public static class RoleConstants
        {
            public const string AdminRoleName = "Administrator";
            public const string UserRoleName = "User";

            public const int NameMaxLength = 25;
            public const int NameMinLength = 3;
        }
    }
}
