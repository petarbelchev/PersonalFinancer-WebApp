namespace PersonalFinancer.Web
{
    public static class Constants
    {
        public static class UrlPathConstants
        {
            public const string ApiCategoriesPath = "/api/categories/";
            public const string ApiTransactionsPath = "/api/transactions/";
            public const string ApiAccountTypesPath = "/api/accounttypes/";
            public const string ApiCurrencyPath = "/api/currencies/";
            public const string ApiAccountsPath = "/api/accounts/";
            public const string ApiAccountsCashFlowPath = "/api/accounts/cashflow";
            public const string ApiAccountTransactionsPath = "/api/accounts/transactions";
            public const string ApiUsersPath = "/api/users/";

            public const string TransactionDetailsPath = "/Transactions/TransactionDetails/";
            public const string AdminTransactionDetailsPath = "/Admin/Transactions/TransactionDetails/";

            public const string AdminUserDetailsPath = "/Admin/Users/Details/";

            public const string AccountDetailsPath = "/Accounts/AccountDetails/";
            public const string AdminAccountDetailsPath = "/Admin/Accounts/AccountDetails/";

			public const string BadRequestImgPath = "/images/400BadRequest.webp";
            public const string InternalServerErrorImgPath = "/images/500InternalServerError.webp";
            public const string NotFoundImgPath = "/images/404NotFound.webp";
        }
    }
}
