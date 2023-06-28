namespace PersonalFinancer.Web
{
    public static class Constants
    {
        public static class UrlPathConstants
        {
            public const string ApiCategoriesEndpoint = "/api/categories/";
            public const string ApiTransactionsEndpoint = "/api/transactions/";
            public const string ApiAccountTypesEndpoint = "/api/accounttypes/";
            public const string ApiCurrencyEndpoint = "/api/currencies/";
            public const string ApiAccountsEndpoint = "/api/accounts/";
            public const string ApiAccountsCashFlowEndpoint = "/api/accounts/cashflow";
            public const string ApiAccountTransactionsEndpoint = "/api/accounts/transactions/";
            public const string ApiUsersEndpoint = "/api/users/";

            public const string TransactionDetailsPath = "/Transactions/TransactionDetails/";
            public const string TransactionEditPath = "/Transactions/EditTransaction/";

            public const string AdminUserDetailsPath = "/Admin/Users/Details/";

            public const string AccountDetailsPath = "/Accounts/AccountDetails/";

			public const string BadRequestImgPath = "/images/400BadRequest.webp";
            public const string InternalServerErrorImgPath = "/images/500InternalServerError.webp";
            public const string NotFoundImgPath = "/images/404NotFound.webp";
        }
    }
}
